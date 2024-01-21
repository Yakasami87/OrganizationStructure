using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationStructureClient.Messages.Messages
{
    public class AddPersonMessage
    {
        public HttpClient HttpClient { get; set; }

        public AddPersonMessage(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }

    public class ConfirmMessage { }
    public class CloseMessage { }
}
