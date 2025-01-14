using Microsoft.AspNetCore.Mvc;
using restAPI.Models;
using restAPI.Repositories;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftsRepository _shiftsRepository;
    public ShiftsController(IShiftsRepository shiftsRepository)
    {
        _shiftsRepository = shiftsRepository;
    }
    
    [HttpGet("{employee_id}")]
    public async Task<IActionResult> GetEmployeeShifts(
        long employee_id, 
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        var shifts = await _shiftsRepository.GetEmployeeShifts(employee_id, startDate, endDate);
        return Ok(shifts);
    } 
    [HttpGet()]
    public async Task<IActionResult> GetAllShifts(long? employee_id)
    {
        var shifts = await _shiftsRepository.GetAllShifts(employee_id);
        return Ok(shifts);
    }
    [HttpGet("todayShifts")]
    public async Task<IActionResult> GetTodayShifts(long? employee_id)
    {
        var shifts = await _shiftsRepository.GetTodaysShifts(employee_id);
        return Ok(shifts);
    }

    [HttpPut]
    public async Task<IActionResult> AddShift(AddShiftDto addShiftDto)
    {
        var result = await _shiftsRepository.AddShift(addShiftDto);
        return Ok(result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> ChangeShift(ChangeShiftDto changeShiftDto)
    {
        var result = await _shiftsRepository.ChangeShift(changeShiftDto);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteShift(long id)
    {
        var result = await _shiftsRepository.DeleteShift(id);
        return Ok(result);
    }

}