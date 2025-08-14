namespace SchoolGradingSystem.Exceptions;

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class StudentFieldMissingException : Exception
{
    public StudentFieldMissingException(string message) : base(message) { }
}
