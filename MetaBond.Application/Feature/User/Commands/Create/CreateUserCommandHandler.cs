using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.Create;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<CreateUserCommandHandler> logger,
    ICloudinaryService cloudinaryService,
    IEmailService emailService,
    IRoleService roleService,
    IUserInterestRepository userInterestRepository,
    IEmailConfirmationTokenService emailConfirmationTokenService
) :
    ICommandHandler<CreateUserCommand, UserDTos>
{
    public async Task<ResultT<UserDTos>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        string imageUrl = "";
        if (request.ImageFile != null)
        {
            await using var stream = request.ImageFile.OpenReadStream();
            imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);
        }

        var userWithEmail = await userRepository.EmailExistsAsync(request.Email!, cancellationToken);
        if (userWithEmail)
        {
            logger.LogWarning("Email {Email} already exists.", request.Email!);

            return ResultT<UserDTos>.Failure(Error.Conflict("409", "Email already exists"));
        }

        var userWithUsername = await userRepository.UsernameExistsAsync(request.Username!, cancellationToken);
        if (userWithUsername)
        {
            logger.LogWarning("Username {Username} already exists.", request.Username!);

            return ResultT<UserDTos>.Failure(Error.Conflict("409", "Username already exists"));
        }

        var defaultRole = await roleService.GetRoleByNameAsync(UserRoles.User.ToString(), cancellationToken);

        if (!defaultRole.IsSuccess)
        {
            logger.LogError("Failed to get role {RoleName}", UserRoles.User.ToString());

            return ResultT<UserDTos>.Failure(defaultRole.Error!);
        }

        // Validate InterestsIds is not null or empty (optional, business rule)
        if (!request.InterestsIds.Any())
        {
            logger.LogWarning("No interests provided for user {Email}", request.Email);

            return ResultT<UserDTos>.Failure(Error.Failure("400", "At least one interest must be selected"));
        }

        Domain.Models.User user = new()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Email = request.Email,
            Photo = imageUrl,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = defaultRole.Value.RolesId
        };

        await userRepository.CreateAsync(user, cancellationToken);

        await userInterestRepository.AssociateInterestsToUserAsync(user.Id, request.InterestsIds, cancellationToken);

        logger.LogInformation("User {UserId} created successfully", user.Id);

        var token = await emailConfirmationTokenService.GenerateTokenAsync(user.Id, cancellationToken);

        if (!token.IsSuccess)
        {
            logger.LogError("Failed to generate token for user {UserId}", user.Id);

            return token.Error!;
        }

        string code = token.Value;

        await emailService.SendEmailAsync(new EmailRequestDTo(
            To: request.Email,
            Body: EmailTemplates.ConfirmAccountEmailHtml(code),
            Subject: "Confirm Account"
        ));

        var userDTos = UserMapper.MapUserDTos(user);

        return ResultT<UserDTos>.Success(userDTos);
    }
}