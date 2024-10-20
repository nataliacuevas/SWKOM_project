using AutoMapper;
using sws.DAL.Entities;
using sws.SL.DTOs;


namespace sws.BLL.Mappers

{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<UploadDocumentDTO, UploadDocument>();
            CreateMap<UploadDocument, UploadDocumentDTO>();
        }
        /*  From the demo repository.
        public class MapperConfig
        {
            public static Mapper InitializeAutomapper()
            {
                //Provide all the Mapping Configuration
                var config = new MapperConfiguration(cfg =>
                {
                    //Configuring 
                    cfg.CreateMap<UploadDocumentDTO, UploadDocument>();
                    cfg.CreateMap<UploadDocument, UploadDocumentDTO>();
                    //Any Other Mapping Configuration ....
                });

                //Create an Instance of Mapper and return that Instance
                var mapper = new Mapper(config);
                return mapper;
            }
        }
        */
    }
}
