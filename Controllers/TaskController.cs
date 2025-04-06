using Microsoft.AspNetCore.Mvc;
using first_dotnet_api.Models;

[ApiController] // Marca essa classe como um controller de API
[Route("[controller]")] // Define a rota base: /task
public class TaskController : ControllerBase
{
    private static List<TaskItem> tasks = new(); // Lista em memória com as tarefas

    [HttpGet] // GET /task → retorna todas as tarefas
    public IActionResult GetAll() => Ok(tasks);

    [HttpGet("{id}")] // GET /task/{id} → retorna uma tarefa específica
    public IActionResult GetById(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id); // Busca tarefa pelo ID

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id            
            });
        } 
        return Ok(task); // Retorna a tarefa se existir
    }

    [HttpPost] // POST /task → cria uma nova tarefa
    public IActionResult Create(TaskItem task)
    {
        task.Id = tasks.Count + 1; // Define ID automaticamente
        tasks.Add(task); // Adiciona à lista
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task); // Retorna 201
    }

    [HttpPut("{id}")] // PUT /task/{id} → atualiza uma tarefa
    public IActionResult Update(int id, TaskItem updatedTask)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id); // Busca pelo ID
        if (task == null)
            return NotFound(); // Retorna 404 se não achar

        // Atualiza os dados da tarefa
        task.Title = updatedTask.Title;
        task.IsCompleted = updatedTask.IsCompleted;

        return NoContent(); // Retorna 204 (sem conteúdo)
    }

    [HttpDelete("{id}")] // DELETE /task/{id} → remove uma tarefa
    public IActionResult Delete(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id); // Busca tarefa
        if (task == null) 
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id
            });        

        tasks.Remove(task); // Remove da lista

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

    [HttpGet("completed/{status}")] // GET /task/completed/{true/false} → filtra por status
    public IActionResult GetByCompletionStatus(bool status)
    {
        var filteredTasks = tasks
            .Where(t => t.IsCompleted == status) // Filtra pelo status
            .ToList();

        return Ok(filteredTasks); // Retorna tarefas filtradas
    }

    [HttpPut("{id}/complete")] // PUT /task/{id}/complete → marca como concluída
    public IActionResult MarkAsCompleted(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id); // Busca tarefa
        if (task == null)
            return NotFound(new
            {
                message = "Task não encontrada",
                status = 404,
                idBuscado = id
            }); 
        
        task.IsCompleted = true; // Marca como completa
        return Ok(task); // Retorna a tarefa atualizada
    }

    [HttpGet("paged")] // GET /task/paged?page=1&pageSize=5 → paginação
    public IActionResult GetPaged(int page = 1, int pageSize = 5)
    {
        // Pega somente os itens da página atual
        var pagedTasks = tasks 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Calcula total de páginas
        var totalPages = (int)Math.Ceiling((double)tasks.Count / pageSize);

        // Retorna metadados da paginação + tarefas
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
