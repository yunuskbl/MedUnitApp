using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedUnit.Domain.Entities;

namespace MedUnit.Application.Interfaces;

public interface IEmailService
{
    Task GonderAsync(string alici, string konu, string icerik);
}

