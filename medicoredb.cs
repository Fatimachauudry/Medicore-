using MediCore.Models;

namespace MediCore.Data;

/// <summary>
/// In-memory store — swap this with EF Core / SQL Server for production.
/// </summary>
public class MediCoreDb
{
    private readonly List<Patient> _patients = new();
    private readonly List<VisitRecord> _records = new();

    // ── STAFF CREDENTIALS (never exposed to the frontend) ────────────────
    //    Change these passwords before deploying!
    private readonly Dictionary<string, (string Password, string Role, string Name, string Specialty)> _staff = new()
    {
        ["admin"]   = ("admin123",  "admin",  "Admin User",       "Administration"),
        ["doctor"]  = ("doc123",    "doctor", "Dr. Imran Khan",   "General Physician"),
        ["doctor2"] = ("doc456",    "doctor", "Dr. Fareeha Shah", "Internal Medicine"),
    };
    // ─────────────────────────────────────────────────────────────────────

    public MediCoreDb() => Seed();

    // ── AUTH ──────────────────────────────────────────────────────────────
    public (bool ok, string role, string name, string specialty) ValidateStaff(string username, string password, string role)
    {
        if (_staff.TryGetValue(username.ToLower(), out var s) && s.Password == password && s.Role == role)
            return (true, s.Role, s.Name, s.Specialty);
        return (false, "", "", "");
    }

    // ── PATIENTS ──────────────────────────────────────────────────────────
    public List<Patient> GetPatients() => _patients;

    public Patient? GetPatient(string id) =>
        _patients.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public string NextPatientId() =>
        $"MC-2026-{(_patients.Count + 1):D3}";

    public Patient AddPatient(RegisterPatientRequest req)
    {
        var p = new Patient
        {
            Id        = NextPatientId(),
            Name      = req.Name,
            Age       = req.Age,
            Gender    = req.Gender,
            Blood     = string.IsNullOrWhiteSpace(req.Blood)     ? "N/A" : req.Blood,
            Phone     = string.IsNullOrWhiteSpace(req.Phone)     ? "N/A" : req.Phone,
            Email     = string.IsNullOrWhiteSpace(req.Email)     ? "N/A" : req.Email,
            Address   = string.IsNullOrWhiteSpace(req.Address)   ? "N/A" : req.Address,
            Emergency = string.IsNullOrWhiteSpace(req.Emergency) ? "N/A" : req.Emergency,
            Joined    = string.IsNullOrWhiteSpace(req.Joined)    ? DateTime.Today.ToString("yyyy-MM-dd") : req.Joined,
        };
        _patients.Add(p);
        return p;
    }

    // ── RECORDS ───────────────────────────────────────────────────────────
    public List<VisitRecord> GetRecords() => _records;

    public List<VisitRecord> GetRecordsForPatient(string patientId) =>
        _records.Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.Date)
                .ToList();

    public VisitRecord AddRecord(AddRecordRequest req)
    {
        var r = new VisitRecord
        {
            Id        = "R" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            PatientId = req.PatientId,
            Date      = req.Date,
            Diagnosis = req.Diagnosis,
            Notes     = req.Notes,
            Meds      = req.Meds,
            Doctor    = req.Doctor,
            Followup  = req.Followup,
            Bp        = req.Bp,
            Temp      = req.Temp,
            Weight    = req.Weight,
        };
        _records.Add(r);
        return r;
    }

    // ── SEED DATA ─────────────────────────────────────────────────────────
    private void Seed()
    {
        _patients.AddRange(new[]
        {
            new Patient { Id="MC-2026-001", Name="Ahmed Raza",    Age=34, Gender="Male",   Phone="0301-1234567", Blood="B+", Address="House 12, Block A, Lahore",    Joined="2024-01-15", Email="ahmed@email.com",  Emergency="0321-111222" },
            new Patient { Id="MC-2026-002", Name="Sana Malik",    Age=28, Gender="Female", Phone="0311-9876543", Blood="O+", Address="Flat 5, DHA Phase 4, Lahore",  Joined="2024-02-20", Email="sana@email.com",   Emergency="0302-334455" },
            new Patient { Id="MC-2026-003", Name="Tariq Hassan",  Age=52, Gender="Male",   Phone="0321-4567890", Blood="A-", Address="Street 7, Gulberg III, Lahore",Joined="2024-03-05", Email="tariq@email.com",  Emergency="0345-667788" },
        });

        _records.AddRange(new[]
        {
            new VisitRecord { Id="R001", PatientId="MC-2026-001", Date="2024-01-15", Diagnosis="Seasonal Flu",           Notes="Fever 38.5°C, sore throat, runny nose. Rest and fluids advised.", Meds=new(){"Paracetamol 500mg","Antihistamine"},        Doctor="Dr. Imran Khan",   Followup="2024-01-22", Bp="118/76", Temp="38.5°C", Weight="70kg" },
            new VisitRecord { Id="R002", PatientId="MC-2026-001", Date="2024-03-10", Diagnosis="Hypertension Follow-Up", Notes="BP 145/92. Controlled with medication.",                          Meds=new(){"Amlodipine 5mg","Losartan 50mg"},           Doctor="Dr. Imran Khan",   Followup="2024-04-10", Bp="145/92", Temp="36.8°C", Weight="71kg" },
            new VisitRecord { Id="R003", PatientId="MC-2026-002", Date="2024-02-20", Diagnosis="Iron Deficiency Anemia", Notes="Hemoglobin 9.2g/dL. Fatigue and dizziness reported.",            Meds=new(){"Iron Supplement 150mg","Vitamin C 500mg"}, Doctor="Dr. Fareeha Shah", Followup="2024-03-20", Bp="110/70", Temp="36.5°C", Weight="58kg" },
            new VisitRecord { Id="R004", PatientId="MC-2026-003", Date="2024-03-05", Diagnosis="Type 2 Diabetes",        Notes="HbA1c 8.2%. Fasting glucose 165mg/dL.",                         Meds=new(){"Metformin 500mg","Glucophage XR"},          Doctor="Dr. Imran Khan",   Followup="2024-04-05", Bp="130/85", Temp="36.9°C", Weight="88kg" },
            new VisitRecord { Id="R005", PatientId="MC-2026-003", Date="2024-04-12", Diagnosis="Diabetes Follow-up",     Notes="Blood sugar improved to 138mg/dL.",                             Meds=new(){"Metformin 1000mg","Voltaren Gel"},          Doctor="Dr. Fareeha Shah", Followup="2024-05-12", Bp="128/82", Temp="36.7°C", Weight="87kg" },
        });
    }
}
