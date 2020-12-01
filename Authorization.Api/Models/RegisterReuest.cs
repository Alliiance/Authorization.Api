using System.ComponentModel.DataAnnotations;

namespace Authorization.Api.Models
{
    public class RegisterReuest
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
