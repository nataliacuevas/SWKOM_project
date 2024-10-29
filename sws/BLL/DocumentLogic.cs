using AutoMapper;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using log4net;


namespace sws.BLL
{
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentLogic));

        public DocumentLogic(IDocumentRepository documentRepository, IMapper mapper) 
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public void Add(UploadDocumentDTO uploadDocumentDTO)
        {
            
            UploadDocument document = _mapper.Map<UploadDocument>(uploadDocumentDTO);
            _documentRepository.Add(document);
            log.Info("Document added successfully.");
        }

        public UploadDocumentDTO? PopById(long id)
        {
            log.Info($"Attempting to delete document with ID: {id}");
            var document = _documentRepository.Pop(id);
            if(document == null)
            {
                log.Warn($"Document with ID: {id} not found for deletion.");
                return null;
            }
            log.Info($"Document with ID: {id} deleted successfully.");
            return _mapper.Map<UploadDocumentDTO>(document);
        }

        public UploadDocumentDTO? GetById(long id)
        {
            log.Info($"Retrieving document with ID: {id}");
            var document = _documentRepository.Get(id);

            if (document == null)
            {
                log.Warn($"Document with ID: {id} not found.");
                return null;
            }
            log.Info($"Document with ID: {id} retrieved successfully.");
            return _mapper.Map<UploadDocumentDTO>(document);
        }

        public async Task<UploadDocumentDTO?> GetByIdAsync(long id)
        {
            var document = await _documentRepository.GetAsync(id);
            return _mapper.Map<UploadDocumentDTO>(document);
        }


        public List<UploadDocumentDTO> GetAll()
        {
            log.Info("Retrieving all documents.");
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
