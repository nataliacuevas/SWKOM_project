using AutoMapper;
using sws.DAL.Entities;
using sws.SL.DTOs;


namespace sws.BLL.Mappers

{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<UploadDocumentDTO, UploadDocument>()
              .ForMember(dest => dest.File, opt => opt.MapFrom(src => ConvertIFormFileToByteArray(src.File)));
            // mapping as uploadDocumentDTO -> UploadDocument -> DownloadDocumentDTO because the upload and download fields do not match 
            CreateMap<UploadDocument, DownloadDocumentDTO>();
            
        }

        // Helper method to convert IFormFile to byte[]
        private static byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null) return null;

            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
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
