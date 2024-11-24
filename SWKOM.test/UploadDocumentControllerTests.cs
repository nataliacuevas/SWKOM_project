using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
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
        public async Task PostUploadDocument_ShouldReturnOk_WhenUploadIsSuccessful()
        {
            // Arrange
            var mockFile = CreateMockFormFile("NewDoc.pdf", new byte[] { 0x01 });
            var uploadDocument = new UploadDocumentDTO { Name = "NewDoc", File = mockFile };

            // Act
            var result = await _controller.PostUploadDocument(uploadDocument);

            // Assert
            Assert.That(result, Is.Not.Null);
            var actionResult = result as OkObjectResult;
            Assert.That(actionResult?.StatusCode, Is.EqualTo(200));
            Assert.That(actionResult?.Value, Is.EqualTo("File uploaded successfully"));
            _mockDocumentLogic.Verify(logic => logic.Add(uploadDocument), Times.Once);
        }

        [Test]
        public async Task GetUploadDocument_ShouldReturnNotFound_WhenDocumentDoesNotExist()
        {
            // Arrange
            _mockDocumentLogic.Setup(logic => logic.GetByIdAsync(1)).ReturnsAsync((DownloadDocumentDTO)null!);

            // Act
            var result = await _controller.GetUploadDocument(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }


        [Test]
        public async Task DeleteUploadDocument_ShouldReturnNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Returns(new DownloadDocumentDTO { Id = 1 });

            // Act
            var result = await _controller.DeleteUploadDocument(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteUploadDocument_ShouldReturnNotFound_WhenDocumentDoesNotExist()
        {
            // Arrange
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Returns((DownloadDocumentDTO)null);

            // Act
            var result = await _controller.DeleteUploadDocument(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteUploadDocument_ShouldReturnServerError_WhenDeletionFails()
        {
            // Arrange
            _mockDocumentLogic.Setup(logic => logic.PopById(1)).Throws(new System.Exception());

            // Act
            var result = await _controller.DeleteUploadDocument(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var actionResult = result as ObjectResult;
            Assert.That(actionResult?.StatusCode, Is.EqualTo(500));
            Assert.That(actionResult?.Value, Is.EqualTo("Internal server error while deleting document."));
        }
    }
}
