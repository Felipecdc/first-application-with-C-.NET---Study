using Microsoft.AspNetCore.Mvc;
using first_dotnet_api.Models;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private static List<TaskItem> tasks = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(tasks);

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        // return task == null ? NotFound() : Ok(task);
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
    public IActionResult Create(TaskItem task)
    {
        task.Id = tasks.Count + 1;
        tasks.Add(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id}, task);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, TaskItem updatedTask)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
            return NotFound();

        task.Title = updatedTask.Title;
        task.IsCompleted = updatedTask.IsCompleted;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) 
            return NotFound(new
                {
                    message = "Task não encontrada",
                    status = 404,
                    idBuscado = id
                });        

        tasks.Remove(task);

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
}