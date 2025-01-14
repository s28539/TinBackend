using restAPI.Models;

namespace restAPI.Repositories;

public interface IRolesRepository
{
    Task<List<Role>> GetRoles();
}