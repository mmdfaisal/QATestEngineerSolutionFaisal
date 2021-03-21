using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TagsResponse
    {
        public TagsResponse()
        {
            errors = new List<object>();
            messages = new List<string>();
            tags = new List<string>();
        }
        public List<object> errors { get; set; }
        public List<string> messages { get; set; }
        public bool success { get; set; }
        public List<string> tags { get; set; }
        public int totalLibraryHashes { get; set; }
    }

}
