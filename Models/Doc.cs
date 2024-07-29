using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notebad.Models
{
    public class Doc
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }
        

    }
}
