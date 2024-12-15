using Moq;
using NUnit.Framework;
using sws.BLL;
using sws.DAL;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWKOM.test
{
    [TestFixture]
    public class DocumentSearchServiceTests
    {
        private Mock<IElasticsearchRepository> _mockElasticsearchRepository;
        private Mock<IDocumentRepository> _mockDocumentRepository;
        private DocumentSearchService _documentSearchService;

        [SetUp]
        public void Setup()
        {
            _mockElasticsearchRepository = new Mock<IElasticsearchRepository>();
            _mockDocumentRepository = new Mock<IDocumentRepository>();

            _documentSearchService = new DocumentSearchService(
                _mockElasticsearchRepository.Object,
                _mockDocumentRepository.Object
            );
        }

        [Test]
        public async Task SearchDocumentsAsync_ReturnsResults_WhenDocumentsAreFound()
        {
            // Arrange
            var query = "test query";
            var documentIds = new List<long> { 1, 2 };
            var documents = new List<UploadDocument>
            {
                new UploadDocument { Id = 1, Name = "Document 1" },
                new UploadDocument { Id = 2, Name = "Document 2" }
            };

            _mockElasticsearchRepository
                .Setup(repo => repo.SearchQueryInDocumentContent(query))
                .ReturnsAsync(documentIds);

            _mockDocumentRepository
                .Setup(repo => repo.GetDocumentsByIdsAsync(documentIds))
                .ReturnsAsync(documents);

            // Act
            var result = await _documentSearchService.SearchDocumentsAsync(query);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(r => r.Name), Is.EquivalentTo(new[] { "Document 1", "Document 2" }));

            _mockElasticsearchRepository.Verify(repo => repo.SearchQueryInDocumentContent(query), Times.Once);
            _mockDocumentRepository.Verify(repo => repo.GetDocumentsByIdsAsync(documentIds), Times.Once);
        }

        [Test]
        public async Task SearchDocumentsAsync_ReturnsEmptyList_WhenNoDocumentsFound()
        {
            // Arrange
            var query = "no results";
            var emptyDocumentIds = new List<long>();

            _mockElasticsearchRepository
                .Setup(repo => repo.SearchQueryInDocumentContent(query))
                .ReturnsAsync(emptyDocumentIds);

            // Act
            var result = await _documentSearchService.SearchDocumentsAsync(query);

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.That(result.Count, Is.EqualTo(0));

            _mockElasticsearchRepository.Verify(repo => repo.SearchQueryInDocumentContent(query), Times.Once);
            _mockDocumentRepository.Verify(repo => repo.GetDocumentsByIdsAsync(It.IsAny<List<long>>()), Times.Never);
        }


        [Test]
        public void SearchDocumentsAsync_ThrowsBusinessLogicException_WhenDatabaseFails()
        {
            // Arrange
            var query = "valid query";
            var documentIds = new List<long> { 1, 2 };

            _mockElasticsearchRepository
                .Setup(repo => repo.SearchQueryInDocumentContent(query))
                .ReturnsAsync(documentIds);

            _mockDocumentRepository
                .Setup(repo => repo.GetDocumentsByIdsAsync(documentIds))
                .ThrowsAsync(new DataAccessException("Database error"));

            // Act & Assert
            var exception = Assert.ThrowsAsync<BusinessLogicException>(
                async () => await _documentSearchService.SearchDocumentsAsync(query)
            );

            Assert.That(exception.Message, Does.Contain("Error searching documents"));
            _mockElasticsearchRepository.Verify(repo => repo.SearchQueryInDocumentContent(query), Times.Once);
            _mockDocumentRepository.Verify(repo => repo.GetDocumentsByIdsAsync(documentIds), Times.Once);
        }
    }
}
