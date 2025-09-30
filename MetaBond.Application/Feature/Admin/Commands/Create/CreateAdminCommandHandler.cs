using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;
using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Admin.Commands.Create;

internal sealed class CreateAdminCommandHandler(
    IAdminRepository adminRepository,
    ILogger<CreateAdminCommandHandler> logger,
    ICloudinaryService cloudinaryService
) : ICommandHandler<CreateAdminCommand, AdminDto>
{
    public async Task<ResultT<AdminDto>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (request.ImageFile != null)
        {
            await using var stream = request.ImageFile.OpenReadStream();
            imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);
        }

        bool emailInUse = await adminRepository.ExistsEmailAsync(request.Email, cancellationToken);
        if (emailInUse)
        {
            logger.LogWarning("Email {Email} already exists.", request.Email);

            return ResultT<AdminDto>.Failure(Error.Conflict("409", $"Email '{request.Email}' already exists"));
        }

        bool usernameInUse = await adminRepository.ExistsUsernameAsync(request.Username, cancellationToken);
        if (usernameInUse)
        {
            logger.LogWarning("Username {Username} already exists.", request.Username);

            return ResultT<AdminDto>.Failure(Error.Conflict("409", $"Username '{request.Username}' already exists"));
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var admin = new Domain.Models.Admin
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Email = request.Email,
            Photo = imageUrl,
            Password = hashedPassword,
            IsEmailConfirmed = true
        };

        await adminRepository.CreateAsync(admin, cancellationToken);

        logger.LogInformation("Admin {AdminId} created with email {Email}.", admin.Id, admin.Email);

        return ResultT<AdminDto>.Success(AdminMapper.ToDTo(admin));
    }
}