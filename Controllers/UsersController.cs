using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using first_dotnet_api.Data;
using first_dotnet_api.Models;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound(new
            {
                message = "User não encontrado",
                status = 404,
                idBuscado = id
            });        
        }

        return Ok(user);
    }


    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id}, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User updateUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.Name = updateUser.Name;
        user.Email = updateUser.Email;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
           return NotFound(new
            {
                message = "User não encontrado",
                status = 404,
                idBuscado = id
            });  

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            message = "User removido com sucesso",
            status = 200,
            user = new
            {
                user.Id,
                user.Name,
                user.Email
            }
        });  
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var filteredUser = await _context.Users
            .Where(u => u.Email == email)
            .ToListAsync();

        return Ok(filteredUser);        
    }
}