namespace OrganizationStructureService.AutoMapperProfiles
{
    public class Profiles : Profile
    {
        public Profiles() 
        {
            CreateMap<Person, PersonDTO>()
               .ForSourceMember(src => src.RoleId, opt => opt.DoNotValidate());

            CreateMap<PersonDTO, Person>()
                .ForSourceMember(src => src.Employees, opt => opt.DoNotValidate());

            CreateMap<Role,RoleDTO>()
                .ForSourceMember(src => src.Persons, opt => opt.DoNotValidate());
            
            CreateMap<RoleDTO, Role>();
        }
    }
}
