namespace delivery_backend_advanced.Exceptions;

public class AuthErrorsException : Exception
{
    public List<string> Errors;
    
    public AuthErrorsException(List<string> Errors)
    {
        this.Errors = Errors;
    }
}