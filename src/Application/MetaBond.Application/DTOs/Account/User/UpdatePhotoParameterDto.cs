using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.DTOs.Account.User;

public sealed record UpdatePhotoParameterDto(
    IFormFile? Photo
);