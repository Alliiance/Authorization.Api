using System.ComponentModel.DataAnnotations;

namespace Authorization.Api.Models
{
    public class LoginReuest
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
