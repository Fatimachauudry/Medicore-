using Microsoft.AspNetCore.Mvc;
using MediCore.Data;
using MediCore.Models;

namespace MediCore.Controllers;

[ApiController]
[Route("api/records")]
public class RecordsController : ControllerBase
{
    private readonly MediCoreDb _db;
    public RecordsController(MediCoreDb db) => _db = db;

    private string? Role => HttpContext.Session.GetString("role");
    private bool IsStaff  => Role == "admin" || Role == "doctor";

    // GET /api/records — all records (staff only)
    [HttpGet]
    public IActionResult GetAll()
    {
        if (!IsStaff) return Unauthorized(new { error = "Access denied." });
        return Ok(_db.GetRecords());
    }

    // POST /api/records — add visit record (admin or doctor only)
    [HttpPost]
    public IActionResult Add([FromBody] AddRecordRequest req)
    {
        if (!IsStaff) return Unauthorized(new { error = "Only staff can add records." });

        if (string.IsNullOrWhiteSpace(req.Date) || string.IsNullOrWhiteSpace(req.Diagnosis) || string.IsNullOrWhiteSpace(req.Doctor))
            return BadRequest(new { error = "Date, Diagnosis, and Doctor Name are required." });

        var patient = _db.GetPatient(req.PatientId);
        if (patient == null) return NotFound(new { error = "Patient not found." });

        var record = _db.AddRecord(req);
        return Ok(record);
    }
}
