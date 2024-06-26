using Microsoft.AspNetCore.Mvc;
using webchat.Models;
using webchat.Service;

namespace webchat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessagesController(MessageService messageService) =>
            _messageService = messageService;

        [HttpGet]
        public async Task<List<MessageClass>> Get() =>
            await _messageService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<MessageClass>> Get(string id)
        {
            var MessageClass = await _messageService.GetAsync(id);

            if (MessageClass is null)
            {
                return NotFound();
            }

            return MessageClass;
        }

        [HttpPost]
        public async Task<IActionResult> Post(MessageClass newMessageClass)
        {
            await _messageService.CreateAsync(newMessageClass);

            return CreatedAtAction(nameof(Get), new { id = newMessageClass._id }, newMessageClass);
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
