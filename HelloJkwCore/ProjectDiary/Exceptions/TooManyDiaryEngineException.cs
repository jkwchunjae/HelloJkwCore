namespace ProjectDiary;

public class TooManyDiaryEngineException : Exception
{
    public TooManyDiaryEngineException()
    {
    }

    public TooManyDiaryEngineException(string message)
        : base(message)
    {
    }
}

