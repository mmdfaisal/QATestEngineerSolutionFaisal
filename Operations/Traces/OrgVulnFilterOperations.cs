using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using DTO;

namespace Operations
{
    public class OrgVulnFilterOperations
    {
        public static string ResourceUrlFilterOrgVulnerabilities => Helper.LoadFromResourceUrls("//Contrast/Filters/FilterOrgVulnerabilities");

        public static IRestResponse FilterOrgVulnerabilities(TraceFilterRequest traceFilterRequest)
        {
            string resourceUrl = ResourceUrlFilterOrgVulnerabilities;
            IRestResponse response = APIHelper.MakePOSTRequest(resourceUrl, traceFilterRequest);
            return APIHelper.CheckNullResponse(response, "Could not filter organization vulnerabilities");
        }
    }
}
