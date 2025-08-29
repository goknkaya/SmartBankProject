using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Chargeback
{
    public class DecideChargebackDto
    {
        public string Decision { get; set; } = "Won"; // Won|Lost|Cancelled
        public string? Note { get; set; }
    }
}
