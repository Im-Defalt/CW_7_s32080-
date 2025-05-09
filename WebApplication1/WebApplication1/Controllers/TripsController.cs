using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TripsController(IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrip()
    {
        var result = new List<TripGetDTO>();
        
        var connectionString = configuration.GetConnectionString("Default");
        
        await using var connection = new SqlConnection(connectionString);
        var sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.IdCountry, c.Name AS CountryName FROM Trip t LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip LEFT JOIN Country c ON ct.IdCountry = c.IdCountry ORDER BY t.IdTrip";
        
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        var lastId = 0;
        while (await reader.ReadAsync())
        {
            if (reader.GetInt32(0) > lastId)
            {
                result.Add(new TripGetDTO
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    Countries = new List<CountryGetDTO>()
                });
                lastId = reader.GetInt32(0);
            }

            result.Last().Countries.Add(new CountryGetDTO()
            {
                IdCountry = reader.GetInt32(6),
                Name = reader.GetString(7),
            });
        }
        
        return Ok(result);
    }
    
}