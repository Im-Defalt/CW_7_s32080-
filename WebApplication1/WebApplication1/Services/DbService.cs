using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Controllers.DTOs;
using WebApplication1.Exceptions;

namespace WebApplication1.Services;

public interface IDbService
{
    //public Task<IEnumerable<>
}

public class DbService(IConfiguration configuration) : IDbService
{

    public async Task<ClientGetDTO> GetClientById(int id)
    {
        var connectionString = configuration.GetConnectionString("Default");
        
        await using var connection = new SqlConnection(connectionString);
        var sql = "select IdClient, FirstName, LastName, Email, Telephon, Pesel from Client where Id = @Id";
        
        await using var command = new SqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
             throw new NotFoundException($"Client with {id} id was not found");
        }

        return new ClientGetDTO
        {
            IdClient = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            Telephone = reader.GetString(4),
            Pesel = reader.GetString(5)
        };
    }
    
    
    
    public async Task<ClientGetDTO> CreateClient(ClientCreateDTO client)
    {
        var connectionString = configuration.GetConnectionString("Default");
        
        await using var connection = new SqlConnection(connectionString);
        var sql =
            "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); select scope_identity()";
        
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("FirstName", client.FirstName);
        command.Parameters.AddWithValue("LastName", client.LastName);
        command.Parameters.AddWithValue("Email", client.Email);
        command.Parameters.AddWithValue("Telephone", client.Telephone);
        command.Parameters.AddWithValue("Pesel", client.Pesel);
        
        await connection.OpenAsync();
        
        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new ClientGetDTO()
        {
            IdClient = newId,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Telephone = client.Telephone,
            Pesel = client.Pesel
        };
    }
}