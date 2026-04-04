using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedUnit.Application.Interfaces;

namespace RandevuAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ZoomController : ControllerBase
{
    private readonly IZoomService _zoomService;

    public ZoomController(IZoomService zoomService)
    {
        _zoomService = zoomService;
    }

    [HttpPost("meeting-olustur/{randevuId}")]
    [Authorize(Roles = "doktor,admin")]
    public async Task<IActionResult> MeetingOlustur(int randevuId)
    {
        try
        {
            var result = await _zoomService.MeetingOlusturAsync(randevuId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("meeting-sil/{meetingId}")]
    [Authorize(Roles = "doktor,admin")]
    public async Task<IActionResult> MeetingSil(string meetingId)
    {
        try
        {
            await _zoomService.MeetingSilAsync(meetingId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("meeting-bilgisi/{randevuId}")]
    public async Task<IActionResult> MeetingBilgisi(int randevuId)
    {
        try
        {
            var result = await _zoomService.MeetingOlusturAsync(randevuId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}