using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanOrigination.Api.Data;

namespace LoanOrigination.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) { _db = db; }

        [Authorize(Roles = "Admin")]
        [HttpGet("applications")]
        public async Task<IActionResult> AllApplications()
        {
            var apps = await _db.Applications
                .Include(a => a.Prediction)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return Ok(apps);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("applications/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var app = await _db.Applications.FindAsync(id);
            if (app == null) return NotFound();
            app.Status = status;
            await _db.SaveChangesAsync();
            return Ok(app);
        }
    }
}
