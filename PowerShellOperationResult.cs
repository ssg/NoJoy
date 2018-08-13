namespace NoJoy
{
    public class PowerShellOperationResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        public PowerShellOperationResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}