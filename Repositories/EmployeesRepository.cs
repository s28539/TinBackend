using Microsoft.AspNetCore.Mvc;
using Npgsql;
using restAPI.Models;

namespace restAPI.Repositories;

public class EmployeesRepository : IEmployeesRepository
{
    private readonly IConfiguration _configuration;
    public EmployeesRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesEmployeeExist(long id)
    {
        var query = "SELECT 1 FROM employee WHERE employee_id = @ID";

        await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using NpgsqlCommand command = new NpgsqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
   
    public async Task<EmployeeDto> EmployeeDetails(long id)
    {
            var query = "SELECT * FROM employee WHERE employee_id = @ID";
            Console.WriteLine(query);

            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EmployeeDto
                {
                    EmployeeID = reader.GetInt64(reader.GetOrdinal("employee_id")),  
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Surname = reader.GetString(reader.GetOrdinal("surname"))
                };
            }

            return null;
    }
    
    
    public async Task<List<EmployeeDto>> Employees()
    {
        List<EmployeeDto> employees = new List<EmployeeDto>();
        var query = "SELECT * FROM employee";
        

        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new NpgsqlCommand(query, connection);

        await connection.OpenAsync();
        
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(new EmployeeDto()
            {
                EmployeeID = reader.GetInt64(reader.GetOrdinal("employee_id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Surname = reader.GetString(reader.GetOrdinal("surname"))
            });
        }
        return employees;
    }
    public async Task<bool> AddEmployee(EmployeeDto newEmployeeDto)
    {
        var query = "INSERT INTO employee (employee_id,name, surname) VALUES (@Pracownik_id,@Name, @Surname)";

        try
        {
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", newEmployeeDto.Name);
            command.Parameters.AddWithValue("@Surname", newEmployeeDto.Surname);
            command.Parameters.AddWithValue("@Pracownik_id", newEmployeeDto.EmployeeID);
            await connection.OpenAsync();

            var result = await command.ExecuteNonQueryAsync();
            return result == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dodawania pracownika: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteEmployee(long id)
    {
        
        var query1 = "DELETE FROM shift WHERE employee_id = @EmployeeId";
        var query2 = "DELETE FROM employee WHERE employee_id = @EmployeeId";
        var query3 = "DELETE FROM raporthistory WHERE employee_id = @EmployeeId";

        try
        {
            
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();

            
            await using var transaction = await connection.BeginTransactionAsync();

           
            await using var command1 = new NpgsqlCommand(query1, connection, transaction);
            command1.Parameters.AddWithValue("@EmployeeId", id);
            await command1.ExecuteNonQueryAsync();

           
            await using var command3 = new NpgsqlCommand(query3, connection, transaction);
            command3.Parameters.AddWithValue("@EmployeeId", id);
            await command3.ExecuteNonQueryAsync();

            
            await using var command2 = new NpgsqlCommand(query2, connection, transaction);
            command2.Parameters.AddWithValue("@EmployeeId", id);
            var result = await command2.ExecuteNonQueryAsync();

            
            await transaction.CommitAsync();

            
            return result > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas usuwania pracownika: {ex.Message}");
            throw;
        }
    }


}
