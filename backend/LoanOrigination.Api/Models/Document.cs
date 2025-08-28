using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanOrigination.Api.Models
{
    [Table("documents")]
    public class Document
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("loan_application_id")]
        public int LoanApplicationId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("type")]
        public string Type { get; set; } = "Other"; // ID, Paystub, BankStatement, Other

        [Column("file_size_bytes")]
        public long FileSizeBytes { get; set; }

        [Column("mime_type")]
        [StringLength(100)]
        public string MimeType { get; set; } = string.Empty;

        [Column("extracted_data_json", TypeName = "jsonb")]
        public string ExtractedDataJson { get; set; } = "{}";

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Column("verified_by")]
        public int? VerifiedById { get; set; }

        [Column("verified_at")]
        public DateTime? VerifiedAt { get; set; }

        // Navigation properties
        [ForeignKey("LoanApplicationId")]
        public virtual LoanApplication LoanApplication { get; set; } = null!;

        [ForeignKey("VerifiedById")]
        public virtual User? VerifiedBy { get; set; }
    }
}
