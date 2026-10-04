namespace MediCore.Models;

public class Patient
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Gender { get; set; } = "";
    public string Phone { get; set; } = "N/A";
    public string Email { get; set; } = "N/A";
    public string Blood { get; set; } = "N/A";
    public string Address { get; set; } = "N/A";
    public string Emergency { get; set; } = "N/A";
    public string Joined { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
}

public class VisitRecord
{
    public string Id { get; set; } = "";
    public string PatientId { get; set; } = "";
    public string Date { get; set; } = "";
    public string Diagnosis { get; set; } = "";
    public string Notes { get; set; } = "";
    public List<string> Meds { get; set; } = new();
    public string Doctor { get; set; } = "";
    public string Followup { get; set; } = "";
    public string Bp { get; set; } = "";
    public string Temp { get; set; } = "";
    public string Weight { get; set; } = "";
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
}

public class PatientLoginRequest
{
    public string PatientId { get; set; } = "";
}

public class RegisterPatientRequest
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Gender { get; set; } = "";
    public string Blood { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string Emergency { get; set; } = "";
    public string Joined { get; set; } = "";
}

public class AddRecordRequest
{
    public string PatientId { get; set; } = "";
    public string Date { get; set; } = "";
    public string Diagnosis { get; set; } = "";
    public string Notes { get; set; } = "";
    public List<string> Meds { get; set; } = new();
    public string Doctor { get; set; } = "";
    public string Followup { get; set; } = "";
    public string Bp { get; set; } = "";
    public string Temp { get; set; } = "";
    public string Weight { get; set; } = "";
}
