using AutoMapper;

namespace TronBox.Domain.Automapper
{
    public class AutoMapperConfiguration
    {
        public static void RegisterMappings()
        {
            Mapper.Reset();
            Mapper.Initialize(p =>
            {
                p.AddProfile<DomainToViewModelMappingProfile>();
                p.AddProfile<ViewModelToDomainMappingProfile>();
                p.AddProfile<Comum.Domain.Automapper.DomainToViewModelMappingProfile>();
                p.AddProfile<Comum.Domain.Automapper.ViewModelToDomainMappingProfile>();
            });
        }
    }
}
