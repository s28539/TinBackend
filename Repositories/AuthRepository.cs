using Npgsql;
using restAPI.Models;

namespace restAPI.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IConfiguration _configuration;
    public AuthRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> AddRaportToAcceptByAdmin(RegisterDto registerDto)
{
    EmployeesRepository employeesRepository = new EmployeesRepository(_configuration);
    RaportRepository raportRepository = new RaportRepository(_configuration);

    if (!await employeesRepository.DoesEmployeeExist(registerDto.CardID)) return false;
    EmployeeDto employee = await employeesRepository.EmployeeDetails(registerDto.CardID);
    var ID = await raportRepository.GetMaxIDRaport();
    ID += 1;

    string description = $"[new-user] !{registerDto.Role}! ( {registerDto.CardID} ) {employee.Name} {employee.Surname}";

    var query1 = "INSERT INTO raport (id, description, status) VALUES (@ID, @Description, 'NOWE')";
    var query2 = "INSERT INTO raporthistory (timestamp, employee_id, raport_id) VALUES (@Timestamp, @EmployeeIDPoster, @ID)";
    var query3 = "UPDATE employee SET role_id = @RoleID, email = @Email, password=@Password, online_active = false WHERE employee_id = @CardID";

    await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync(); 
    try
    {
        
        await using var command1 = new NpgsqlCommand(query1, connection, transaction);
        command1.Parameters.AddWithValue("@ID", ID);
        command1.Parameters.AddWithValue("@Description", description);
        await command1.ExecuteNonQueryAsync();

        
        await using var command2 = new NpgsqlCommand(query2, connection, transaction);
        command2.Parameters.AddWithValue("@Timestamp", DateTime.Now);
        command2.Parameters.AddWithValue("@EmployeeIDPoster", registerDto.CardID);
        command2.Parameters.AddWithValue("@ID", ID);
        await command2.ExecuteNonQueryAsync();

        
        await using var command3 = new NpgsqlCommand(query3, connection, transaction);
        command3.Parameters.AddWithValue("@RoleID", registerDto.RoleID);
        command3.Parameters.AddWithValue("@Email", registerDto.Email);
        command3.Parameters.AddWithValue("@CardID", registerDto.CardID);
        command3.Parameters.AddWithValue("@Password", registerDto.Password);
        await command3.ExecuteNonQueryAsync();

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

    public async Task<bool> ActivateAccount(long id)
    {
        var query = "UPDATE employee SET online_active = true WHERE employee_id = @EmployeeID";
        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();

            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmployeeID", id); 

           
            int rowsAffected = await command.ExecuteNonQueryAsync();

            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    public async Task<List<User>> GetUsers()
    {
        List<User> users = new List<User>();
        var query = "SELECT employee_id, password, role.description FROM employee INNER JOIN role ON role.id = employee.role_id WHERE online_active = true;";

        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();
            
            await using var command = new NpgsqlCommand(query, connection);
            
            await using var reader = await command.ExecuteReaderAsync();

           
            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt64(0),
                    Password = reader.GetString(1), 
                    Role = reader.GetString(2) 
                };
                users.Add(user);
            }

            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return users; 
        }
    }

}