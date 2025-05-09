using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Controllers.DTOs;
using WebApplication1.Exceptions;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips([FromRoute]int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid Client Id");
        }
        
        var connectionString = configuration.GetConnectionString("Default");
        
        if (!await CheckClientExists(id, connectionString))
        {
            return NotFound($"There is no client with {id} id");
        }
        
        
        await using var connection = new SqlConnection(connectionString);
        var sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.IdCountry, c.Name AS CountryName, ct.RegisteredAt, ct.PaymentDate FROM Client_Trip ct JOIN Trip t ON ct.IdTrip = t.IdTrip LEFT JOIN Country_Trip ct2 ON t.IdTrip = ct2.IdTrip LEFT JOIN Country c ON ct2.IdCountry = c.IdCountry WHERE ct.IdClient = @ClientId ORDER BY t.IdTrip";
        
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ClientId", id);
        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();
        
        var result = new List<ClientTripDTO>();
        
        
        var lastId = 0;
        while (await reader.ReadAsync())
        {
            if (reader.GetInt32(0) > lastId)
            {
                result.Add(new ClientTripDTO()
                {
                    IdTrip = reader.GetInt32(0), 
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    Countries = new List<CountryGetDTO>(), 
                    RegisteredAt = reader.GetInt32(8),
                    PaymentDate = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                });
                lastId = reader.GetInt32(0);
            }
            
            result.Last().Countries.Add(new CountryGetDTO()
            {
                IdCountry = reader.GetInt32(6),
                Name = reader.GetString(7),
            });
        }

        if (result.Count <= 0)
        {
            return NotFound($"This Client doesnt have any booked trips");
        }
        
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientById([FromRoute] int id)
    {
        DbService service = new DbService(configuration);
        try
        {
            return Ok(await service.GetClientById(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    
    
    



    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientCreateDTO body)
    {
        DbService service = new DbService(configuration);
        var client = await service.CreateClient(body);
        return CreatedAtAction(nameof(GetClientById), new {id = client.IdClient}, client);
    }
    
    
    
    
    
    
    
    private async Task<bool> CheckClientExists(int clientId, string connectionString)
    {
        string checkQuery = "SELECT COUNT(*) FROM Client WHERE IdClient = @ClientId";
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = new SqlCommand(checkQuery, connection))
        {
            command.Parameters.AddWithValue("@ClientId", clientId);
            await connection.OpenAsync();
            int count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
    }
}