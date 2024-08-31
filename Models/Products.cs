// Models/Customer.cs

using System.ComponentModel.DataAnnotations;

namespace DotnetWebApiWithEFCodeFirst.Models
{
    public class Products
    {
        [Key]
        public long id { get; set; }

        [Required]
        [MaxLength(255)]
        public string name { get; set; }

        [Required]
        [MaxLength(255)]
        public string description { get; set; }

        [Required]
        public decimal price { get; set; }

        [Required]
        public int stock { get; set; }

        public string image { get; set; }
    }
}