using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using first_dotnet_api.Data;
using first_dotnet_api.Models;

[ApiController]
[Route("[controller]")] 
public class TaskController : ControllerBase
{

    private readonly AppDbContext _context;
    
    public TaskController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet] 
    public async Task<IActionResult> GetAll() 
    {
        var tasks = await _context.TaskItems.ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")] 
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null)
        {
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id            
            });
        } 
        
        return Ok(task); 
    }

    [HttpPost] 
    public async Task<IActionResult> Create(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow; 
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskItem updatedTask)
    {
        var task = await _context.TaskItems.FindAsync(id); 
        if (task == null)
            return NotFound();

        task.Title = updatedTask.Title;
        task.IsCompleted = updatedTask.IsCompleted;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null) 
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id
            });        

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Task removida com sucesso",
            status = 200,
            task = new
            {
                task.Id,
                task.Title,
                task.IsCompleted
            }
        });    
    }

    [HttpGet("completed/{status}")] 
    public async Task<IActionResult> GetByCompletionStatus(bool status)
    {
        var filteredTasks = await _context.TaskItems
            .Where(t => t.IsCompleted == status)
            .ToListAsync();

        return Ok(filteredTasks); 
    }

    [HttpPut("{id}/complete")] 
    public async Task<IActionResult> MarkAsCompleted(int id)
    {
        var task = await _context.TaskItems.FindAsync(id); 
        if (task == null)
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id
            }); 
        
        task.IsCompleted = true; 

        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpGet("paged")] 
    public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 5)
    {

        var tasks = await _context.TaskItems.ToListAsync();
        var pagedTasks = tasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling((double)tasks.Count / pageSize);

        return Ok(new 
        {
            page,
            pageSize,
            totalPages,
            totalTasks = tasks.Count,
            data = pagedTasks
        });
    }
}
