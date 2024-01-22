namespace OrganizationStructureService.Data.Models
{
    public class Person
    {
        #region Proprieties

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? ManagerId { get; set; }
        public int? RoleId { get; set; }

        #endregion

        #region DB Relationship Definition

        public Person Manager { get; set; }
        public Role Role { get; set; }

        #endregion

    }
}
