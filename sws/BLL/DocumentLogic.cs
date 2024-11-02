using AutoMapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using System.Text;


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
            send2RabbitMQ(document);
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

            string message = "Uploading document with id: " + docu.Id.ToString();
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
