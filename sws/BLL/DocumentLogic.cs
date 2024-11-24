using AutoMapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using System.Text;
using log4net;


namespace sws.BLL
{
    public class DocumentLogic : IDocumentLogic
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMinioRepository _minioRepository;
        private readonly IMapper _mapper;
        
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentLogic));

        public DocumentLogic(IDocumentRepository documentRepository, IMapper mapper, IMinioRepository minioRepository) 
        {
            _documentRepository = documentRepository;
            _minioRepository = minioRepository;
            _mapper = mapper;
                        
        }

        public async Task Add(UploadDocumentDTO uploadDocumentDTO)
        {
            log.Info("Adding a new document.");
            try
            {
                UploadDocument document = _mapper.Map<UploadDocument>(uploadDocumentDTO);
                _documentRepository.Add(document);
                await _minioRepository.Add(document);
                
                send2RabbitMQ(document);
                
                log.Info($"Document '{document.Name}' added successfully.");
            }
            catch(Exception ex)
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


        public DownloadDocumentDTO? GetById(long id)
        {
            var document = _documentRepository.Get(id);
            return _mapper.Map<DownloadDocumentDTO>(document);
        }

        public async Task<DownloadDocumentDTO?> GetByIdAsync(long id)
        {
            var document = await _documentRepository.GetAsync(id);
            return _mapper.Map<DownloadDocumentDTO>(document);
        }


        public List<DownloadDocumentDTO> GetAll()
        {
            log.Info("Fetching all documents.");
            var list = _documentRepository.GetAll();
            return list.Select(doc => _mapper.Map<DownloadDocumentDTO>(doc)).ToList();
        }

        public DownloadDocumentDTO? Put(UploadDocumentDTO uploadDocumentDTO)
        {
            var uploadDocument = _mapper.Map<UploadDocument>(uploadDocumentDTO);
            var document = _documentRepository.Put(uploadDocument);

            return _mapper.Map<DownloadDocumentDTO>(document);

        }


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
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            string message = $"Uploading document with name: {docu.Name} and id. {docu.Id}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "post",
                                 basicProperties: null,
                                 body: body);
/*  To retrieve message from RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                VirtualHost = "mrRabbit"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "post",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(201, $" [x] Received {message}");
            };
            channel.BasicConsume(queue: "post",
                     autoAck: true,
                     consumer: consumer);
*/
        }

    }
}
