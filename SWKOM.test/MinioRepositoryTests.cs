using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using Minio;
using Minio.DataModel;      
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using System.Net;


namespace SWKOM.test
{
    [TestFixture]
    public class MinioRepositoryTests
    {
        private Mock<IMinioClient> _minioClientMock;
        private MinioRepository _repository;
        private const string BucketName = "uploads";

        [SetUp]
        public void SetUp()
        {
            //initialize mock minIO client
            _minioClientMock = new Mock<IMinioClient>(MockBehavior.Strict);

            // set up default behavior for bucket existense
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);


            //set up default behavior for file upload
         _minioClientMock
            .Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(
                HttpStatusCode.OK,
                BucketName,
                new Dictionary<string, string>(),
                3L,          // file length
                "test-etag"
           ));

            //set up default behavior for bucket creation
            _minioClientMock
                .Setup(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            // Initialize the repository with the mock client
            _repository = new MinioRepository(_minioClientMock.Object);
        }

        [Test]
        public void Constructor_WithNullMinioClient_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MinioRepository(null));
        }

        [Test]
        public async Task Add_BucketExists_DoesNotCreateBucket()
        {
            //test adding a document when bucket already exists

            //Arrange: mock to simulate existing bucket
            var document = CreateTestUploadDocument();
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            await _repository.Add(document);

            //Assert
            _minioClientMock.Verify(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Never);
            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Add_BucketDoesNotExist_CreatesBucket()
        {
            //adding a document when bucket does not exist
            //Arrange: mock to simulate missing bucket
            var document = CreateTestUploadDocument();
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            //Act
            await _repository.Add(document);
            //Assert: verify bucket was created before adding document
            _minioClientMock.Verify(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Add_UploadsDocumentSuccessfully()
        {
            //test document is uploaded successfully to MinIO
            //Arrange
            var document = CreateTestUploadDocument();
            //Act
            await _repository.Add(document);
            //Assert
            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

      
        [Test]
        public async Task Add_EmptyFile_DoesNotThrow()
        {
            var document = new UploadDocument
            {
                Id = 99999L, // long ID
                File = Array.Empty<byte>()
            };

            await _repository.Add(document);

            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private UploadDocument CreateTestUploadDocument()
        {
            //helper method to create a test document
            return new UploadDocument
            {
                Id = 12345L,
                File = new byte[] { 0x01, 0x02, 0x03 }
            };
        }
    }
}
