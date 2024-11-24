using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
    internal interface IMinioRepository
    {
        Task<MemoryStream> Get(string filename);
    }
}
