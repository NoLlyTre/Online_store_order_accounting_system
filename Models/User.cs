using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderSystemEF.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

