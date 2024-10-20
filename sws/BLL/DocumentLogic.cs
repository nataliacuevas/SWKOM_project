using AutoMapper;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;


namespace sws.BLL
{
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        public DocumentLogic(IDocumentRepository documentRepository, IMapper mapper) 
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public void Add(UploadDocumentDTO uploadDocumentDTO)
        {
            UploadDocument document = _mapper.Map<UploadDocument>(uploadDocumentDTO);
            _documentRepository.Add(document);
        }
    }
}
