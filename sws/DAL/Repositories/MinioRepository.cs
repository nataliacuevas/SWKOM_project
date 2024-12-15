﻿using log4net;
using Microsoft.Build.Framework;
using Minio;
using Minio.DataModel.Args;
using sws.DAL;
using sws.DAL.Entities;

namespace sws.DAL.Repositories
{
    public class MinioRepository : IMinioRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MinioRepository));
        private readonly IMinioClient _minioClient;
        private const string BucketName = "uploads";
        public MinioRepository() 
        {
            _minioClient = new MinioClient()
                .WithEndpoint("minio", 9000)
                .WithCredentials("minioadmin", "minioadmin")
                .WithSSL(false)
                .Build();
        }
        //for testing
        public MinioRepository(IMinioClient minioClient)
        {
            _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));
        }
     
        public async Task Add(UploadDocument document)
        {
            try
            {
                await EnsureBucketExists();

                var fileName = document.Id.ToString();
                using var fileStream = new MemoryStream(document.File); //minIO requires a stream

                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(fileName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(document.File.Length));

                _log.Info($"Document {fileName} added to MinIO!");
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error adding document to MinIO.", ex);
            }
        }


        private async Task EnsureBucketExists()
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));
            if (!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName));
            }
        }

    }
}
