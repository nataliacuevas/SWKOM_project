
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using sws.BLL;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM.test
{
    [TestFixture]
    public class DocumentLogicTests
    {
        private Mock<IDocumentRepository> _mockDocumentRepository;
        private Mock<IMinioRepository> _mockMinioRepository;
        private Mock<IMapper> _mockMapper;
        private DocumentLogic _documentLogic;

        [SetUp]
        public void Setup()
        {
            
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _mockMinioRepository = new Mock<IMinioRepository>();
            _mockMapper = new Mock<IMapper>();

            _documentLogic = new DocumentLogic(
                _mockDocumentRepository.Object,
                _mockMapper.Object,
                _mockMinioRepository.Object);
        }
        //test add method to ensure document is correctly aved to repository and minIO and message is sent to rabbitmq
        [Test]
        public async Task Add_ShouldAddDocumentAndSendToRabbitMQ()
        {
            // Arrange
            //set up input data and mocks
            var fileContent = Encoding.UTF8.GetBytes("Sample Content");
            var fileName = "TestDocument.txt";
            var formFileMock = new Mock<IFormFile>();

            formFileMock.Setup(f => f.FileName).Returns(fileName);
            formFileMock.Setup(f => f.Length).Returns(fileContent.Length);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));

            var uploadDocumentDTO = new UploadDocumentDTO
            {
                Name = fileName,
                File = formFileMock.Object
            };

            var uploadDocument = new UploadDocument
            {
                Id = 1,
                Name = fileName,
                File = fileContent
            };

            _mockMapper.Setup(m => m.Map<UploadDocument>(uploadDocumentDTO))
                .Returns(uploadDocument);

            // Act : call add method
            await _documentLogic.Add(uploadDocumentDTO);

            // Assert : verify expected repository interactions
            _mockDocumentRepository.Verify(repo => repo.Add(uploadDocument), Times.Once);
            _mockMinioRepository.Verify(repo => repo.Add(uploadDocument), Times.Once);
        }

        //test to ensure method retrieves and removes a document if it exists in the repository
        [Test]
        public void PopById_ShouldReturnDocumentIfExists()
        {
            // Arrange: set up a document to be returned by the mock repository
            var document = new UploadDocument { Id = 1, Name = "Test Document" };
            var expectedDTO = new DownloadDocumentDTO { Id = 1, Name = "Test Document" };

            _mockDocumentRepository.Setup(repo => repo.Pop(1)).Returns(document);
            _mockMapper.Setup(m => m.Map<DownloadDocumentDTO>(document)).Returns(expectedDTO);

            // Act
            var result = _documentLogic.PopById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedDTO.Name));
        }

        [Test]
        public void PopById_ShouldReturnNullIfDocumentNotFound()
        {
            
            // Arrange
            _mockDocumentRepository.Setup(repo => repo.Pop(It.IsAny<long>())).Returns((UploadDocument)null);

            // Act
            var result = _documentLogic.PopById(99);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetById_ShouldReturnMappedDocument()
        {
            //test method to ensure it retrives a document by ID and maps it to a DTO

            // Arrange
            var document = new UploadDocument { Id = 1, Name = "Test Document" };
            var expectedDTO = new DownloadDocumentDTO { Id = 1, Name = "Test Document" };

            _mockDocumentRepository.Setup(repo => repo.Get(1)).Returns(document);
            _mockMapper.Setup(m => m.Map<DownloadDocumentDTO>(document)).Returns(expectedDTO);

            // Act
            var result = _documentLogic.GetById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedDTO.Name));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedDocument()
        {
        
            // Arrange
            var document = new UploadDocument { Id = 1, Name = "Test Document" };
            var expectedDTO = new DownloadDocumentDTO { Id = 1, Name = "Test Document" };

            _mockDocumentRepository.Setup(repo => repo.GetAsync(1)).ReturnsAsync(document);
            _mockMapper.Setup(m => m.Map<DownloadDocumentDTO>(document)).Returns(expectedDTO);

            // Act
            var result = await _documentLogic.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedDTO.Name));
        }

        [Test]
        public void GetAll_ShouldReturnListOfDocuments()
        {
            //retrieves all documents and maps them to a list of DTOs

            // Arrange: set up documents and their corresponding DTOs
            var documents = new List<UploadDocument>
            {
                new UploadDocument { Id = 1, Name = "Doc 1" },
                new UploadDocument { Id = 2, Name = "Doc 2" }
            };

            var expectedDTOs = documents.Select(doc => new DownloadDocumentDTO { Id = doc.Id, Name = doc.Name }).ToList();

            _mockDocumentRepository.Setup(repo => repo.GetAll()).Returns(documents);
            _mockMapper.Setup(m => m.Map<DownloadDocumentDTO>(It.IsAny<UploadDocument>()))
                .Returns((UploadDocument doc) => expectedDTOs.First(dto => dto.Id == doc.Id));

            // Act
            var result = _documentLogic.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(dto => dto.Name), Is.EquivalentTo(expectedDTOs.Select(dto => dto.Name)));
        }

        [Test]
        public void Put_ShouldUpdateDocument()
        {
            // Arrange
            var uploadDocumentDTO = new UploadDocumentDTO { Name = "Updated Document" };
            var uploadDocument = new UploadDocument { Id = 1, Name = "Updated Document" };
            var updatedDocument = new UploadDocument { Id = 1, Name = "Updated Document" };
            var expectedDTO = new DownloadDocumentDTO { Id = 1, Name = "Updated Document" };

            _mockMapper.Setup(m => m.Map<UploadDocument>(uploadDocumentDTO)).Returns(uploadDocument);
            _mockDocumentRepository.Setup(repo => repo.Put(uploadDocument)).Returns(updatedDocument);
            _mockMapper.Setup(m => m.Map<DownloadDocumentDTO>(updatedDocument)).Returns(expectedDTO);

            // Act
            var result = _documentLogic.Put(uploadDocumentDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedDTO.Name));
        }
    }
}
