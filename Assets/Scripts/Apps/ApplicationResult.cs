[System.Serializable]
public class ApplicationResult
{    
    public bool succeeded;
    public string errorMessage;
    public static ApplicationResult GetSucceededResult()
    {
        return new ApplicationResult
        {
            succeeded = true,
            errorMessage = null,
        };
    }
    public static ApplicationResult GetFailedResult(string errorMessage)
    {
        return new ApplicationResult
        {
            succeeded = false,
            errorMessage = errorMessage,
        };
    }
}