using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraReader.Contracts
{
    public class CameraDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public bool Enabled { get; set; }
        public bool AnalyticsEnabled { get; set; }
    }
}
