
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _3___Data.Models
{
    public class CraftingStatsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdMaterial { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("IdMaterial")]
        public MaterialModel Material { get; set; }
    }
}