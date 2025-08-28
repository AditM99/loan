using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanOrigination.Api.Data;
using LoanOrigination.Api.DTOs;
using LoanOrigination.Api.Models;
using LoanOrigination.Api.Services;
using System.Security.Claims;

namespace LoanOrigination.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AiEligibilityService _ai;
        private readonly IWebHostEnvironment _env;
        private readonly DocumentExtractionService _extractor;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(AppDbContext db, AiEligibilityService ai, IWebHostEnvironment env, DocumentExtractionService extractor, ILogger<ApplicationsController> logger)
        {
            _db = db;
            _ai = ai;
            _env = env;
            _extractor = extractor;
            _logger = logger;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<LoanApplicationResponse>> Create(CreateLoanRequest req)
        {
            var uid = GetUserId();
            _logger.LogInformation("Creating application for user {UserId}", uid);
            
            var app = new LoanApplication
            {
                UserId = uid,
                LoanType = req.LoanType,
                Amount = req.Amount,
                TermMonths = req.TermMonths,
                IncomeMonthly = req.IncomeMonthly,
                DebtMonthly = req.DebtMonthly,
                CreditScore = req.CreditScore,
                Status = "Pending"
            };
            _db.Applications.Add(app);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created application {AppId} for user {UserId}", app.Id, uid);
            
            return CreatedAtAction(nameof(GetById), new { id = app.Id }, MapToResponse(app));
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoanApplicationResponse>> GetById(int id)
        {
            var app = await _db.Applications
                .Include(a => a.Prediction)
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();
            return MapToResponse(app);
        }

        [Authorize]
        [HttpGet("user/mine")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponse>>> Mine()
        {
            var uid = GetUserId();
            _logger.LogInformation("Fetching applications for user {UserId}", uid);
            
            var apps = await _db.Applications
                .Where(a => a.UserId == uid)
                .Include(a => a.Prediction)
                .ToListAsync();
                
            _logger.LogInformation("Found {Count} applications for user {UserId}", apps.Count, uid);
            return apps.Select(MapToResponse).ToList();
        }

        [Authorize]
        [HttpPost("{id:int}/predict")]
        public async Task<ActionResult<PredictionResponse>> Predict(int id)
        {
            var app = await _db.Applications.FindAsync(id);
            if (app == null) return NotFound();

            var (prob, reason) = _ai.Predict(app.IncomeMonthly, app.DebtMonthly, app.CreditScore, app.Amount, app.TermMonths, app.LoanType);
            var pred = new Prediction
            {
                LoanApplicationId = app.Id,
                ApprovalProbability = prob,
                Explanation = reason
            };

            _db.Predictions.Add(pred);
            await _db.SaveChangesAsync();

            return new PredictionResponse(
                pred.Id,
                pred.ApprovalProbability,
                pred.Explanation,
                pred.ModelVersion,
                pred.ConfidenceScore,
                pred.CreatedAt
            );
        }

        [Authorize]
        [HttpPost("{id:int}/upload")]
        public async Task<ActionResult<UploadDocumentResponse>> Upload(int id, IFormFile file, [FromQuery] string type = "Other")
        {
            var app = await _db.Applications.FindAsync(id);
            if (app == null) return NotFound();

            var uploads = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            Directory.CreateDirectory(uploads);

            var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var path = Path.Combine(uploads, safeName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var extracted = _extractor.ExtractFromFileName(file.FileName);
            var doc = new Document
            {
                LoanApplicationId = app.Id,
                FileName = file.FileName,
                Url = $"/uploads/{safeName}",
                Type = type,
                ExtractedDataJson = System.Text.Json.JsonSerializer.Serialize(extracted)
            };
            _db.Documents.Add(doc);
            await _db.SaveChangesAsync();

            return new UploadDocumentResponse(doc.Id, doc.Url, doc.Type, doc.ExtractedDataJson);
        }

        private static LoanApplicationResponse MapToResponse(LoanApplication app)
        {
            return new LoanApplicationResponse(
                app.Id,
                app.LoanType,
                app.Amount,
                app.TermMonths,
                app.IncomeMonthly,
                app.DebtMonthly,
                app.CreditScore,
                app.Status,
                app.CreatedAt,
                app.UpdatedAt,
                app.Prediction != null ? new PredictionResponse(
                    app.Prediction.Id,
                    app.Prediction.ApprovalProbability,
                    app.Prediction.Explanation,
                    app.Prediction.ModelVersion,
                    app.Prediction.ConfidenceScore,
                    app.Prediction.CreatedAt
                ) : null
            );
        }
    }
}
