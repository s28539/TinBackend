using Npgsql;
using restAPI.Models;

namespace restAPI.Repositories;

public class RaportRepository : IRaportRepository
{
    private readonly IConfiguration _configuration;
    public RaportRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<List<Raport>> GetNotCompletedRaports()
    {
        List<Raport> raports = new List<Raport>();
        var query = "SELECT * FROM raport where status like 'NOWE' order by id desc";

        await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using NpgsqlCommand command = new NpgsqlCommand();

        command.Connection = connection;
        command.CommandText = query;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            raports.Add(new Raport()
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),  
                Description = reader.GetString(reader.GetOrdinal("description")),
                Status = reader.GetString(reader.GetOrdinal("status"))
            });
        }
        return raports;
    }
    public async Task<List<Raport>> GetCompletedRaports()
    {
        List<Raport> raports = new List<Raport>();
        var query = "SELECT * FROM raport where status not like 'NOWE' order by id desc";

        await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using NpgsqlCommand command = new NpgsqlCommand();

        command.Connection = connection;
        command.CommandText = query;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            raports.Add(new Raport()
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),  
                Description = reader.GetString(reader.GetOrdinal("description")),
                Status = reader.GetString(reader.GetOrdinal("status"))
            });
        }
        return raports;
    }
    public async Task<int> GetMaxIDRaport()
    {
        var query = "select max(raport.id) from raport";
        await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
    
        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        
        return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
    }
   public async Task<bool> AddRaport(RaportDto raportDto)
{
    string description = $"[{raportDto.Action}] ( {raportDto.Employee_id} ) {raportDto.Employee_name} {raportDto.Employee_surname} ";
    if (raportDto.Shift_id is not null)
    {
        description += " ID ZMIANY: " + raportDto.Shift_id;
    }

    if (raportDto.StartDate is not null && raportDto.StopDate is not null)
    {
        description += " ( " + raportDto.StartDate + " - " + raportDto.StopDate + " )";
    }

    var query1 = "INSERT INTO raport (description, status) VALUES (@Description, 'NOWE') RETURNING id";
    var query2 = "INSERT INTO raporthistory (timestamp, employee_id, raport_id) VALUES (@Timestamp, @EmployeeIDPoster, @RaportID)";

    await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync(); 
    try
    {
       
        await using var command1 = new NpgsqlCommand(query1, connection, transaction);
        command1.Parameters.AddWithValue("@Description", description);

       
        var raportID = Convert.ToInt64(await command1.ExecuteScalarAsync()); 
        Console.WriteLine(raportID);

        
        await using var command2 = new NpgsqlCommand(query2, connection, transaction);
        command2.Parameters.AddWithValue("@Timestamp", DateTime.Now);
        command2.Parameters.AddWithValue("@EmployeeIDPoster", raportDto.Employee_id_poster);
        command2.Parameters.AddWithValue("@RaportID", raportID); 
        await command2.ExecuteNonQueryAsync();

        await transaction.CommitAsync(); 
        return true;
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.WriteLine($"An error occurred: {ex.Message}");
        return false;
    }
}
    public async Task<bool> DeleteRaport(int id)
    {
        
        var deleteHistoryQuery = "DELETE FROM raporthistory WHERE raport_id = @ID";
        var deleteRaportQuery = "DELETE FROM raport WHERE id = @ID";

        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync(); // Rozpoczęcie transakcji
        try
        {
            
            await using var command1 = new NpgsqlCommand(deleteHistoryQuery, connection, transaction);
            command1.Parameters.AddWithValue("@ID", id);
            await command1.ExecuteNonQueryAsync();

            
            await using var command2 = new NpgsqlCommand(deleteRaportQuery, connection, transaction);
            command2.Parameters.AddWithValue("@ID", id);
            await command2.ExecuteNonQueryAsync();

            await transaction.CommitAsync(); 
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); 
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> ChangeRaport(ChangeRaportDto changeRaportDto)
    {
        
        var insertHistoryQuery = @"INSERT INTO raporthistory (timestamp, employee_id, raport_id) VALUES (@Timestamp, @EmployeeId, @RaportId)";
        var updateRaportQuery = @"UPDATE raport SET status = @NewStatus WHERE id = @RaportId";

        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync(); 
        try
        {
            
            await using var command2 = new NpgsqlCommand(updateRaportQuery, connection, transaction);
            command2.Parameters.AddWithValue("@NewStatus", changeRaportDto.NewStatus);
            command2.Parameters.AddWithValue("@RaportId", changeRaportDto.RaportID);
            await command2.ExecuteNonQueryAsync();
            
            
            await using var command1 = new NpgsqlCommand(insertHistoryQuery, connection, transaction);
            command1.Parameters.AddWithValue("@Timestamp", DateTime.Now);
            command1.Parameters.AddWithValue("@EmployeeId", changeRaportDto.EmployeeIDPoster);
            command1.Parameters.AddWithValue("@RaportId", changeRaportDto.RaportID);
            await command1.ExecuteNonQueryAsync();

            await transaction.CommitAsync(); 
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); 
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            return false;
        }
    }

    public async Task<List<DateTime>> GetHistoryOfRaport(int raport_id)
    {
        
        var query = @"SELECT timestamp FROM raporthistory WHERE raport_id = @RaportID";

        await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();

        try
        {
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@RaportID", raport_id);

            var timestamps = new List<DateTime>();

            
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                timestamps.Add(reader.GetDateTime(0)); 
            }

            return timestamps; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            return new List<DateTime>(); 
        }
    }


}