using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TraceResource
    {
        public List<object> app_version_tags { get; set; }
        public Application application { get; set; }
        public int auto_remediated_expiration_period { get; set; }
        public List<object> bugtracker_tickets { get; set; }
        public string category { get; set; }
        public string category_label { get; set; }
        public long closed_time { get; set; }
        public string confidence { get; set; }
        public string confidence_label { get; set; }
        public string default_severity { get; set; }
        public string default_severity_label { get; set; }
        public long discovered { get; set; }
        public List<object> events { get; set; }
        public string evidence { get; set; }
        public long first_time_seen { get; set; }
        public bool hasParentApp { get; set; }
        public string impact { get; set; }
        public string impact_label { get; set; }
        public string instance_uuid { get; set; }
        public string language { get; set; }
        public long last_time_seen { get; set; }
        public long last_vuln_time_seen { get; set; }
        public string license { get; set; }
        public string likelihood { get; set; }
        public string likelihood_label { get; set; }
        public List<object> links { get; set; }
        public List<object> notes { get; set; }
        public string organization_name { get; set; }
        public ParentApplication parent_application { get; set; }
        public string pending_status { get; set; }
        public long pending_status_creation_time { get; set; }
        public string pending_substatus { get; set; }
        public bool reported_to_bug_tracker { get; set; }
        public long reported_to_bug_tracker_time { get; set; }
        public Request request { get; set; }
        public string rule_name { get; set; }
        public string rule_title { get; set; }
        public List<object> server_environments { get; set; }
        public List<object> servers { get; set; }
        public List<object> session_metadata { get; set; }
        public string severity { get; set; }
        public string severity_label { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public string sub_title { get; set; }
        public string sub_status_keycode { get; set; }
        public List<object> tags { get; set; }
        public string title { get; set; }
        public int total_traces_received { get; set; }
        public string uuid { get; set; }
        public List<object> violations { get; set; }
        public bool visible { get; set; }
        public List<object> vulnerability_instances { get; set; }
    }

    public class Application
    {
    }

    public class ParentApplication
    {
    }

    public class Request
    {
        public string body { get; set; }
        public List<object> headers { get; set; }
        public List<object> links { get; set; }
        public string method { get; set; }
        public List<object> parameters { get; set; }
        public int port { get; set; }
        public string protocol { get; set; }
        public string queryString { get; set; }
        public string uri { get; set; }
        public string url { get; set; }
        public string version { get; set; }
    }
}
