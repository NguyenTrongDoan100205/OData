using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ODataEx.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
 public string Title { get; set; }
        [Required]
        [StringLength(50)]
        public string Author { get; set; }
        [Range(0.01, 9999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}