using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restAPI.Models;
using restAPI.Repositories;

namespace restAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ReportsController : ControllerBase
{
    private readonly IReportsRepository _reportsRepository;
    public ReportsController(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }
    
    [HttpPost("worked-hours")]
    public async Task<IActionResult> GetWorkedHours(long employeeID,DateTime startTime, DateTime stopTime)
    {
        var result = await _reportsRepository.GetWorkedHours(employeeID,startTime,stopTime);
        return Ok(result);
    }
}