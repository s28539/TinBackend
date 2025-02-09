using restAPI.Models;

namespace restAPI.Repositories;

public interface IAuthRepository
{
    Task<string> AddRaportToAcceptByAdmin(RegisterDto registerDto);
    Task<bool> ActivateAccount(long id);
    Task<List<User>> GetUsers();
}