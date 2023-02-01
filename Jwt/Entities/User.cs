using System.ComponentModel.DataAnnotations;

namespace Jwt.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
