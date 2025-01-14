using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using restAPI.Models;
using restAPI.Repositories;

namespace restAPI.Controllers;

[ApiController]
[Route("api/[controller]")]


public class RaportsController : ControllerBase
{
    private readonly IRaportRepository _raportRepository;
    public RaportsController(IRaportRepository raportsRepository)
    {
        _raportRepository = raportsRepository;
    }
    
    [HttpGet("not-completed")]
    public async Task<IActionResult> GetNotCompletedRaports()
    {
        var raports = await _raportRepository.GetNotCompletedRaports();
            
        return Ok(raports);
    }
    
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedRaports()
    {
        var raports = await _raportRepository.GetCompletedRaports();
            
        return Ok(raports);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddRaport(RaportDto raportDto)
    {
        var result = await _raportRepository.AddRaport(raportDto);
            
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRaport(int id)
    {
        var result = await _raportRepository.DeleteRaport(id);
        return Ok(result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> ChangeRaport(ChangeRaportDto changeRaportDto)
    {
        var result = await _raportRepository.ChangeRaport(changeRaportDto);
        return Ok(result);
    }
    [HttpGet("raport-history")]
    public async Task<IActionResult> GetRaportHistory(int raport_id)
    {
        var result = await _raportRepository.GetHistoryOfRaport(raport_id);
        return Ok(result);
    }
}