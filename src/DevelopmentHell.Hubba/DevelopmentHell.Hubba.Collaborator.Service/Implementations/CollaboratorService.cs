using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Collaborator.Service.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private IValidationService _validationService;

        public CollaboratorService(IValidationService validationService)
        {
            _validationService = validationService;
        }

        public async Task<Result> CreateCollaborator(CollaboratorProfile collab)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            if (collab == null)
            {
                result.ErrorMessage = "Error, collab is null.";
                return result;
            }
            var validateCollab = _validationService.ValidateCollaborator(collab);
            if (validateCollab == null)
            {
                result.ErrorMessage = "Unable to validate collaborator with files.";
                return result;
            }
            if (!validateCollab.IsSuccessful)
            {
                return validateCollab;
            }

            throw new NotImplementedException();
        }

        public async Task<Result> DeleteCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteCollaboratorWithAccountId(int accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> EditCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<CollaboratorProfile>> GetCollaborator(int collabId)
        {
            throw new NotImplementedException();
        }
        public async Task<Result> ChangeVisibility(int collabId, bool isPublic)
        {
            throw new NotImplementedException();
        }
    }
}
