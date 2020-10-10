using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Skeptical.Beavers.Backend.Model
{
    public sealed class LoginRequest
    {
        [Required]
        [DefaultValue("admin")]
        public string UserName { get; set; }

        [Required]
        [DefaultValue("admin")]
        public string Password { get; set; }
    }
}