using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Application.Dtos;

    public class ZoomMeetingResponseDto
    {
        public string MeetingId { get; set; } = string.Empty;
        public string JoinUrl { get; set; } = string.Empty;
        public string HostUrl { get; set; } = string.Empty;
        public DateTime BaslangicZamani { get; set; }
    }

