namespace GameLibra;

public class HintAttribute : Attribute
{
    public string Description { get; private set; }
    public HintAttribute(string description)
    {
        Description = description;
    }
}

public enum LibraGameHint
{
    /// <summary>
    /// 힌트 필요 없습니다.
    /// </summary>
    [Hint("힌트 필요 없습니다.")]
    None,
    /// <summary>
    /// 무작위로 큐브 하나를 공개합니다.
    /// </summary>
    [Hint("무작위로 큐브 한 개를 공개합니다.")]
    OpenRandom1,
    /// <summary>
    /// 무작위로 큐브 두개를 공개합니다.
    /// </summary>
    [Hint("무작위로 큐브 두 개를 공개합니다.")]
    OpenRandom2,
    /// <summary>
    /// 가장 작은 큐브 하나를 공개합니다.
    /// </summary>
    [Hint("가장 작은 큐브 한 개를 공개합니다.")]
    OpenSmall1,
    /// <summary>
    /// 가장 작은 큐브 두개를 공개합니다.
    /// </summary>
    [Hint("가장 작은 큐브 두 개를 공개합니다.")]
    OpenSmall2,
    /// <summary>
    /// 가운데 큐브 한 개를 공개합니다.
    /// </summary>
    [Hint("가운데 큐브 한 개를 공개합니다. (데블스 플랜)")]
    OpenMiddle1,
    /// <summary>
    /// 가장 큰 큐브 한 개를 공개합니다.
    /// </summary>
    [Hint("가장 큰 큐브 한 개를 공개합니다.")]
    OpenLarge1,
    /// <summary>
    /// 가장 큰 큐브 두 개를 공개합니다.
    /// </summary>
    [Hint("가장 큰 큐브 두 개를 공개합니다.")]
    OpenLarge2,
}
