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

        public UploadDocumentDTO? PopById(long id)
        {
            var document = _documentRepository.Pop(id);
            return _mapper.Map<UploadDocumentDTO>(document);
        }

        public UploadDocumentDTO? GetById(long id)
        {
            var document = _documentRepository.Get(id);
            return _mapper.Map<UploadDocumentDTO>(document);
        }

        public async Task<UploadDocumentDTO?> GetByIdAsync(long id)
        {
            var document = await _documentRepository.GetAsync(id);
            return _mapper.Map<UploadDocumentDTO>(document);
        }


        public List<UploadDocumentDTO> GetAll()
        {
            var list = _documentRepository.GetAll();
            return list.Select(doc => _mapper.Map<UploadDocumentDTO>(doc)).ToList();
        }

        public UploadDocumentDTO? Put(UploadDocumentDTO uploadDocumentDTO)
        {
            var uploadDocument = _mapper.Map<UploadDocument>(uploadDocumentDTO);
            var document = _documentRepository.Put(uploadDocument);
            return _mapper.Map<UploadDocumentDTO>(document);

        }
    }
}
