using Microsoft.AspNetCore.Mvc;
using MediCore.Data;
using MediCore.Models;

namespace MediCore.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly MediCoreDb _db;
    public PatientsController(MediCoreDb db) => _db = db;

    // ── Auth guard helper ─────────────────────────────────────────────────
    private string? Role => HttpContext.Session.GetString("role");
    private bool IsStaff  => Role == "admin" || Role == "doctor";
    private bool IsAdmin  => Role == "admin";

    // GET /api/patients — all patients (admin & doctor only)
    [HttpGet]
    public IActionResult GetAll()
    {
        if (!IsStaff) return Unauthorized(new { error = "Access denied." });
        return Ok(_db.GetPatients());
    }

    // GET /api/patients/{id} — single patient
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        if (Role == null) return Unauthorized();
        // Patients can only see themselves
        if (Role == "patient" && HttpContext.Session.GetString("patientId") != id.ToUpper())
            return Forbid();

        var patient = _db.GetPatient(id);
        if (patient == null) return NotFound(new { error = "Patient not found." });
        return Ok(patient);
    }

    // GET /api/patients/{id}/records — visit records for a patient
    [HttpGet("{id}/records")]
    public IActionResult GetRecords(string id)
    {
        if (Role == null) return Unauthorized();
        if (Role == "patient" && HttpContext.Session.GetString("patientId") != id.ToUpper())
            return Forbid();

        var patient = _db.GetPatient(id);
        if (patient == null) return NotFound(new { error = "Patient not found." });

        return Ok(_db.GetRecordsForPatient(id));
    }

    // POST /api/patients — register new patient (admin only)
    [HttpPost]
    public IActionResult Register([FromBody] RegisterPatientRequest req)
    {
        if (!IsAdmin) return Unauthorized(new { error = "Only admins can register patients." });

        if (string.IsNullOrWhiteSpace(req.Name) || req.Age <= 0 || string.IsNullOrWhiteSpace(req.Gender))
            return BadRequest(new { error = "Name, Age, and Gender are required." });

        var patient = _db.AddPatient(req);
        return Ok(patient);
    }

    // GET /api/patients/next-id — generate next patient ID
    [HttpGet("next-id")]
    public IActionResult NextId()
    {
        if (!IsAdmin) return Unauthorized();
        return Ok(new { id = _db.NextPatientId() });
    }
}
