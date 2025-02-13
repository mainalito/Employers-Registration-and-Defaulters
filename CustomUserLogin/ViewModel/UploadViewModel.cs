
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace CustomUserLogin.ViewModel
{
  

    public class UploadViewModel
    {
        [Required(ErrorMessage = "Please select an Excel file.")]
 
        public IFormFile File { get; set; }
    }

 
}
