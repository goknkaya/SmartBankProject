using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Switching
{
    public class SwitchLogDto
    {
        public string Stage { get; set; } = "";
        public string Level { get; set; } = "";
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PayloadIn { get; set; }
        public string? PayloadOut { get; set; }
    }
}
