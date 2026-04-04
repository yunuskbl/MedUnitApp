using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Application.Dtos;

namespace MedUnit.Application.Interfaces;

public interface IZoomService
{
    Task<ZoomMeetingResponseDto> MeetingOlusturAsync(int randevuId);
    Task MeetingSilAsync(string meetingId);
}

