using Microsoft.AspNetCore.Mvc;
using Npgsql;
using restAPI.Models;
using restAPI.Repositories;


public class ShiftsRepository : IShiftsRepository
{
    private readonly IConfiguration _configuration;

    public ShiftsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<List<Shift>> GetEmployeeShifts(long id, DateTime? startDate = null, DateTime? endDate = null)
    {
        List<Shift> shifts = new List<Shift>();

        var query = "SELECT * FROM shift WHERE employee_id = @ID";

        
        if (startDate.HasValue)
        {
            query += " AND start_date >= @StartDate";
        }

        if (endDate.HasValue)
        {
            query += " AND stop_date <= @EndDate";
        }

        query = query + " ORDER BY start_date DESC";
        
        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", id);

            if (startDate.HasValue)
            {
                command.Parameters.AddWithValue("@StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                command.Parameters.AddWithValue("@EndDate", endDate.Value);
            }

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                shifts.Add(new Shift
                {
                    ShiftID = reader.GetInt64(reader.GetOrdinal("shift_id")),
                    EmployeeID = reader.GetInt64(reader.GetOrdinal("employee_id")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("start_date")),
                    StopTime = reader.IsDBNull(reader.GetOrdinal("stop_date"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("stop_date"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania zmian: {ex.Message}");
            throw;
        }

        return shifts;
    }
    public async Task<List<Shift>> GetAllShifts(long? employee_id)
    {
        List<Shift> shifts = new List<Shift>();

        var query = "SELECT * FROM shift INNER JOIN employee  on shift.employee_id = employee.employee_id";
        if (employee_id is not null)
        {
            query += " Where shift.employee_id = " + employee_id;
        }
        query += " order by start_date desc;";
        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            
            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                shifts.Add(new Shift
                {
                    ShiftID = reader.GetInt64(reader.GetOrdinal("shift_id")),
                    EmployeeID = reader.GetInt64(reader.GetOrdinal("employee_id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    surname = reader.GetString(reader.GetOrdinal("surname")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("start_date")),
                    StopTime = reader.IsDBNull(reader.GetOrdinal("stop_date"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("stop_date"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania zmian: {ex.Message}");
            throw;
        }

        return shifts;
    }
    public async Task<List<Shift>> GetTodaysShifts(long? employee_id)
    {
        List<Shift> shifts = new List<Shift>();

        var query = "SELECT *  FROM shift INNER JOIN employee ON shift.employee_id = employee.employee_id WHERE start_date::date = CURRENT_DATE ";
        if (employee_id is not null)
        {
            query +=" AND shift.employee_id = "+employee_id;
        }

        query +=" ORDER BY start_date DESC";
        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            
            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                shifts.Add(new Shift
                {
                    ShiftID = reader.GetInt64(reader.GetOrdinal("shift_id")),
                    EmployeeID = reader.GetInt64(reader.GetOrdinal("employee_id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    surname = reader.GetString(reader.GetOrdinal("surname")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("start_date")),
                    StopTime = reader.IsDBNull(reader.GetOrdinal("stop_date"))
                        ? (DateTime?)null
                        : reader.GetDateTime(reader.GetOrdinal("stop_date"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania zmian: {ex.Message}");
            throw;
        }

        return shifts;
    }
    public async Task<bool> AddShift(AddShiftDto addShiftDto)
    {
        
        var query = "INSERT INTO shift (employee_id, start_date, stop_date) VALUES (@EmployeeId, @StartDate, @StopDate)";

        try
        {
            
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            
            command.Parameters.AddWithValue("@EmployeeId", addShiftDto.Employee_id);
            command.Parameters.AddWithValue("@StartDate", addShiftDto.StartDate);
            command.Parameters.AddWithValue("@StopDate", addShiftDto.StopDate);

           
            await connection.OpenAsync();

            
            int rowsAffected = await command.ExecuteNonQueryAsync();

            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Błąd podczas dodawania zmiany: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> ChangeShift(ChangeShiftDto changeShiftDto)
    {
        
        var query = "UPDATE shift SET start_date = @StartDate, stop_date = @StopDate WHERE shift_id = @ShiftID";

        try
        {
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new NpgsqlCommand(query, connection);

            
            command.Parameters.AddWithValue("@StartDate", changeShiftDto.StartTime);
            command.Parameters.AddWithValue("@StopDate", changeShiftDto.StopTime);
            command.Parameters.AddWithValue("@ShiftID", changeShiftDto.ShiftID);

            
            await connection.OpenAsync();

           
            var result = await command.ExecuteNonQueryAsync();

            
            return result > 0;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Błąd podczas zmiany zmiany: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> DeleteShift(long id)
    {
        
        var query = "DELETE FROM shift WHERE shift_id = @ShiftID";

        try
        {
            
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new NpgsqlCommand(query, connection);

            
            command.Parameters.AddWithValue("@ShiftID", id);

            
            await connection.OpenAsync();

            
            var result = await command.ExecuteNonQueryAsync();

            
            return result > 0;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Błąd podczas usuwania zmiany: {ex.Message}");
            throw;
        }
    }


}
