namespace PublicUtilities.Middlewares;

public class AppException : Exception
{
    public int Code { get; }

    public AppException(string message, int code = 400) : base(message)
    {
        Code = code;
    }
}
