using AutoMapper;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Extensions;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Models
{
    public class UserDao : BaseDao<UserEntity, User>, IUserDao
    {
        public UserDao(IApplicationDbContext db, ILogger<BaseDao<UserEntity, User>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) :
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<User> GetAsync(int id, IncludeProperties includes = null)
        {
            var userEntity = await base.GetAsync(id, includes);
            return MapAndPrune(userEntity, includes);
        }

        public new async Task<List<User>> GetAllAsync(IncludeProperties includes = null)
        {
            var userEntities = await base.GetAllAsync(includes);
            return userEntities.Select(userEntity => MapAndPrune(userEntity, includes)).ToList();
        }

        public async Task<User> FindUserByIdentifier(string identifier, IncludeProperties includes = null)
        {
            if (identifier == null)
            {
                return null;
            }
            var allUsers = await GetAllAsync(includes);
            var possibleUser = allUsers.SingleOrDefault(user => user.Identifier == identifier);
            return possibleUser;
        }

        public async Task<User> FindUserByPersonalIdentifier(string personalIdentifier,
            IncludeProperties includes = null)
        {
            var allUsers = await GetAllAsync(includes);
            var possibleUser = allUsers.SingleOrDefault(user => 
                string.Equals(user.PersonalIdentifier, personalIdentifier, StringComparison.CurrentCultureIgnoreCase));
            return possibleUser;
        }

        public async Task<User> FindUserByEmail(string email, IncludeProperties includes = null)
        {
            var allUsers = await GetAllAsync(includes);
            var possibleUser = allUsers.SingleOrDefault(user =>
                string.Equals(user.Email, email, StringComparison.CurrentCultureIgnoreCase));
            return possibleUser;
        }

        public async Task<User> UpdateAsync(User model, IncludeProperties includes = null)
        {
            var userEntity = await UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(userEntity, includes);
        }

        public new async Task<User> CreateAsync(User model, IncludeProperties includes = null)
        {
            var userEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(userEntity, includes);
        }

        public new async Task<List<User>> CreateRangeAsync(List<User> models, IncludeProperties includes = null)
        {
            var userEntities = await base.CreateRangeAsync(models, includes);
            return userEntities.Select(userEntity => MapAndPrune(userEntity, includes)).ToList();
        }
    }
}