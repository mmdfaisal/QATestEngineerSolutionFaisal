using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TraceFilterResponse
    {
        public TraceFilterResponse()
        {
            traces = new List<TraceResource>();
        }
        public int count { get; set; }
        public int licensedCount { get; set; }
        public List<object> links { get; set; }
        public List<object> messages { get; set; }
        public bool success { get; set; }
        public List<TraceResource> traces { get; set; }
    }
}
