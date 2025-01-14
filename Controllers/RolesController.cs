using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restAPI.Models;
using restAPI.Repositories;

namespace restAPI.Controllers;


[ApiController]
[Route("api/[controller]")]


public class RolesController : ControllerBase
{
    private readonly IRolesRepository _roleRepository;
    public RolesController(IRolesRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    
    [HttpGet()]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleRepository.GetRoles();
        return Ok(roles);
    }
}