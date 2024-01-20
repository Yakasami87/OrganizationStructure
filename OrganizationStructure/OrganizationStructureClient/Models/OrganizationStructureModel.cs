using OrganizationStructureShared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationStructureClient.Models
{
    public class OrganizationStructureModel
    {
        public PersonDTO Person { get; set; }
        public ObservableCollection<OrganizationStructureModel> Employees { get; set; } = new ObservableCollection<OrganizationStructureModel>();
    }
}
