using HealthcareManagementSystem.Models;
using HealthcareManagementSystem.Repositories;

namespace HealthcareManagementSystem;

public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo;
    private readonly Repository<Prescription> _prescriptionRepo;
    private readonly Dictionary<int, List<Prescription>> _prescriptionMap;
    private int _nextPatientId = 1;
    private int _nextPrescriptionId = 1;

    public HealthSystemApp()
    {
        _patientRepo = new Repository<Patient>();
        _prescriptionRepo = new Repository<Prescription>();
        _prescriptionMap = new Dictionary<int, List<Prescription>>();
    }

    public int GetIntInput(string prompt)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Please enter a valid positive number.");
        }
    }

    private string GetStringInput(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    public void AddNewPatient()
    {
        Console.WriteLine("\n=== Add New Patient ===");
        string name = GetStringInput("Enter patient name: ");
        int age = GetIntInput("Enter patient age: ");
        string gender = GetStringInput("Enter patient gender (Male/Female): ");

        var patient = new Patient(_nextPatientId++, name, age, gender);
        _patientRepo.Add(patient);
        Console.WriteLine($"\nPatient added successfully! Patient ID: {patient.Id}");
    }

    public void AddNewPrescription()
    {
        Console.WriteLine("\n=== Add New Prescription ===");
        
        PrintAllPatients();
        int patientId = GetIntInput("Enter patient ID: ");

        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine("Patient not found!");
            return;
        }

        string medicationName = GetStringInput("Enter medication name: ");
        var prescription = new Prescription(_nextPrescriptionId++, patientId, medicationName, DateTime.Now);
        _prescriptionRepo.Add(prescription);

        if (!_prescriptionMap.ContainsKey(patientId))
        {
            _prescriptionMap[patientId] = new List<Prescription>();
        }
        _prescriptionMap[patientId].Add(prescription);

        Console.WriteLine("Prescription added successfully!");
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        var allPrescriptions = _prescriptionRepo.GetAll();
        foreach (var prescription in allPrescriptions)
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        var patients = _patientRepo.GetAll();
        if (patients.Count == 0)
        {
            Console.WriteLine("\nNo patients in the system.");
            return;
        }

        Console.WriteLine("\n=== All Patients ===");
        foreach (var patient in patients)
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"\nNo patient found with ID: {patientId}");
            return;
        }

        Console.WriteLine($"\n=== Prescriptions for {patient.Name} ===");
        if (_prescriptionMap.TryGetValue(patientId, out var prescriptions) && prescriptions.Count > 0)
        {
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription);
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var prescriptions) 
            ? prescriptions 
            : new List<Prescription>();
    }
}

class Program
{
    static void DisplayMenu()
    {
        Console.WriteLine("\n=== Healthcare Management System Menu ===");
        Console.WriteLine("1. Add New Patient");
        Console.WriteLine("2. Add New Prescription");
        Console.WriteLine("3. View All Patients");
        Console.WriteLine("4. View Patient's Prescriptions");
        Console.WriteLine("5. Exit");
        Console.Write("\nEnter your choice (1-5): ");
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("=== Healthcare Management System ===\n");
        var app = new HealthSystemApp();
        
        while (true)
        {
            DisplayMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    app.AddNewPatient();
                    break;

                case "2":
                    app.AddNewPrescription();
                    break;

                case "3":
                    app.PrintAllPatients();
                    break;

                case "4":
                    app.PrintAllPatients();
                    int patientId = app.GetIntInput("Enter patient ID to view prescriptions: ");
                    app.PrintPrescriptionsForPatient(patientId);
                    break;

                case "5":
                    Console.WriteLine("\nThank you for using the Healthcare Management System!");
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;
            }
        }
    }
}
