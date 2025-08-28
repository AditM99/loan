using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanOrigination.Api.Models
{
    [Table("loan_applications")]
    public class LoanApplication
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("loan_type")]
        public string LoanType { get; set; } = "Personal"; // Personal, Auto, Mortgage

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        [Range(100, 10000000)]
        public decimal Amount { get; set; }

        [Required]
        [Column("term_months")]
        [Range(1, 360)]
        public int TermMonths { get; set; }

        [Required]
        [Column("income_monthly", TypeName = "decimal(18,2)")]
        [Range(0, 1000000)]
        public decimal IncomeMonthly { get; set; }

        [Required]
        [Column("debt_monthly", TypeName = "decimal(18,2)")]
        [Range(0, 1000000)]
        public decimal DebtMonthly { get; set; }

        [Required]
        [Column("credit_score")]
        [Range(300, 850)]
        public int CreditScore { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, UnderReview, Approved, Rejected

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        [Column("reviewed_by")]
        public int? ReviewedById { get; set; }

        [StringLength(1000)]
        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ReviewedById")]
        public virtual User? ReviewedBy { get; set; }

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual Prediction? Prediction { get; set; }
    }
}
