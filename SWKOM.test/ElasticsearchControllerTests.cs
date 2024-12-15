using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using sws.SL.Controllers;
using sws.SL.DTOs;
using sws.BLL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sws.Tests.Controllers
{
    [TestFixture]
    public class ElasticsearchControllerTests
    {
        private Mock<IDocumentSearchService> _mockDocumentSearchService;
        private ElasticsearchController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockDocumentSearchService = new Mock<IDocumentSearchService>();
            _controller = new ElasticsearchController(_mockDocumentSearchService.Object);
        }

        [Test]
        public async Task FulltextSearch_ReturnsOkResultWithResults_WhenSearchIsSuccessful()
        {
            // Arrange
            string query = "test";
            var searchResults = new List<DocumentSearchDTO>
            {
                new DocumentSearchDTO { Id = 1, Name = "Miau" },
                new DocumentSearchDTO { Id = 2, Name = "Guau" }
            };

            _mockDocumentSearchService
                .Setup(service => service.SearchDocumentsAsync(query))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.FulltextSearch(query);

            // Assert
            // The controller returns an OK object, therefore we need to cast it
            Assert.That((result.Result as OkObjectResult).Value, Is.EqualTo(searchResults));
        }

        [Test]
        public async Task FulltextSearch_ReturnsInternalServerError_WhenBusinessLogicExceptionIsThrown()
        {
            // Arrange
            string query = "test";
            _mockDocumentSearchService
                .Setup(service => service.SearchDocumentsAsync(query))
                .ThrowsAsync(new BusinessLogicException("Business logic error"));

            // Act
            var result = await _controller.FulltextSearch(query);

            // Assert
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task FulltextSearch_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
        {
            // Arrange
            string query = "test query";
            _mockDocumentSearchService
                .Setup(service => service.SearchDocumentsAsync(query))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.FulltextSearch(query);
            
            // Assert
            Assert.That((result.Result as ObjectResult).StatusCode, Is.EqualTo(500));
        }

    }
}
