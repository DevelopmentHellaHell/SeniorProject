using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.WebAPI.DTO.Collaborator;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollaboratorController : HubbaController
    {
        private readonly ICollaboratorManager _collaboratorManager;
        
        public CollaboratorController(ICollaboratorManager collaboratorManager)
        {
            _collaboratorManager = collaboratorManager;
        }

        [HttpPost]
        [Route("CreateCollaborator")]
        public async Task<IActionResult> CreateCollaborator([FromForm]CreateCollaboratorDTO createCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.CreateCollaborator(createCollaboratorDTO).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("EditCollaborator")]
        public async Task<IActionResult> EditCollaborator([FromForm] EditCollaboratorDTO editCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.EditCollaborator(editCollaboratorDTO).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("GetCollaborator")]
        public async Task<IActionResult> GetCollaborator(GetCollaboratorDTO getCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.GetCollaborator(getCollaboratorDTO.CollaboratorId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("DeleteCollaborator")]
        public async Task<IActionResult> DeleteCollaborator(DeleteCollaboratorIdDTO deleteCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.DeleteCollaborator(deleteCollaboratorDTO.CollaboratorId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("DeleteCollaboratorWithAccountId")]
        public async Task<IActionResult> DeleteCollaboratorWithAccountId(DeleteCollaboratorAccountIdDTO deleteCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.DeleteCollaboratorWithAccountId(deleteCollaboratorDTO.AccountId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("RemoveCollaborator")]
        public async Task<IActionResult> RemoveCollaborator(RemoveCollaboratorDTO removeCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.RemoveCollaborator(removeCollaboratorDTO.CollaboratorId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("VoteCollaborator")]
        public async Task<IActionResult> VoteCollaborator(VoteCollaboratorDTO voteCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.Vote(voteCollaboratorDTO.CollaboratorId, voteCollaboratorDTO.Upvote).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("HasCollaborator")]
        public async Task<IActionResult> HasCollaborator(HasCollaboratorDTO hasCollaboratorDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.HasCollaborator(hasCollaboratorDTO.AccountId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("ChangeCollaboratorVisibility")]
        public async Task<IActionResult> ChangeCollaboratorVisibility(CollaboratorVisibilityDTO collaboratorVisibilityDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.ChangeVisibility(collaboratorVisibilityDTO.CollaboratorId, collaboratorVisibilityDTO.IsPublic).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode);
            }).ConfigureAwait(false);
        }


        [HttpPost]
        [Route("GetFiles")]
        public async Task<IActionResult> GetFiles(GetFilesDTO getFilesDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _collaboratorManager.GetFileUrls(getFilesDTO.CollaboratorId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

    }
}
