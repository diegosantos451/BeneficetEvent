using Microsoft.AspNetCore.Mvc;
using BeneficentEvent.Services;
using Microsoft.AspNetCore.Authorization;

namespace BeneficentEvent.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    public DashboardController(DashboardService dashboardService)
    {   
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task <ActionResult> ObterDashBoard()
    {
        return Ok(await _dashboardService.ObterDashboardAsync());
    }
}