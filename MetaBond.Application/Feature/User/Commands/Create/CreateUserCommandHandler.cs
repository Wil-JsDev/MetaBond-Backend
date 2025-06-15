using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.Create;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<CreateUserCommandHandler> logger,
    ICloudinaryService cloudinaryService,
    IEmailService emailService,
    IEmailConfirmationTokenService emailConfirmationTokenService
    ) :  
    ICommandHandler<CreateUserCommand, UserDTos>
{
    public async Task<ResultT<UserDTos>> Handle(
        CreateUserCommand request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            string imageUrl = "";
            if (request.ImageFile != null)
            {
                using var stream = request.ImageFile.OpenReadStream();
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
            
            Domain.Models.User user = new()
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Photo = imageUrl,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            
            await userRepository.CreateAsync(user, cancellationToken);

            var token = await emailConfirmationTokenService.GenerateTokenAsync(user.Id,cancellationToken);
            
            string code = token.Value;
            
            await emailService.SendEmailAsync(new EmailRequestDTo(
                To: request.Email,
                Body:EmailTemplates.ConfirmAccountEmailHtml(code),
                Subject: "Confirm Account"
            ));

            UserDTos userDTos = new(
               UserId:  user.Id,
               FirstName: user.FirstName,
               LastName: user.LastName,
               Username: user.Username,
               Photo:  user.Photo
            ); 
            
            return  ResultT<UserDTos>.Success(userDTos); 
        }
        
        logger.LogWarning("CreateUserCommand request was null.");
        
        return ResultT<UserDTos>.Failure(Error.Failure("400", "Invalid request: CreateUserCommand cannot be null."));
    }
}