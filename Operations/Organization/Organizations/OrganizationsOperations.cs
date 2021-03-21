using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Utilities;

namespace Operations
{
    public static class OrganizationsOperations
    {
        private static string ResrourceUrlGetApiKey => Helper.LoadFromResourceUrls("//Contrast/Organization/Organizations/ApiKey");
        public static IRestResponse GetApiKey()
        {
            IRestResponse response = APIHelper.MakeGETRequest(ResrourceUrlGetApiKey);
            return APIHelper.CheckNullResponse(response, "Could not retrieve API key.");
        }
    }
}
