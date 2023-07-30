using Newtonsoft.Json.Linq;

namespace ProjectPingpong;

[TextJsonConverter(typeof(GamePointJsonConverter))]
public class GamePoint
{
    public int LeftPoint { get; set; }
    public int RightPoint { get; set; }

    public GamePoint()
    {
        LeftPoint = 0;
        RightPoint = 0;
    }
    public GamePoint(int leftPoint, int rightPoint)
    {
        LeftPoint = leftPoint;
        RightPoint = rightPoint;
    }
    public GamePoint(GamePoint gamePoint)
    {
        LeftPoint = gamePoint.LeftPoint;
        RightPoint = gamePoint.RightPoint;
    }
}

public class GamePointJsonConverter : System.Text.Json.Serialization.JsonConverter<GamePoint>
{
    public override GamePoint Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var pointText = reader.GetString();
        var arr = pointText.Split(':').Select(x => x.ToInt()).ToArray();
        var gamePoint = new GamePoint(arr[0], arr[1]);
        return gamePoint;
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, GamePoint? value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value?.LeftPoint}:{value?.RightPoint}");
    }
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum SetStatus
{
    Prepare,
    Playing,
    End,
}
public class SetData
{
    public SetStatus Status { get; set; } = SetStatus.Prepare;
    public GamePoint CurrentPoint { get; set; } = new();
    public List<GamePoint> History { get; set; } = new();
    [TextJsonIgnore] private int? _currentHistory = null;

    [TextJsonIgnore] public bool IsPlaying => Status == SetStatus.Playing;

    private void UpdatePoint()
    {
        History.Add(new GamePoint(CurrentPoint));
    }
    public int IncreaseLeft()
    {
        CurrentPoint.LeftPoint++;
        UpdatePoint();
        return CurrentPoint.LeftPoint;
    }
    public int IncreaseRight()
    {
        CurrentPoint.RightPoint++;
        UpdatePoint();
        return CurrentPoint.RightPoint;
    }

    public void Undo()
    {
    }

    public void Redo()
    {
    }
}
