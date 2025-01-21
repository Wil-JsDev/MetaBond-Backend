using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Service
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageCloudinaryAsync(Stream fileStream, string imageName, CancellationToken cancellationToken);
    }
}
