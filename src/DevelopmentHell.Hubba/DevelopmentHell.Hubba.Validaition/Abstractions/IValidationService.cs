﻿using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Validation.Service.Abstractions
{
    public interface IValidationService
    {
        Result ValidateEmail(string email);
        Result ValidatePassword(string password);
        Result ValidatePhoneNumber(string phoneNumber);
        Result ValidateCollaboratorAllowEmptyFiles(CollaboratorProfile collab);
        Result ValidateCollaborator(CollaboratorProfile collab);
        Result ValidateImageFile(IFormFile file);
    }
}
