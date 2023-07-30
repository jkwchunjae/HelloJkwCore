namespace ProjectPingpong;

public interface IPpNotifier<TData>
{
    public event EventHandler<TData>? Updated;
}

public class PpNotifier<TId, TData> : IPpNotifier<TData> where TId : StringId
{
    public TId Id { get; set; }
    public int WatchCount { get; set; }
    public event EventHandler<TData>? Updated;
    public PpNotifier(TId id)
    {
        Id = id;
        WatchCount = 1;
    }
    public void MatchUpdated(TData data)
    {
        Updated?.Invoke(this, data);
    }
}

