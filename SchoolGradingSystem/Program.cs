using SchoolGradingSystem.Exceptions;
using SchoolGradingSystem.Models;

namespace SchoolGradingSystem;

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        string[] lines = File.ReadAllLines(inputFilePath);
        int lineNumber = 0;

        foreach (var line in lines)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = line.Split(',').Select(f => f.Trim()).ToArray();

            if (fields.Length != 3)
            {
                throw new StudentFieldMissingException(
                    $"Line {lineNumber}: Expected 3 fields (ID, Name, Score), but found {fields.Length} fields.");
            }

            if (!int.TryParse(fields[0], out int id))
            {
                throw new InvalidScoreFormatException(
                    $"Line {lineNumber}: Invalid student ID format: {fields[0]}");
            }

            if (!int.TryParse(fields[2], out int score))
            {
                throw new InvalidScoreFormatException(
                    $"Line {lineNumber}: Invalid score format: {fields[2]}");
            }

            if (score < 0 || score > 100)
            {
                throw new InvalidScoreFormatException(
                    $"Line {lineNumber}: Score must be between 0 and 100. Found: {score}");
            }

            students.Add(new Student(id, fields[1], score));
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);

        writer.WriteLine("=== Student Grade Report ===");
        writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        writer.WriteLine("============================\n");

        foreach (var student in students)
        {
            writer.WriteLine(student.ToString());
        }

        writer.WriteLine("\n============================");
        writer.WriteLine($"Total Students: {students.Count}");
        
        var gradeDistribution = students
            .GroupBy(s => s.GetGrade())
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        writer.WriteLine("\nGrade Distribution:");
        foreach (var grade in "ABCDF")
        {
            int count = gradeDistribution.GetValueOrDefault(grade.ToString(), 0);
            writer.WriteLine($"Grade {grade}: {count} student(s)");
        }
    }
}

class Program
{
    private static List<Student> students = new List<Student>();
    private static readonly string inputFile = "students.txt";
    private static readonly string outputFile = "grade_report.txt";
    private static readonly StudentResultProcessor processor = new StudentResultProcessor();

    private static int GetValidId()
    {
        while (true)
        {
            Console.Write("Enter student ID: ");
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                if (students.Any(s => s.Id == id))
                {
                    Console.WriteLine("Error: This ID already exists. Please use a unique ID.");
                    continue;
                }
                return id;
            }
            Console.WriteLine("Error: Please enter a valid positive number for ID.");
        }
    }

    private static string GetValidName()
    {
        while (true)
        {
            Console.Write("Enter student full name: ");
            string? name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            Console.WriteLine("Error: Name cannot be empty.");
        }
    }

    private static int GetValidScore()
    {
        while (true)
        {
            Console.Write("Enter student score (0-100): ");
            if (int.TryParse(Console.ReadLine(), out int score) && score >= 0 && score <= 100)
            {
                return score;
            }
            Console.WriteLine("Error: Please enter a valid score between 0 and 100.");
        }
    }

    private static void AddStudent()
    {
        Console.WriteLine("\n=== Add New Student ===");
        int id = GetValidId();
        string name = GetValidName();
        int score = GetValidScore();

        students.Add(new Student(id, name, score));
        Console.WriteLine("Student added successfully!");
    }

    private static void ViewAllStudents()
    {
        if (students.Count == 0)
        {
            Console.WriteLine("\nNo students in the system.");
            return;
        }

        Console.WriteLine("\n=== Current Students ===");
        foreach (var student in students.OrderBy(s => s.Id))
        {
            Console.WriteLine(student);
        }
    }

    private static void SaveToFile()
    {
        if (students.Count == 0)
        {
            Console.WriteLine("\nNo students to save.");
            return;
        }

        try
        {
            var lines = students.OrderBy(s => s.Id)
                              .Select(s => $"{s.Id},{s.FullName},{s.Score}");
            File.WriteAllLines(inputFile, lines);
            
            processor.WriteReportToFile(students, outputFile);
            
            Console.WriteLine($"\nData saved to {inputFile}");
            Console.WriteLine($"Detailed report saved to {outputFile}");
            Console.WriteLine("\nReport Contents:");
            Console.WriteLine(File.ReadAllText(outputFile));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError saving files: {ex.Message}");
        }
    }

    private static void LoadFromFile()
    {
        try
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"\nNo existing data file ({inputFile}) found.");
                return;
            }

            students = processor.ReadStudentsFromFile(inputFile);
            Console.WriteLine($"\nSuccessfully loaded {students.Count} students from {inputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError loading file: {ex.Message}");
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("\n=== School Grading System Menu ===");
        Console.WriteLine("1. Add New Student");
        Console.WriteLine("2. View All Students");
        Console.WriteLine("3. Save to File");
        Console.WriteLine("4. Load from File");
        Console.WriteLine("5. Exit");
        Console.Write("\nEnter your choice (1-5): ");
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("=== School Grading System ===");
        
        while (true)
        {
            DisplayMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;

                case "2":
                    ViewAllStudents();
                    break;

                case "3":
                    SaveToFile();
                    break;

                case "4":
                    LoadFromFile();
                    break;

                case "5":
                    Console.WriteLine("\nThank you for using the School Grading System!");
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;
            }
        }
    }
}
