namespace OrganizationStructureService.AutoMapperProfiles
{
    public class Profiles : Profile
    {
        public Profiles() 
        {
            CreateMap<Person, PersonDTO>()
               .ForSourceMember(src => src.RoleId, opt => opt.DoNotValidate());

            CreateMap<PersonDTO, Person>();

            CreateMap<Role,RoleDTO>()
                .ForSourceMember(src => src.Persons, opt => opt.DoNotValidate());
            
            CreateMap<RoleDTO, Role>();
        }
    }
}
