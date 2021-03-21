using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RestSharp;
using Operations;
using DTO;
using System.Net;
using Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace UnitTestsAPI
{
    [TestClass]
    public class UnitTestsAPI
    {
        //Pre-Conditions/Assumptions for all test methods:
        //(1). Knowledge of various user config parms. Stored in ContrastConfig.xml file in the Utilities project 
        //(2). Knowledge of OrgUuid. Stored as a variable in ResourceUrls.xml file in the Utilities project 
        //(3). Knowledge of TraceUuid - in the input json files for test methods, in the TestDataUnitTestsAPI folder.

        private UserApiKeyResponse userApiKeyResponse;
        private TagRequest tagRequest;
        private TagsVulnerabilitiesRequest tagsVulnerabilitiesRequest;
        private TagsResponse tagsResponse;
        private TraceFilterRequest traceFilterRequest;
        private TraceFilterResponse traceFilterResponse;

        [TestInitialize]
        public void Setup()
        {
            userApiKeyResponse = new UserApiKeyResponse(); //Reason for this approach is that if these objects are used in multiple test methods
                                                           //then they don't have to be initialized in each test method 
                                                           //Also if this test class was about, say CRUD operations for any certain type of objects/resources
                                                           //then I would also use [TestCleanup] to pass in this same objects to Delete() operations to clean up the test env.
            tagRequest = new TagRequest();                 
            tagsVulnerabilitiesRequest = new TagsVulnerabilitiesRequest();
            tagsResponse = new TagsResponse();
            traceFilterRequest = new TraceFilterRequest();
            traceFilterResponse = new TraceFilterResponse();
        }

        [TestMethod]
        [Description("Makes GET request to retrieve the api key" +
                    "Asserts that Status Code is OK(200)" +
                    "Asserts that the retrieved api key is correct (assuming we know what the correct key is)." +
                    "In this case, we are validating against the API Key obtained from the UI(which can be automated through Selenium) and stored in ContrastConfig.xml")]
        public void TC001()
        {
            IRestResponse response = OrganizationsOperations.GetApiKey();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The API Key failed to get retrieved");

            userApiKeyResponse = JsonConvert.DeserializeObject<UserApiKeyResponse>(response.Content);
            Assert.AreEqual(userApiKeyResponse.api_key, APIHelper.ApiKey, "The retrieved API Key is not correct");
        }

        [TestMethod]
        [Description("Pre-Condition/Assumption: We know the vulnerability UUID to be 'PSQM-Q3BR-OSCI-GDA0'" +
                    "Tags a vulnerability (adds two tags)" +
                    "Asserts status code is OK(200)" +
                    "Gets list of all tags for the specific vulnerability" +
                    "Validates the above added tags are in the above list")]
        public void TC002()
        {
            tagsVulnerabilitiesRequest = Helper.DeserializeJsonFiletoObject<TagsVulnerabilitiesRequest>(@".\TestDataUnitTestsAPI\TC002.json");
            IRestResponse response = TagsOperations.AddTagsToVulnerabilities(tagsVulnerabilitiesRequest);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The tag(s) failed to get tagged to vulnerability(s)");

            response = TagsOperations.GetTagsforVulnerability(tagsVulnerabilitiesRequest.traces_id[0]); //Using just one vuln in this test. If there were more, we'd loop through the list
                                                                                                        //Get the response for each, and do the validation below for each    
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Could not retrieve the list of all tags for the specific vulnerability");

            tagsResponse = JsonConvert.DeserializeObject<TagsResponse>(response.Content);
            //Validate that all the newly added tags are part of the list of tags for the specific vulnerability
            bool isFound = Helper.IsListSubsetOfAnotherList(tagsVulnerabilitiesRequest.tags, tagsResponse.tags);
            Assert.IsTrue(isFound, "One or more tag(s) did not get successfully tagged to vulnerability(s)");
        }

        [TestMethod]
        [Description("Pre-Condition/Assumption: We know 2 trace UUIDs which we read from input json" +
                    "Add two tags each to the above two vulnerabilities" +
                    "Asserts status code is OK(200)" +
                    "Uses the above tags as filter tags to filter organization vulnerabilities" +
                    "Validates that the results include both of the above vulns")]
        public void TC003()
        {
            tagsVulnerabilitiesRequest = Helper.DeserializeJsonFiletoObject<TagsVulnerabilitiesRequest>(@".\TestDataUnitTestsAPI\TC003.json");
            IRestResponse response = TagsOperations.AddTagsToVulnerabilities(tagsVulnerabilitiesRequest);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The tag(s) failed to get tagged to vulnerability(s)");

            traceFilterRequest.filterTags = tagsVulnerabilitiesRequest.tags; //Using the vulnerability tags we added earlier as the filter tags

            response = OrgVulnFilterOperations.FilterOrgVulnerabilities(traceFilterRequest);
            //Note: For the above request, the status code is OK even when all tags in traceFilterRequest.filterTags are non-existant
            //So we are NOT validating for status code OK
            //We will also NOT validate for success property to be true
            //Because there could be other vulnerabilities with the same tags, and so the success will be true even when *only* those other vulns are in response
            
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            traceFilterResponse = JsonConvert.DeserializeObject<TraceFilterResponse>(response.Content, settings);

            //We will validate (by UUIDs) that the above result includes the specific vulnerabilites which were tagged in this test earlier

            //Collecting all the UUIDs which were received in the response
            List<string> filteredResultsTraceUuidList = new List<string>();
            foreach (var item in traceFilterResponse.traces)
            {
                filteredResultsTraceUuidList.Add(item.uuid);
            }

            //Validating that all the trace UUUIDs which were sent in the tag request are part of the above filteredResultsTraceUuidList
            bool allFound = Helper.IsListSubsetOfAnotherList(tagsVulnerabilitiesRequest.traces_id, filteredResultsTraceUuidList);
            Assert.IsTrue(allFound, "The filtered results (by tags) did not include one or more vulnerability whose tags were specified in the filter");
        }

        [TestMethod]
        [Description("Pre-Condition/Assumption: We know the vulnerability UUID to be 'PSQM-Q3BR-OSCI-GDA0'" +
                     "Makes Delete request to delete a non-existant tag from the above vulnerability" +
                     "Asserts that the messages array in response does NOT contain the string {Tag deleted successfully}")]
        //Note: The following test fails (as of latest build 03/20/21) - the message 'Tag deleted successfully' is received
        public void TC004()
        {
            string traceUuid = "PSQM-Q3BR-OSCI-GDA0";
            tagRequest.tag = Guid.NewGuid().ToString(); //GUID collision possibility is extremely small, but not zero.
                                                        //You can delete existing tags from a vulnerability one by one (or if there is a bulk delete option)
                                                        //Until you can assert that the tags array is empty
            IRestResponse response = TagsOperations.DeleteTagFromVulnerability(tagRequest, traceUuid);
            tagsResponse = JsonConvert.DeserializeObject<TagsResponse>(response.Content);

            List<string> successfulDeleteMessageList = new List<string>();
            successfulDeleteMessageList.Add("Tag deleted successfully");

            //Validating that the message {Tag deleted successfully} is NOT part of the messages list
            //We expect isFound to be false
            bool isFound = Helper.IsListSubsetOfAnotherList(successfulDeleteMessageList, tagsResponse.messages);
            Assert.IsFalse(isFound, "Successful delete message received (incorrectly)"); 
        }

    }
}
