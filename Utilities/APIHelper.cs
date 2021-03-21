using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class APIHelper
    {
        //NLog logger object. See NLog.config in the Utilities project for details.
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        //Read-only, as this should not be changed anywhere in the code
        public static Uri BaseUri => new Uri(Helper.LoadFromContrastConfig("//Contrast/BaseUrl"));
                
        //The following three properties are only used inside the AddRequestHeaders() method
        //So I would declare them inside that method as local variables
        //But I am using ApiKey in TC001 for demo purposes, so leaving them here 
        //Otherwise, I would put them inside the AddRequestHeaders() method
        //Or have them here, but make them all private 
        public static string ApiKey => Helper.LoadFromContrastConfig("//Contrast/ApiKey");
        private static string UserName => Helper.LoadFromContrastConfig("//Contrast/UserName");
        private static string ServiceKey => Helper.LoadFromContrastConfig("//Contrast/ServiceKey");

        private static int TimeoutInitialValue
        {
            get
            {
                if (!Int32.TryParse(Helper.LoadFromRetryConfig("//RetryConfig/TimeoutInitialValue"), out int num))
                {
                    throw new ArgumentException($"The TimeoutInitialValue specified is not a valid integer");
                }
                return num * 1000;
            }
        }
        private static int TimeoutRetryCount
        {
            get
            {
                if (!Int32.TryParse(Helper.LoadFromRetryConfig("//RetryConfig/TimeoutRetryCount"), out int num))
                {
                    throw new ArgumentException($"The TimeoutRetryCount specified is not a valid integer");
                }
                return num;
            }
        }
        private static int TimeoutIncrementTime
        {
            get
            {
                if (!Int32.TryParse(Helper.LoadFromRetryConfig("//ApiConfig/TimeoutIncrementTime"), out int num))
                {
                    throw new ArgumentException($"The TimeoutIncrementTime specified is not a valid integer");
                }
                return num * 1000;
            }
        }
        //This is private since used only inside this class
        private static RestRequest AddRequestHeaders(RestRequest request)
        {
            request.AddHeader("API-Key", ApiKey);
            request.AddHeader("Authorization", Helper.GetAuthorizationString($"{UserName}:{ServiceKey}"));
            request.AddHeader("Accept", "application/json"); 
            return request;
        }
        public static IRestResponse MakeGETRequest(string resourceUrl)
        {
            try
            {
                RestClient client = new RestClient(BaseUri);
                RestRequest request = new RestRequest(resourceUrl, Method.GET);
                request = AddRequestHeaders(request);
                Logger.Info($"Making GET Request at {client.BaseUrl}{request.Resource}");
                IRestResponse response = ApiCallWithRetry(client, request, () => { return client.ExecuteAsync<RestResponse>(request).GetAwaiter().GetResult(); });
                Logger.Info($"Response Status: {response.StatusCode}\nResponse Body:\n{response.Content}");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
        public static IRestResponse MakeDELETERequest(string resourceUrl, object customObject)
        {
            try
            {
                RestClient client = new RestClient(BaseUri);
                RestRequest request = new RestRequest(resourceUrl, Method.DELETE);
                request = AddRequestHeaders(request);
                request.AddJsonBody(customObject); //This method can take in either json string or a custom object
                                                   //In case of custom object, it automatically serializes it
                Logger.Info($"Making DELETE Request at {client.BaseUrl}{request.Resource}\nRequest Body:\n{JsonConvert.SerializeObject(customObject)}");
                IRestResponse response = ApiCallWithRetry(client, request, () => { return client.ExecuteAsync<RestResponse>(request).GetAwaiter().GetResult(); });
                Logger.Info($"Response Status: {response.StatusCode}\nResponse Body:\n{response.Content}");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public static IRestResponse MakePOSTRequest(string resourceUrl, object customObject)
        {
            try
            {
                RestClient client = new RestClient(BaseUri);
                RestRequest request = new RestRequest(resourceUrl, Method.POST);
                request = AddRequestHeaders(request);
                request.AddJsonBody(customObject); 
                Logger.Info($"Making POST Request at {client.BaseUrl}{request.Resource}\nRequest Body:\n{JsonConvert.SerializeObject(customObject)}");
                IRestResponse response = ApiCallWithRetry(client, request, () => { return client.ExecuteAsync<RestResponse>(request).GetAwaiter().GetResult(); });
                Logger.Info($"Response Status: {response.StatusCode}\nResponse Body:\n{response.Content}");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        //I understannd MakeDELETERequest(), MakePUTRequest() and MakePOSTRequest()can be combined into one method
        //Perhaps by using an enum with values like DELETE, PUT, POST and using swtich case to use Method.DELETE or Method.PUT or Method.POST
        //But I have left it this way for readability/clarity, for the callers to just use same style, consistent, method names themselves which are descriptive (and not pass in an enum)
        //Also I'd also in future need to write methods like MakeDELETERequestUsingQueryString(resourceUrl, queryStringKey, queryStringValuesList), MakePOSTRequest<List<T>>() etc.
        //So I think it is better to leave the methods as is
        //I'd be happy to learn other techniques which can better address scenarios like this in an elegant manner
        public static IRestResponse MakePUTRequest(string resourceUrl, object customObject)
        {
            try
            {
                RestClient client = new RestClient(BaseUri);
                RestRequest request = new RestRequest(resourceUrl, Method.PUT);
                request = AddRequestHeaders(request);
                request.AddJsonBody(customObject); 
                Logger.Info($"Making PUT Request at {client.BaseUrl}{request.Resource}\nRequest Body:\n{JsonConvert.SerializeObject(customObject)}");
                IRestResponse response = ApiCallWithRetry(client, request, () => { return client.ExecuteAsync<RestResponse>(request).GetAwaiter().GetResult(); });
                Logger.Info($"Response Status: {response.StatusCode}\nResponse Body:\n{response.Content}");
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        //Some of the APIs I have worked with take more than 100 seconds to respond (depending on the amount of data being retrieved from SQL Server)
        //RestSharp's RestClient object's default timeout is 100 seconds
        //I understand same is the case with .NET's native HttpClient object
        //This method retries API calls if there is timeout, increasing the timeout every time, based on values in RetryConfig.xml in Utilities project
        private static IRestResponse ApiCallWithRetry(IRestClient client, IRestRequest request, Func<IRestResponse> executeRequest) //Delegate func executeRequest takes
        {                                                                                                                           //zero input parms and returns one parm    
            try
            {
                IRestResponse response;
                int timeoutValue = TimeoutInitialValue;
                for (int attempts = 1; attempts <= TimeoutRetryCount; attempts++)
                {
                    client.Timeout = timeoutValue;
                    response = executeRequest();
                    if (!response.IsSuccessful)
                    {
                        if (response.ResponseStatus == ResponseStatus.TimedOut) //This is the reason for using client.ExecuteAsync<RestResponse>(request).GetAwaiter().GetResult() 
                        {                                                       //For regular client.Execute<RestResponse>(request), in case of timeout, response.ResponseStatus was just Error
                                                                                //Which could mean any error, and not specific timeout situation
                            Logger.Warn($"Response timed out. Attempt {attempts} of {TimeoutRetryCount}");
                            if (attempts == TimeoutRetryCount)
                            {
                                Logger.Error($"Response timed out maximum ({TimeoutRetryCount}) number of times");
                            }
                            timeoutValue += TimeoutIncrementTime;
                            continue;
                        }
                    }
                    return response; //The code will reach here if:
                                     // - response is successful
                                     // - response is not successful, but it has NOT timed out
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null; //If the response times out max number of times, the execution flow will reach here,
                         //or if there is an exception in this method
        }

        //This method is being called from the methods in the Operations class, which sits one level below the unit test classes
        public static IRestResponse CheckNullResponse(IRestResponse response, string exceptionMessage)
        {
            try
            {
                if (response == null)
                {
                    throw new Exception($"{exceptionMessage}. Because the response object is null.");
                }
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw; //So that it bubbles up to the test method. 
                       //If we absorb the exception here, the test result wil have object reference not set to an instance of an object
                       //Which does not provide any useful info
            }
        }
    }
}
