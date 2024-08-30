// Models/Customer.cs

using System.ComponentModel.DataAnnotations;

namespace DotnetWebApiWithEFCodeFirst.Models
{
    public class Coupons
    {
        [Key]
        public long id { get; set; }

        [Required]
        [MaxLength(255)]
        public string code { get; set; }

        [Required]
        public DateTimeOffset expires_at { get; set; }

        [Required]
        public int usage_count { get; set; }
    }
}