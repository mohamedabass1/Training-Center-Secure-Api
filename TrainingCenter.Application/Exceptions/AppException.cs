
namespace TrainingCenter.Application.Exceptions
{
    public abstract partial class AppException : Exception
    {
        protected AppException(string message)
            : base(message)
        {
        }
    }
}
