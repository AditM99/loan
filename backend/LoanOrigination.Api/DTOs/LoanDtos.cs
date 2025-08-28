namespace LoanOrigination.Api.DTOs
{
    public record CreateLoanRequest(
        string LoanType,
        decimal Amount,
        int TermMonths,
        decimal IncomeMonthly,
        decimal DebtMonthly,
        int CreditScore
    );

    public record UploadDocumentResponse(int Id, string Url, string Type, string ExtractedDataJson);

    public record LoanApplicationResponse(
        int Id,
        string LoanType,
        decimal Amount,
        int TermMonths,
        decimal IncomeMonthly,
        decimal DebtMonthly,
        int CreditScore,
        string Status,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        PredictionResponse? Prediction
    );

    public record PredictionResponse(
        int Id,
        float ApprovalProbability,
        string Explanation,
        string ModelVersion,
        float ConfidenceScore,
        DateTime CreatedAt
    );
}
