using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Domain.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Infrastructure.Shared.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private CloudinarySettings _cloudinary { get; }

        public CloudinaryService(IOptions<CloudinarySettings> cloudinary)
        {
            _cloudinary = cloudinary.Value;
        }

        public async Task<string> UploadImageCloudinaryAsync(
            Stream fileStream,
            string imageName,
            CancellationToken cancellationToken)
        {
            Cloudinary cloudinary = new (_cloudinary.CloudinaryUrl);
            ImageUploadParams image = new()
            {
                File = new FileDescription(imageName, fileStream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };
            
            var uploadResult = await cloudinary.UploadAsync(image,cancellationToken);
            return uploadResult.SecureUrl.ToString();
        }

    }
}
