using Npgsql;
using restAPI.Models;

namespace restAPI.Repositories;

public class RolesRepository :IRolesRepository
{
    private readonly IConfiguration _configuration;

    public RolesRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<List<Role>> GetRoles()
    {
        List<Role> roles = new List<Role>();

        var query = "SELECT * FROM role";

        await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("Default"));
        await using NpgsqlCommand command = new NpgsqlCommand(query, connection);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            roles.Add(new Role
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),  
                Description = reader.GetString(reader.GetOrdinal("description")),
            });
        }

        return roles;
    }

}