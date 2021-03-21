using RestSharp;
using DTO;
using Utilities;


namespace Operations
{
    public class TagsOperations
    {
        private static string ResourceUrlDelTagFromVulnarability => Helper.LoadFromResourceUrls("//Contrast/Tags/DelTagFromVulnerability");
        private static string ResourceUrlAddTagToVulnerability => Helper.LoadFromResourceUrls("//Contrast/Tags/AddTagsToVulnerabilities");
        private static string ResourceUrlGetTagsForVulnerability => Helper.LoadFromResourceUrls("//Contrast/Tags/GetTagsForVulnerability");

        public static IRestResponse DeleteTagFromVulnerability (TagRequest tagRequest, string traceUuid)
        {
            string resourceUrl = ResourceUrlDelTagFromVulnarability.Replace("{traceUuid}", traceUuid);
            IRestResponse response = APIHelper.MakeDELETERequest(resourceUrl, tagRequest);
            return APIHelper.CheckNullResponse(response, "Could not delete tag from vulnerability");
        }

        public static IRestResponse AddTagsToVulnerabilities(TagsVulnerabilitiesRequest tagsVulnerabilitiesRequest)
        {
            string resourceUrl = ResourceUrlAddTagToVulnerability;
            IRestResponse response = APIHelper.MakePUTRequest(resourceUrl, tagsVulnerabilitiesRequest);
            return APIHelper.CheckNullResponse(response, "Could not add tags to vulnerabilities");
        }

        //Retrieves all tags for a specific vulnerability
        public static IRestResponse GetTagsforVulnerability(string traceUuid)
        {
            string resourceUrl = ResourceUrlGetTagsForVulnerability.Replace("{traceUuid}", traceUuid);
            IRestResponse response = APIHelper.MakeGETRequest(resourceUrl);
            return APIHelper.CheckNullResponse(response, "Could not retrieve tags for vulnerability");
        }
    }
}
