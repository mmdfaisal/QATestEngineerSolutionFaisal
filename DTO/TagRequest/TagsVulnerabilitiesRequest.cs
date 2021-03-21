using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TagsVulnerabilitiesRequest
    {
        public TagsVulnerabilitiesRequest()
        {
            tags = new List<string>();
            traces_id = new List<string>();
        }
        public List<string> tags { get; set; }
        public List<string> traces_id { get; set; }
    }
}
