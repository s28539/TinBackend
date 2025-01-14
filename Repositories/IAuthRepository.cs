using restAPI.Models;

namespace restAPI.Repositories;

public interface IAuthRepository
{
    Task<bool> AddRaportToAcceptByAdmin(RegisterDto registerDto);
    Task<bool> ActivateAccount(long id);
    Task<List<User>> GetUsers();
}