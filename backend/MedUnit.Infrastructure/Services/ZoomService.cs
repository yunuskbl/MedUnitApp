using System.Net.Http.Headers;
using System.Text;
using MedUnit.Application.Dtos;
using MedUnit.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedUnit.Infrastructure.Services;

public class ZoomService : IZoomService
{
    private readonly IConfiguration _config;
    private readonly IAppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ZoomService(IConfiguration config, IAppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<string> AccessTokenAlAsync()
    {
        var client = _httpClientFactory.CreateClient();

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            $"{_config["Zoom:ClientId"]}:{_config["Zoom:ClientSecret"]}"));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        var response = await client.PostAsync(
            $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={_config["Zoom:AccountId"]}",
            null);

        var json = await response.Content.ReadAsStringAsync();
        var data = JObject.Parse(json);

        return data["access_token"]!.ToString();
    }

    public async Task<ZoomMeetingResponseDto> MeetingOlusturAsync(int randevuId)
    {
        var randevu = await _context.Randevular
            .Include(r => r.Hasta)
            .Include(r => r.Doktor)
            .FirstOrDefaultAsync(r => r.Id == randevuId)
            ?? throw new Exception("Randevu bulunamadı.");

        if (randevu.Durum == "iptal")
            throw new Exception("İptal edilmiş randevu için görüşme başlatılamaz.");

        if (randevu.Durum != "onaylandi")
            throw new Exception("Sadece onaylı randevular için görüşme başlatılabilir.");

        var token = await AccessTokenAlAsync();
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var meetingData = new
        {
            topic = $"Dr. {randevu.Doktor.Ad} {randevu.Doktor.Soyad} — {randevu.Hasta.Ad} {randevu.Hasta.Soyad}",
            type = 2,
            start_time = randevu.BaslangicTarihi.ToString("yyyy-MM-ddTHH:mm:ss"),
            duration = (int)(randevu.BitisTarihi - randevu.BaslangicTarihi).TotalMinutes,
            timezone = "Europe/Istanbul",
            settings = new
            {
                waiting_room = true,
                join_before_host = false,
                auto_recording = "none"
            }
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(meetingData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);
        var json = await response.Content.ReadAsStringAsync();
        var data = JObject.Parse(json);

        randevu.ZoomMeetingId = data["id"]!.ToString();
        randevu.ZoomJoinUrl = data["join_url"]!.ToString();
        randevu.ZoomHostUrl = data["start_url"]!.ToString();
        await _context.SaveChangesAsync();

        return new ZoomMeetingResponseDto
        {
            MeetingId = randevu.ZoomMeetingId,
            JoinUrl = randevu.ZoomJoinUrl,
            HostUrl = randevu.ZoomHostUrl,
            BaslangicZamani = randevu.BaslangicTarihi
        };
    }

    public async Task MeetingSilAsync(string meetingId)
    {
        var token = await AccessTokenAlAsync();
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await client.DeleteAsync($"https://api.zoom.us/v2/meetings/{meetingId}");
    }
}