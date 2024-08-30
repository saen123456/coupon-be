// Models/Customer.cs

using System.ComponentModel.DataAnnotations;

namespace DotnetWebApiWithEFCodeFirst.Models
{
    public class Users
    {
        [Key]
        public long id { get; set; }

        [Required]
        [MaxLength(255)]
        public string username { get; set; }

        [Required]
        [MaxLength(255)]
        public string email { get; set; }

        [Required]
        [MaxLength(255)]
        public string password { get; set; }

        [Required]
        public bool is_admin { get; set; }
    }
}