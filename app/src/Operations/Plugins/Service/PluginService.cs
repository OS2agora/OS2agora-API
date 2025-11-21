using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces.Plugins;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Agora.Operations.Common.Telemetry;
using Agora.Operations.Plugins.Plugins;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService : IPluginService
    {
        private static Assembly CurrentAssembly => typeof(PluginService).Assembly;

        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IEnumerable<Type> _allPluginTypes;

        public PluginService(IServiceProvider serviceProvider, IOptions<PluginOptions> pluginOptions)
        {
            _serviceProvider = serviceProvider;
            _pluginOptions = pluginOptions;
            _allPluginTypes = LoadPlugins();
        }

        private IEnumerable<Type> LoadPlugins()
        {
            var targetNamespaces = _pluginOptions.Value.LoadAllFromNameSpaces;
            var pluginTypes = CurrentAssembly.GetTypes()
                .Where(type => 
                    targetNamespaces.Any(nameSpace => nameSpace.Equals(type.Namespace, StringComparison.InvariantCultureIgnoreCase)) &&
                    typeof(PluginBase).IsAssignableFrom(type) && 
                    type.IsClass && !type.IsAbstract && type.GetInterfaces().Any(i => i == typeof(IPlugin)));
            return pluginTypes;
        }

        private PluginConfiguration GetPluginConfigurationFromType(Type type)
        {
            var pluginName = _pluginOptions.Value.Configurations.Keys.SingleOrDefault(key => key.Equals(type.Name, StringComparison.InvariantCultureIgnoreCase));
            return pluginName != null ? _pluginOptions.Value.Configurations[pluginName] : null;
        }

        private bool IsPluginDisabled(Type type)
        {
            var conf = GetPluginConfigurationFromType(type);
            var pluginIsConfigured = conf != null;
            return pluginIsConfigured && conf.Disabled;
        }

        private bool IsEventDisabled(Type type, string memberName)
        {
            var conf = GetPluginConfigurationFromType(type);
            return conf != null && conf.EventConfigurations.Any(config =>
                       config.Name.Equals(memberName, StringComparison.InvariantCultureIgnoreCase) &&
                       config.Disabled);
        }

        private async Task<T> InvokePlugin<T>(Type t, string memberName, params object[] args) where T : class
        {
            if (IsEventDisabled(t, memberName))
            {
                return null;
            }

            var methodToCall = t.GetMethod(memberName);
            if (methodToCall?.DeclaringType == t)
            {
                var classInstance = Activator.CreateInstance(t, _serviceProvider, GetPluginConfigurationFromType(t));
                return await (Task<T>)methodToCall.Invoke(classInstance, args);
            }

            return null;
        }

        private async Task InvokePlugin(Type t, string memberName, params object[] args)
        {
            if (!IsEventDisabled(t, memberName))
            {
                var methodToCall = t.GetMethod(memberName);
                if (methodToCall?.DeclaringType == t)
                {
                    using var activity = StartPluginActivity(t, methodToCall);
                    var classInstance = Activator.CreateInstance(t, _serviceProvider, GetPluginConfigurationFromType(t));
                    await (Task)methodToCall.Invoke(classInstance, args);
                }
            }
        }

        private async Task<T> InvokeMethodOnPlugins<T>(T seed, Func<Type, T, Task<T>> methodInvoker) where T : class
        {
            var result = seed;

            foreach (var type in _allPluginTypes)
            {
                if (IsPluginDisabled(type))
                {
                    continue;
                }

                var newResult = await methodInvoker(type, result);
                result = newResult ?? result;
            }

            return result;
        }

        private async Task InvokeMethodOnPlugins(Func<Type, Task> methodInvoker)
        {
            foreach (var type in _allPluginTypes)
            {
                if (IsPluginDisabled(type))
                {
                    continue;
                }

                await methodInvoker(type);
            }
        }

        private static Activity StartPluginActivity(Type type, MethodInfo methodInfo)
        {
            var activity = Instrumentation.Source.StartActivity("Invoke Plugin Activity");
            activity?.SetTag("plugin.full.name", type.FullName);
            activity?.SetTag("plugin.method.name", methodInfo.Name);
            return activity;
        }
    }
}