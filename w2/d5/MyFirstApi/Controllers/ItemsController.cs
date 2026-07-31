using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMessageService _messageService;

    public ItemsController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    private static readonly List<string> Items = new()
    {
        "قلم", "دفتر", "شنطة", "كتاب"
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Items);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id < 0 || id >= Items.Count)
        {
            return NotFound();
        }

        return Ok(Items[id]);
    }

    [HttpGet("message")]
    public IActionResult GetMessage()
    {
        return Ok(_messageService.GetMessage());
    }
}