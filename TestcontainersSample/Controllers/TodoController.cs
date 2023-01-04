using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestcontainersSample.DatabaseContext;

namespace TestcontainersSample.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly TodoContext _todoContext;

    public TodoController(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }

    [HttpGet]
    public async Task<ActionResult<TodoItem?>> GetAsync(int page, int pageSize)
    {
        var items = await _todoContext.TodoItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{itemId}", Name = "GetTodoItem")]
    public async Task<ActionResult<TodoItem?>> GetByIdAsync(int itemId)
    {
        var item = await _todoContext.TodoItems
            .SingleOrDefaultAsync(a => a.ItemId == itemId);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<int>> PostAsync(TodoItem todoItem)
    {
        _todoContext.Add(todoItem);

        await _todoContext.SaveChangesAsync();
        
        return CreatedAtRoute("GetTodoItem", new { itemId = todoItem.ItemId }, todoItem);
    }
}
