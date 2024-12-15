using AutoMapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using System.Text;
using log4net;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using sws.DAL;


namespace sws.BLL
{
    public class DocumentLogic : IDocumentLogic
    {
        // Dependencies injected into the business logic layer
        private readonly IDocumentRepository _documentRepository; //handles db interactions
        private readonly IMinioRepository _minioRepository; //handles interactions with MinIO 
        private readonly IMapper _mapper; //maps DTOs to entities and vice versa
        
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentLogic));

        public DocumentLogic(IDocumentRepository documentRepository, IMapper mapper, IMinioRepository minioRepository) 
        {
            _documentRepository = documentRepository;
            _minioRepository = minioRepository;
            _mapper = mapper;
        }

        /* Adds a new document to the system: saves it to the database, 
         * uploads it to MinIO, and sends a RabbitMQ message.
        */
        public async Task Add(UploadDocumentDTO uploadDocumentDTO)
        {
            log.Info("Adding a new document.");
            try
            {
                UploadDocument document = _mapper.Map<UploadDocument>(uploadDocumentDTO);
                _documentRepository.Add(document); // ID is added to document
                await _minioRepository.Add(document);

                send2RabbitMQ(document);

                log.Info($"Document '{document.Name}' added successfully.");
            }
            catch (Exception ex)
            {
                log.Error("Error adding document.", ex);
            }
        }
        

        public DownloadDocumentDTO? PopById(long id)
        {
            log.Info($"Attempting to pop document with ID: {id}");
            var document = _documentRepository.Pop(id);
            if (document == null)
            {
                log.Warn($"Document with ID {id} not found.");
            }
            else
            {
                log.Info($"Document with ID {id} popped successfully.");
            }
            return _mapper.Map<DownloadDocumentDTO>(document);
        }
      
        public async Task<DownloadDocumentDTO?> GetByIdAsync(long id)
        {
            try
            {
                var document = await _documentRepository.GetAsync(id);
                return _mapper.Map<DownloadDocumentDTO>(document);
            }
            catch (DataAccessException ex)
            {
                throw new BusinessLogicException("Error fetching document asynchronously by ID in the business logic layer.", ex);
            }
        }


        public DownloadDocumentDTO? GetById(long id)
        {
            try
            {
                var document = _documentRepository.Get(id);
                return _mapper.Map<DownloadDocumentDTO>(document);
            }
            catch (DataAccessException ex)
            {
                throw new BusinessLogicException("Error fetching document by ID in the business logic layer.", ex);
            }
        }

        public DownloadDocumentDTO? Put(UploadDocumentDTO uploadDocumentDTO)
        {
            var uploadDocument = _mapper.Map<UploadDocument>(uploadDocumentDTO);
            var document = _documentRepository.Put(uploadDocument);

            return _mapper.Map<DownloadDocumentDTO>(document);

        }
       
        public List<DownloadDocumentDTO> GetAll()
        {
            try
            {
                log.Info("Fetching all documents.");
                var list = _documentRepository.GetAll();
                return list.Select(doc => _mapper.Map<DownloadDocumentDTO>(doc)).ToList();
            }
            catch (DataAccessException ex)
            {
                throw new BusinessLogicException("Error in the business logic layer while retrieving all documents.", ex);
            }
        }
        
        //Publishes a message to RabbitMQ, notifying that a document has been uploaded
        
        public void send2RabbitMQ(UploadDocument docu) 
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                VirtualHost = "mrRabbit"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "post",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            string message = $"Uploading document with name: {docu.Name} and id. {docu.Id}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "post",
                                 basicProperties: null,
                                 body: body);

        }

    }
}
