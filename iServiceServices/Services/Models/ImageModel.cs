using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace iServiceServices.Services.Models
{
    public class ImageModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
