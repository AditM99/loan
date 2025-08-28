using System.Text.RegularExpressions;

namespace LoanOrigination.Api.Services
{
    public class DocumentExtractionService
    {
        // Placeholder extractor that attempts to parse income and name from filename
        // e.g., "paystub_income_7500_name_John_Doe_2025-08.pdf"
        public Dictionary<string, string> ExtractFromFileName(string fileName)
        {
            var result = new Dictionary<string, string>();

            var incomeMatch = Regex.Match(fileName, @"income[_\-](\d{3,6})", RegexOptions.IgnoreCase);
            if (incomeMatch.Success) result["incomeMonthly"] = incomeMatch.Groups[1].Value;

            var nameMatch = Regex.Match(fileName, @"name[_\-]([A-Za-z]+)[_\-]([A-Za-z]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success) result["name"] = $"{nameMatch.Groups[1].Value} {nameMatch.Groups[2].Value}";

            return result;
        }
    }
}
