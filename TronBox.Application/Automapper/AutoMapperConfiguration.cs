using AutoMapper;

namespace TronBox.Domain.Automapper
{
    public class AutoMapperConfiguration
    {
        public static IMapper RegisterMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDomainMappingProfile>();
                cfg.AddProfile<Comum.Domain.Automapper.DomainToViewModelMappingProfile>();
                cfg.AddProfile<Comum.Domain.Automapper.ViewModelToDomainMappingProfile>();
            });

            return config.CreateMapper();
        }
    }
}
