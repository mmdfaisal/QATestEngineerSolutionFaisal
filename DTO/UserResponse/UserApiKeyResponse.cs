using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UserApiKeyResponse
    {
        public UserApiKeyResponse()
        {
            messages = new List<string>();
        }

        public string api_key { get; set; } //I understand naming convention for class properties is to use TitleCase, 
                                            //but keeping it lower case with underscores to use the same format as in json string
        public List<string> messages { get; set; }
        public string service_key { get; set; }
        public bool success { get; set; }
        public string user_uid { get; set; }
    }
}
