using System.ComponentModel.DataAnnotations;

namespace Jwt.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}
