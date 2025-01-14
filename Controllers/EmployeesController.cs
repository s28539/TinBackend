using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restAPI.Models;
using restAPI.Repositories;

namespace restAPI.Controllers;


[ApiController]
[Route("api/[controller]")]


public class EmployeesController : ControllerBase
{
    private readonly IEmployeesRepository _employeesRepository;
    public EmployeesController(IEmployeesRepository employeesRepository)
    {
        _employeesRepository = employeesRepository;
    }
    
    [HttpGet("{id}/")]
    public async Task<IActionResult> GetEmployee(long id)
    {
        if (!await _employeesRepository.DoesEmployeeExist(id))
            return NotFound($"Employee with given ID - {id} doesn't exist");

        var employeeDetails = await _employeesRepository.EmployeeDetails(id);
            
        return Ok(employeeDetails);
    }
    [HttpGet()]
    public async Task<IActionResult> GetEmployee()
    {
        var employees = await _employeesRepository.Employees();
            
        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto newEmployee)
    {
        if (newEmployee == null || string.IsNullOrWhiteSpace(newEmployee.Name) || string.IsNullOrWhiteSpace(newEmployee.Surname))
        {
            return BadRequest("Invalid employee data. Name and Surname are required.");
        }

        try
        {
            bool isAdded = await _employeesRepository.AddEmployee(newEmployee);

            if (isAdded)
            {
                return CreatedAtAction(nameof(GetEmployee), new { id = newEmployee.EmployeeID }, newEmployee);
            }
            else
            {
                return StatusCode(500, "An error occurred while adding the employee.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while adding employee: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteEmployee(long id)
    {
        var result = await _employeesRepository.DeleteEmployee(id);
        return Ok(result);
    }

}