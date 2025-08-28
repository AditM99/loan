using Microsoft.ML;
using Microsoft.ML.Data;

namespace LoanOrigination.Api.Services
{
    public class AiEligibilityService
    {
        private readonly MLContext _ml;
        private ITransformer? _model;
        private PredictionEngine<LoanRow, LoanPrediction>? _engine;
        private bool _isInitialized = false;

        public AiEligibilityService()
        {
            _ml = new MLContext();
            try
            {
                var trainingData = _ml.Data.LoadFromEnumerable(GenerateSyntheticData());

                var pipeline = _ml.Transforms.Concatenate("Features",
                                                          nameof(LoanRow.IncomeMonthly),
                                                          nameof(LoanRow.DebtMonthly),
                                                          nameof(LoanRow.CreditScore),
                                                          nameof(LoanRow.Amount),
                                                          nameof(LoanRow.TermMonths),
                                                          nameof(LoanRow.LoanTypeId))
                               .Append(_ml.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

                _model = pipeline.Fit(trainingData);
                _engine = _ml.Model.CreatePredictionEngine<LoanRow, LoanPrediction>(_model);
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the application
                Console.WriteLine($"AI Service initialization failed: {ex.Message}");
                _isInitialized = false;
            }
        }

        public (float probability, string explanation) Predict(decimal incomeMonthly, decimal debtMonthly, int creditScore, decimal amount, int termMonths, string loanType)
        {
            if (!_isInitialized || _engine == null)
            {
                // Fallback to rule-based prediction if AI model is not available
                return PredictFallback(incomeMonthly, debtMonthly, creditScore, amount, termMonths, loanType);
            }

            try
            {
                var row = new LoanRow
                {
                    IncomeMonthly = (float)incomeMonthly,
                    DebtMonthly = (float)debtMonthly,
                    CreditScore = (float)creditScore,
                    Amount = (float)amount,
                    TermMonths = (float)termMonths,
                    LoanTypeId = loanType switch
                    {
                        "Auto" => 1f,
                        "Mortgage" => 2f,
                        _ => 0f
                    },
                    Label = false
                };

                var pred = _engine.Predict(row);
                return (pred.Probability, GenerateExplanation(incomeMonthly, debtMonthly, creditScore, amount, termMonths, loanType));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI prediction failed: {ex.Message}");
                return PredictFallback(incomeMonthly, debtMonthly, creditScore, amount, termMonths, loanType);
            }
        }

        private (float probability, string explanation) PredictFallback(decimal incomeMonthly, decimal debtMonthly, int creditScore, decimal amount, int termMonths, string loanType)
        {
            var dti = (double)(debtMonthly / Math.Max(1, incomeMonthly));
            var probability = 0.5f; // Default probability

            if (creditScore >= 700 && dti < 0.3) probability = 0.9f;
            else if (creditScore >= 650 && dti < 0.4) probability = 0.7f;
            else if (creditScore >= 600 && dti < 0.45) probability = 0.5f;
            else probability = 0.2f;

            return (probability, GenerateExplanation(incomeMonthly, debtMonthly, creditScore, amount, termMonths, loanType));
        }

        private string GenerateExplanation(decimal incomeMonthly, decimal debtMonthly, int creditScore, decimal amount, int termMonths, string loanType)
        {
            var reasons = new List<string>();
            var dti = (double)(debtMonthly / Math.Max(1, incomeMonthly));
            if (dti > 0.45) reasons.Add("High DTI ratio"); 
            if (creditScore < 640) reasons.Add("Low credit score"); 
            if (amount > incomeMonthly * 20) reasons.Add("Loan amount high vs income"); 
            if (termMonths > 84 && loanType == "Auto") reasons.Add("Long term for auto loan");

            if (reasons.Count == 0) reasons.Add("Healthy DTI and credit mix");

            return string.Join("; ", reasons);
        }

        private IEnumerable<LoanRow> GenerateSyntheticData()
        {
            var rand = new Random(123);
            for (int i = 0; i < 2000; i++)
            {
                var income = rand.Next(2500, 15000);
                var debt = rand.Next(200, 7000);
                var credit = rand.Next(500, 800);
                var amount = rand.Next(2000, 60000);
                var term = rand.Next(12, 84);
                var typeId = rand.Next(0, 3);

                var dti = (float)debt / Math.Max(1, (float)income);
                bool approved = credit >= 660 && dti < 0.42f && amount < income * 15;

                yield return new LoanRow
                {
                    IncomeMonthly = (float)income,
                    DebtMonthly = (float)debt,
                    CreditScore = (float)credit,
                    Amount = (float)amount,
                    TermMonths = (float)term,
                    LoanTypeId = (float)typeId,
                    Label = approved
                };
            }
        }

        public class LoanRow
        {
            public float IncomeMonthly { get; set; }
            public float DebtMonthly { get; set; }
            public float CreditScore { get; set; }
            public float Amount { get; set; }
            public float TermMonths { get; set; }
            public float LoanTypeId { get; set; }
            public bool Label { get; set; }
        }

        public class LoanPrediction
        {
            [ColumnName("PredictedLabel")] public bool Approved { get; set; }
            public float Probability { get; set; }
            public float Score { get; set; }
        }
    }
}
