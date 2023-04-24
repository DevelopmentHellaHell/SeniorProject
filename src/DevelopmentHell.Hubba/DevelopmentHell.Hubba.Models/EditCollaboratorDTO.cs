using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.WebAPI.DTO.Collaborator
{
    public class EditCollaboratorDTO
    {
        public string? Name { get; set; }
        public string? ContactInfo { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? Availability { get; set; }
        public bool Published { get; set; }
        public IFormFile? PfpFile { get; set; }
        public IFormFile[]? UploadedFiles { get; set; }
        public string[]? RemovedFiles { get; set; }
    }
}
