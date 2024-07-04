using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using webchat.Models;
using webchat.Service;
using webchat.Utilities;
using Constants = webchat.Utilities.Constants;

namespace webchat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("LoggedInUsers")]
        public async Task<List<UserClass>> GetLoggedInUsers() =>
            await _userService.GetLoggedInUsersAsync();

        [HttpGet]
        public async Task<List<UserClass>> Get() => await _userService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(UserClass newUserClass)
        {
            ApiResponseClass result;
            try
            {
                await _userService.CreateAsync(newUserClass);
                result = new ApiResponseClass { Success = true };
                result.Message = "User successfully created";
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, UserClass updatedUserClass)
        {
            var UserClass = await _userService.GetAsync(id);

            if (UserClass is null)
            {
                return NotFound();
            }

            updatedUserClass._id = UserClass._id;

            await _userService.UpdateAsync(id, updatedUserClass);

            return Ok();
        }
    }
}
