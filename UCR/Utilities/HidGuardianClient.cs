using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace UCR.Utilities
{
    internal class HidGuardianClient : IDisposable
    {
        private const string HidGuardianUrl = "http://localhost:26762/api/v1/hidguardian";
        private readonly RestClient _client;

        public HidGuardianClient()
        {
            _client = new RestClient(HidGuardianUrl);
        }

        public void WhitelistProcess()
        {
            var request = new RestRequest("whitelist/add/{id}", Method.GET);
            request.AddUrlSegment("id", Process.GetCurrentProcess().Id.ToString());
            var response = _client.Execute(request);
        }

        public void RemoveWhitelistProcess()
        {
            var request = new RestRequest("whitelist/remove/{id}", Method.GET);
            request.AddUrlSegment("id", Process.GetCurrentProcess().Id.ToString());
            _client.Execute(request);
        }

        public void Dispose()
        {
            RemoveWhitelistProcess();
        }
    }
}
