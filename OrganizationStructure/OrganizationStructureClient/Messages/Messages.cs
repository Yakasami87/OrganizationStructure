using Microsoft.AspNetCore.SignalR.Client;
using OrganizationStructureShared.Models.DTOs;
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
        public HubConnection ConnectionHub { get; set; }

        public AddPersonMessage(HttpClient httpClient, HubConnection connectionHub)
        {
            HttpClient = httpClient;
            ConnectionHub = connectionHub;
        }
    }

    public class EditPersonMessage
    {
        public HttpClient HttpClient { get; set; }
        public HubConnection ConnectionHub { get; set; }
        public PersonDTO Person { get; set; }

        public EditPersonMessage(HttpClient httpClient, HubConnection connectionHub, PersonDTO person)
        {
            HttpClient = httpClient;
            ConnectionHub = connectionHub;
            Person = person;
        }
    }

    public class AddRoleMessage
    {
        public HttpClient HttpClient { get; set; }     

        public AddRoleMessage(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }

    public class EditRoleMessage
    {
        public HttpClient HttpClient { get; set; }
        public RoleDTO Role { get; set; }

        public EditRoleMessage(HttpClient httpClient, RoleDTO role)
        {
            HttpClient = httpClient;
            Role = role;
        }
    }

    public class ConfirmMessage { }
    public class CloseMessage { }
}
