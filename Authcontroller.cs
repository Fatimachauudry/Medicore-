using Microsoft.AspNetCore.Mvc;
using MediCore.Data;
using MediCore.Models;

namespace MediCore.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MediCoreDb _db;
    public AuthController(MediCoreDb db) => _db = db;

    // POST /api/auth/login  — staff login (admin / doctor)
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "Please fill in all fields." });

        var (ok, role, name, specialty) = _db.ValidateStaff(req.Username.Trim(), req.Password.Trim(), req.Role.Trim());

        if (!ok)
            return Unauthorized(new { error = "Invalid username or password." });

        // Store session server-side
        HttpContext.Session.SetString("username", req.Username.Trim());
        HttpContext.Session.SetString("role",     role);
        HttpContext.Session.SetString("name",     name);
        HttpContext.Session.SetString("specialty",specialty);

        return Ok(new { role, name, specialty });
    }

    // POST /api/auth/patient-login  — patient login by ID
    [HttpPost("patient-login")]
    public IActionResult PatientLogin([FromBody] PatientLoginRequest req)
    {
        var pid = req.PatientId.Trim().ToUpper();
        var patient = _db.GetPatient(pid);
        if (patient == null)
            return Unauthorized(new { error = "Patient ID not found. Please check and try again." });

        HttpContext.Session.SetString("username", pid);
        HttpContext.Session.SetString("role",     "patient");
        HttpContext.Session.SetString("name",     patient.Name);
        HttpContext.Session.SetString("specialty","");
        HttpContext.Session.SetString("patientId", pid);

        return Ok(new { role = "patient", name = patient.Name, specialty = "" });
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok();
    }

    // GET /api/auth/me — check who is logged in
    [HttpGet("me")]
    public IActionResult Me()
    {
        var role = HttpContext.Session.GetString("role");
        if (role == null) return Unauthorized();
        return Ok(new {
            role,
            name      = HttpContext.Session.GetString("name"),
            specialty = HttpContext.Session.GetString("specialty"),
            patientId = HttpContext.Session.GetString("patientId"),
        });
    }
}
