using NUnit.Framework;
using Moq;
using sws.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using sws.DAL.Entities;
using sws.DAL;
using Microsoft.Extensions.Logging;


namespace SWKOM.test
{
    public class Tests
    {
        // Need to use different database names else the tests collide
        private IUploadDocumentContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<UploadDocumentContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new UploadDocumentContext(options);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Add_ShouldAddDocumentAndSaveChanges_Once()
        {
            // Arrange
            var context = CreateInMemoryContext("Add_ShouldAddDocumentAndSaveChanges_Once");
            var loggerMock = new Mock<ILogger<DocumentRepository>>();

            var repository = new DocumentRepository(context, loggerMock.Object);

            var document = new UploadDocument { File = [] };

            // Act
            var result = repository.Add(document);

            // Assert
            Assert.That(context.UploadedDocuments.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Pop_WhenDocumentExists_RemovesDocumentAndSavesChanges()
        {
            // Arrange

            var context = CreateInMemoryContext("Pop_WhenDocumentExists_RemovesDocumentAndSavesChanges");
            var loggerMock = new Mock<ILogger<DocumentRepository>>();

            // Seed the in-memory database with a document
            var document = new UploadDocument { Id = 1, File = [] };
            context.UploadedDocuments.Add(document);
            context.SaveChanges();

            var repository = new DocumentRepository(context, loggerMock.Object);

            // Act
            var result = repository.Pop(1);

            // Assert
            Assert.That(document, Is.EqualTo(result));
            Assert.That(context.UploadedDocuments.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Pop_WhenNoDocumentExists_DoesNothing()
        {
            // Arrange

            var context = CreateInMemoryContext("Pop_WhenNoDocumentExists_DoesNothing");
            var loggerMock = new Mock<ILogger<DocumentRepository>>();

            // Seed the in-memory database with a document
            var document = new UploadDocument { Id = 1, File = [] };
            context.UploadedDocuments.Add(document);
            context.SaveChanges();

            var repository = new DocumentRepository(context, loggerMock.Object);

            // Act
            var result = repository.Pop(2);

            // Assert
            Assert.That(result, Is.EqualTo(null));
            Assert.That(context.UploadedDocuments.Count(), Is.EqualTo(1));
        }

    }
}