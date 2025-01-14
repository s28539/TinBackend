using restAPI.Models;

namespace restAPI.Repositories;

public interface IEmployeesRepository
{
    Task<bool> DoesEmployeeExist(long id);
    Task<EmployeeDto>EmployeeDetails(long id);
    Task<List<EmployeeDto>> Employees();
    Task<bool> AddEmployee(EmployeeDto newEmployeeDto);
    Task<bool> DeleteEmployee(long id);
}