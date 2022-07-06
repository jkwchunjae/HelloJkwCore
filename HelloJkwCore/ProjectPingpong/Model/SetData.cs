using Newtonsoft.Json.Linq;

namespace ProjectPingpong;

[JsonConverter(typeof(GamePointJsonConverter))]
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
public class GamePointJsonConverter : JsonConverter<GamePoint>
{
    public override GamePoint ReadJson(JsonReader reader, Type objectType, GamePoint? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is string pointText)
        {
            var arr = pointText.Split(':').Select(x => x.ToInt()).ToArray();
            var gamePoint = new GamePoint(arr[0], arr[1]);
            return gamePoint;
        }
        else
        {
            JObject? obj = (JObject?)serializer.Deserialize(reader);
            if (obj != null)
            {
                var leftPoint = obj.Property("LeftPoint")?.Value.ToObject<int>() ?? 0;
                var rightPoint = obj.Property("RightPoint")?.Value.ToObject<int>() ?? 0;
                return new GamePoint(leftPoint, rightPoint);
            }
        }

        return new GamePoint();
    }

    public override void WriteJson(JsonWriter writer, GamePoint? value, JsonSerializer serializer)
    {
        writer.WriteValue($"{value?.LeftPoint}:{value?.RightPoint}");
    }
}



[JsonConverter(typeof(StringEnumConverter))]
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
    [JsonIgnore] private int? _currentHistory = null;

    [JsonIgnore] public bool IsPlaying => Status == SetStatus.Playing;

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
