using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAgent.Contracts
{
    [ServiceContract]
    public interface IDownloadService : IDisposable, IHostedService
    {
        [OperationContract]
        Task<byte[]> DownloadImageAsync(string imageUrl);
    }
}
