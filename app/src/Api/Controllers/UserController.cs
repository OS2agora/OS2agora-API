using BallerupKommune.Api.Models.DTOs;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.Api.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<UserDto>>>> GetUsers()
        {
            var users = await Mediator.Send(new GetUsersQuery());

            var userDtoList = users.Select(user => Mapper.Map<User, DTOs.Models.UserDto>(user)).ToList();
            return Ok(userDtoList);
        }
    }
}