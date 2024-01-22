using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationStructureShared.Models.DTOs
{
    public class PersonDTO 
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PersonDTO? Manager { get; set; }
        public RoleDTO? Role { get; set; }
        public List<PersonDTO> Employees { get; set; } = new List<PersonDTO>();
    }
}
