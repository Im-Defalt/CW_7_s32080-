using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Controllers;


[ApiController]
[Route("[controller]")]
public class TripController(IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrip()
    {
        var result = new List<TripGetDTO>();
        
        var connectionString = configuration.GetConnectionString("Default");
        
        await using var connection = new SqlConnection(connectionString);
        var sql = "SELECT IdTrip, Name, Description, DateFrom, DateTo, MaxPeople FROM Trip";
        
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new TripGetDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5)
            });
        }
        
        return Ok(result);
    }
    
}