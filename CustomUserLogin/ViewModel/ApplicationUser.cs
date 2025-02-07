using CustomUserLogin.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomUserLogin.ViewModel
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }

        
        public int EntityRoleTypeId { get; set; }

        public EntityRoleType EntityRoleType { get; set; }

    }



}
