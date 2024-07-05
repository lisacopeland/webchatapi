﻿using Microsoft.AspNetCore.Mvc;
using webchat.Models;
using webchat.Service;
using webchat.Utilities;

namespace webchat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly WebSocketService _websocketService;
        private readonly UserService _userService;

        public AuthController(UserService userService, WebSocketService webSocketService)
        {
            _websocketService = webSocketService;
            _userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginClass body)
        {
            ApiResponseClass result;
            var UserClass = await _userService.GetByUsernameAsync(body.UserName);

            if (UserClass is null)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = "User not found";
                return BadRequest(result);
            }

            UserClass.Online = true;

            User userPayload = new User();
            userPayload.UserClass = UserClass;
            ActionPayload actionPayload = new ActionPayload();
            actionPayload.Action = Constants.userEnteredAction;
            actionPayload.Payload = userPayload;
            await _websocketService.SendMessageAsync(actionPayload);
            await _userService.UpdateAsync(UserClass._id, UserClass);
            return new JsonResult(UserClass);
        }

        [HttpPut]
        [Route("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutClass body)
        {
            var UserClass = await _userService.GetAsync(body.Id);
            ApiResponseClass result;
            if (UserClass is null)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = "User not found";
                return BadRequest(result);
            }

            UserClass.Online = false;

            UserId payload = new UserId();
            payload.id = body.Id;
            ActionPayload actionPayload = new ActionPayload();
            actionPayload.Action = Constants.userExitedAction;
            actionPayload.Payload = payload;
            await _websocketService.SendMessageAsync(actionPayload);
            await _userService.UpdateAsync(body.Id, UserClass);

            result = new ApiResponseClass { Success = true };
            result.Message = "User successfully logged out";
            return new JsonResult(result);
        }
    }
}
