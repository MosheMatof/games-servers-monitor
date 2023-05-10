using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IGDB.Domain.Contracts
{
    public interface IDownloadService
    {
        Task<byte[]> DownloadImageAsync(string imageUrl);
    }
}
