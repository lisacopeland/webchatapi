﻿using Microsoft.AspNetCore.Mvc;
using webchat.Models;
using webchat.Service;
using webchat.Utilities;

namespace webchat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _messageService;
        private readonly WebSocketService _websocketService;

        public MessagesController(MessageService messageService, WebSocketService webSocketService)
        {
            _messageService = messageService;
            _websocketService = webSocketService;
        }

        [HttpGet]
        public async Task<List<MessageClass>> Get() => await _messageService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<MessageClass>> Get(string id)
        {
            var MessageClass = await _messageService.GetAsync(id);
            ApiResponseClass result;
            if (MessageClass is null)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = "Message not found";
                return BadRequest(result);
            }

            return new JsonResult(MessageClass);
        }

        [HttpPost]
        public async Task<IActionResult> Post(MessageClass newMessageClass)
        {
            ApiResponseClass result;
            try
            {
                await _messageService.CreateAsync(newMessageClass);
                Message payload = new Message();
                payload.MessageClass = newMessageClass;
                ActionPayload actionPayload = new ActionPayload();
                actionPayload.Action = Constants.userMessageCreatedAction;
                actionPayload.Payload = payload;
                await _websocketService.AcceptWebSocketAsync(HttpContext);
                await _websocketService.SendMessageAsync(actionPayload);
                result = new ApiResponseClass { Success = true };
                result.Message = "Message created successfully";
                result.Id = newMessageClass._id;
                return Ok();
            }
            catch (Exception ex)
            {
                result = new ApiResponseClass { Success = false };
                result.Message = ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, MessageClass updatedMessageClass)
        {
            var MessageClass = await _messageService.GetAsync(id);

            if (MessageClass is null)
            {
                return NotFound();
            }

            updatedMessageClass._id = MessageClass._id;

            await _messageService.UpdateAsync(id, updatedMessageClass);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var MessageClass = await _messageService.GetAsync(id);

            if (MessageClass is null)
            {
                return NotFound();
            }

            await _messageService.RemoveAsync(id);

            return NoContent();
        }
    }
}
