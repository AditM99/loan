using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanOrigination.Api.Models
{
    [Table("predictions")]
    public class Prediction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("loan_application_id")]
        public int LoanApplicationId { get; set; }

        [Required]
        [Column("approval_probability")]
        [Range(0.0, 1.0)]
        public float ApprovalProbability { get; set; }

        [Required]
        [StringLength(2000)]
        [Column("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [Column("model_version")]
        [StringLength(50)]
        public string ModelVersion { get; set; } = "1.0";

        [Column("confidence_score")]
        [Range(0.0, 1.0)]
        public float ConfidenceScore { get; set; }

        [Column("features_used", TypeName = "jsonb")]
        public string FeaturesUsed { get; set; } = "{}";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; } = null!;
    }
}
