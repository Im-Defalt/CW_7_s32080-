using System.Runtime.InteropServices.JavaScript;
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




    public async Task<int> CheckClientExists(int IdClient)
    {
        var sql = "select count(*) from Client where IdClient = @IdClient";

        using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdClient", IdClient);
                return (int)await command.ExecuteScalarAsync();
            }
        }
    }
    
    public async Task<int> CheckTripExists(int TripId)
    {
        var sql = @"select count(*) from Trip where IdTrip = @IdTrip";

        using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.AddWithValue("@IdTrip", TripId);
                int result = (int)await command.ExecuteScalarAsync();
                return result;
                

            }
        }
    }
    
    public async Task<int> CheckIfFull(int IdTrip)
    {
        var sql = @"SELECT 
    CASE 
        WHEN COUNT(ct.IdClient) >= t.MaxPeople THEN 1 
        ELSE 0 
    END AS IsFull
FROM 
    Trip t
LEFT JOIN 
    Client_Trip ct ON t.IdTrip = ct.IdTrip
WHERE 
    t.IdTrip = @IdTrip
GROUP BY 
    t.MaxPeople;";

        using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.AddWithValue("@IdTrip", IdTrip);
                int result = (int)await command.ExecuteScalarAsync();
                return result;
                

            }
        }
    }

    public async Task UpdateTrip(int IdClient, int TripId)
    {
        var sql = @"INSERT INTO Client_Trip (IdClient, IdTrip , RegisteredAt) values (@IdClient, @IdTrip , @RegisteredAt)";
        using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdClient", IdClient);
                command.Parameters.AddWithValue("@IdTrip", TripId);
                command.Parameters.AddWithValue("@RegisteredAt", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
                await command.ExecuteNonQueryAsync();
                
            }
        }
    }
    
    
    
}