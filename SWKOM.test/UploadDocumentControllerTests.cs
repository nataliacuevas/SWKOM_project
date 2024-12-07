
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using sws.BLL;
using sws.SL.Controllers;
using sws.SL.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SWKOM.test
{

    [TestFixture]
    public class UploadDocumentControllerTests
    {
        private Mock<IDocumentLogic> _mockDocumentLogic;
        private UploadDocumentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDocumentLogic = new Mock<IDocumentLogic>();
            _controller = new UploadDocumentController(_mockDocumentLogic.Object);
        }

        private IFormFile CreateMockFormFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "formFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
        }

        [Test]
        public async Task GetUploadedDocuments_ShouldReturnAllDocuments()
        {
            var documents = new List<DownloadDocumentDTO>
        {
            new DownloadDocumentDTO { Id = 1, Name = "Doc1" },
            new DownloadDocumentDTO { Id = 2, Name = "Doc2" }
        };
            _mockDocumentLogic.Setup(logic => logic.GetAll()).Returns(documents);

            var result = await _controller.GetUploadedDocuments();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var actionResult = result.Result as OkObjectResult;
            Assert.That(actionResult?.Value, Is.EqualTo(documents));
        }

        [Test]
        public async Task GetUploadedDocuments_ShouldReturnServerError_WhenExceptionOccurs()
        {
            _mockDocumentLogic.Setup(logic => logic.GetAll()).Throws(new Exception());

            var result = await _controller.GetUploadedDocuments();

            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var actionResult = result.Result as ObjectResult;
            Assert.That(actionResult?.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUploadDocument_ShouldReturnNotFound_WhenDocumentDoesNotExist()
        {
            _mockDocumentLogic.Setup(logic => logic.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((DownloadDocumentDTO)null);

            var result = await _controller.GetUploadDocument(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetUploadDocument_ShouldReturnDocument_WhenFound()
        {
            var document = new DownloadDocumentDTO { Id = 1, Name = "TestDoc" };
            _mockDocumentLogic.Setup(logic => logic.GetByIdAsync(1)).ReturnsAsync(document);

            var result = await _controller.GetUploadDocument(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var actionResult = result.Result as OkObjectResult;
            Assert.That(actionResult?.Value, Is.EqualTo(document));
        }

        [Test]
        public async Task PostUploadDocument_ShouldReturnOk_WhenUploadIsSuccessful()
        {
            var mockFile = CreateMockFormFile("NewDoc.pdf", new byte[] { 0x01 });
            var uploadDocument = new UploadDocumentDTO { Name = "NewDoc", File = mockFile };

            var result = await _controller.PostUploadDocument(uploadDocument);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _mockDocumentLogic.Verify(logic => logic.Add(uploadDocument), Times.Once);
        }

        [Test]
        public async Task PostUploadDocument_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            var mockFile = CreateMockFormFile("NewDoc.pdf", new byte[] { 0x01 });
            var uploadDocument = new UploadDocumentDTO { Name = "NewDoc", File = mockFile };
            _mockDocumentLogic.Setup(logic => logic.Add(It.IsAny<UploadDocumentDTO>())).ThrowsAsync(new Exception());

            var result = await _controller.PostUploadDocument(uploadDocument);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var actionResult = result as ObjectResult;
            Assert.That(actionResult?.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteUploadDocument_ShouldReturnNoContent_WhenDeletionIsSuccessful()
        {
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Returns(new DownloadDocumentDTO { Id = 1 });

            var result = await _controller.DeleteUploadDocument(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteUploadDocument_ShouldReturnNotFound_WhenDocumentDoesNotExist()
        {
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Returns((DownloadDocumentDTO)null);

            var result = await _controller.DeleteUploadDocument(1);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteUploadDocument_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Throws(new Exception());

            var result = await _controller.DeleteUploadDocument(1);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var actionResult = result as ObjectResult;
            Assert.That(actionResult?.StatusCode, Is.EqualTo(500));
        }
    }

}