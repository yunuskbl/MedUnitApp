using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedUnit.Domain.Interfaces;

    public interface IZoomRepository
    {
        Task<object> MeetingOlusturAsync(int randevuId);
        Task MeetingSilAsync(string meetingId);
    }

