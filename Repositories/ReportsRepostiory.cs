using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using restAPI.Models;

namespace restAPI.Repositories;

public class ReportsRepostiory : IReportsRepository
{
    private readonly IConfiguration _configuration;
    public ReportsRepostiory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<double> GetWorkedHours(long employeeId, DateTime startTime, DateTime stopTime)
    {
        var query = @"SELECT start_date,stop_date FROM shift WHERE start_date <= @stop_time AND stop_date >= @start_timeAND employee_id = @employeeID;";
        
        try
        {
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new NpgsqlCommand(query, connection);

            
            command.Parameters.AddWithValue("@start_time", NpgsqlTypes.NpgsqlDbType.Timestamp, startTime);
            command.Parameters.AddWithValue("@stop_time", NpgsqlTypes.NpgsqlDbType.Timestamp, stopTime);
            command.Parameters.AddWithValue("@employeeID", NpgsqlTypes.NpgsqlDbType.Bigint, employeeId);

            
            await connection.OpenAsync();

            
            var result = await command.ExecuteScalarAsync();

            
            return result != DBNull.Value && result != null ? Convert.ToDouble(result) : 0.0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania danych: {ex.Message}");
            throw;
        }
    }

}