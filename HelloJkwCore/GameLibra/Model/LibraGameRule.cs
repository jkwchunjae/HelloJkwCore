namespace GameLibra;

public class LibraGameRule
{
    /// <summary>
    /// 큐브 수
    /// </summary>
    public int CubeCount { get; set; }
    /// <summary>
    /// 플레이어 수
    /// </summary>
    public int PlayerCount { get; set; }
    /// <summary>
    /// 큐브의 최소값
    /// </summary>
    public int CubeMinValue { get; set; }
    /// <summary>
    /// 큐브의 최대값
    /// </summary>
    public int CubeMaxValue { get; set; }
    /// <summary>
    /// 각 플레이어가 한 종류의 큐브를 몇개씩 가지고 시작할지
    /// </summary>
    public int CubePerPlayer { get; set; }
    /// <summary>
    /// 게임을 시작하기 전에 큐브를 하나 공개할지
    /// </summary>
    public bool OpenFirst { get; set; }
    /// <summary>
    /// 게임을 시작하기 전에 공개할 큐브의 인덱스
    /// </summary>
    public int OpenCubeIndex { get; set; }
    /// <summary>
    /// 큐브를 저울에 올릴 때 최소한 올려야 하는 큐브 수
    /// 각각 다른 저울에 올려도 된다
    /// </summary>
    public int MinimumApplyCubeCount { get; set; }
    /// <summary>
    /// 양팔저울의 수
    /// </summary>
    public int ScaleCount { get; set; }
}