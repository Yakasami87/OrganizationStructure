namespace OrganizationStructureService.Data.Models
{
    public class Role
    {
        #region Proprieties

        public int Id { get; set; }
        public string Name { get; set; } 

        #endregion

        #region DB Relationship Definition

        public ICollection<Person> Persons { get; set; }

        #endregion
    }
}
