using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TraceFilterRequest
    {
        public TraceFilterRequest()
        {
            filterTags = new List<string>();
        }
        public List<string> filterTags { get; set; } //Only using this property for this project
    }
}
