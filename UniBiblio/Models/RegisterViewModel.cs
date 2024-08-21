using System.ComponentModel.DataAnnotations;

namespace UniBiblio.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cognome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "La password e la conferma password non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }
}
