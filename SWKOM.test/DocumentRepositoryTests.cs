
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using sws.DAL;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SWKOM.test
{
    [TestFixture]
    public class DocumentRepositoryTests
    {
        private DocumentRepository _repository;
        private UploadDocumentContext _context;

        private UploadDocumentContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<UploadDocumentContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new UploadDocumentContext(options);
        }

        [SetUp]
        public void Setup()
        {
            //initialize the in memory database and repository for testing
            _context = CreateInMemoryContext("TestDatabase");
            _repository = new DocumentRepository(_context);

            // Seed test data with required properties into the db
            _context.UploadedDocuments.AddRange(new List<UploadDocument>
            {
                new UploadDocument { Id = 1, Name = "TestDoc1", File = new byte[] { 0x01, 0x02 } },
                new UploadDocument { Id = 2, Name = "TestDoc2", File = new byte[] { 0x03, 0x04 } },
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            //clean up db after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void Add_ShouldAddDocumentToDatabase()
        {
            
            // Arrange
            var document = new UploadDocument { Id = 3, Name = "NewDoc", File = new byte[] { 0x05, 0x06 } };

            // Act
            var result = _repository.Add(document);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(3));
            Assert.That(result.Name, Is.EqualTo("NewDoc"));
            Assert.That(_context.UploadedDocuments.Count(), Is.EqualTo(3)); // Includes new document
        }

        [Test]
        public void Get_ShouldReturnDocument_WhenExists()
        {
            //test retrieving an existing document by ID
            // Act
            var result = _repository.Get(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("TestDoc1"));
        }

        [Test]
        public void Get_ShouldReturnNull_WhenNotExists()
        {
            //retireving a documents that doesn't exist
            // Act
            var result = _repository.Get(99);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Pop_ShouldRemoveDocument_WhenExists()
        {
            //remove an existing document by ID
            // Act
            var result = _repository.Pop(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("TestDoc1"));
            Assert.That(_context.UploadedDocuments.Count(), Is.EqualTo(1)); // One document removed
        }

        [Test]
        public void Pop_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = _repository.Pop(99);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(_context.UploadedDocuments.Count(), Is.EqualTo(2)); // No documents removed
        }
        [Test]
        public async Task GetAsync_ShouldReturnDocument_WhenExists()
        {
            var result = await _repository.GetAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("TestDoc1"));
        }
        [Test]
        public void GetAll_ShouldReturnAllDocuments()
        {
            // Act
            var result = _repository.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(doc => doc.Name == "TestDoc1"), Is.True);
            Assert.That(result.Any(doc => doc.Name == "TestDoc2"), Is.True);
        }


        [Test]
        public void Put_ShouldReturnNull_WhenDocumentDoesNotExist()
        {
            // Arrange
            var nonExistentDocument = new UploadDocument { Id = 99, Name = "NonExistentDoc", File = new byte[] { 0x07 } };

            // Act
            var result = _repository.Put(nonExistentDocument);

            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task GetAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetAsync(999);

            Assert.That(result, Is.Null);
        }
    }
}
