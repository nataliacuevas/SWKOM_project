using sws.DAL.Entities;
using System;

namespace sws.DAL.Repositories
{
    public interface IDocumentRepository
    {
        UploadDocument Add(UploadDocument document);
    }
}
