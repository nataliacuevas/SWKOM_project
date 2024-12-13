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
            _minioClientMock = new Mock<IMinioClient>(MockBehavior.Strict);

            // Default bucket check to true, can override in specific tests.
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _minioClientMock
      .Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new PutObjectResponse(
          HttpStatusCode.OK,
          BucketName,
          new Dictionary<string, string>(),
          3L,          // Assuming the file is 3 bytes long, adjust accordingly
          "test-etag"
      ));
            // MakeBucketAsync in newer Minio versions returns Task only
            _minioClientMock
                .Setup(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

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
            var document = CreateTestUploadDocument();
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _repository.Add(document);

            _minioClientMock.Verify(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Never);
            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Add_BucketDoesNotExist_CreatesBucket()
        {
            var document = CreateTestUploadDocument();
            _minioClientMock
                .Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await _repository.Add(document);

            _minioClientMock.Verify(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Add_UploadsDocumentSuccessfully()
        {
            var document = CreateTestUploadDocument();

            await _repository.Add(document);

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
            return new UploadDocument
            {
                Id = 12345L,
                File = new byte[] { 0x01, 0x02, 0x03 }
            };
        }
    }
}
