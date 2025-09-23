using System.Collections.Generic;

namespace NovaSecTest
{
    public interface IInjectedTestObject
    {
        bool ReturnBoolean(bool input);
        string ReturnString(string input);
        bool InputStringList(List<string> input);
    }
}