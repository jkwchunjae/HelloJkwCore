namespace ProjectWorldCup;

public interface ICacheClearInvoker
{
    event EventHandler ClearCacheInvoked;
    void ClearCache();
}

public class CacheClearInvoker : ICacheClearInvoker
{
    public event EventHandler ClearCacheInvoked;

    public void ClearCache()
    {
        ClearCacheInvoked?.Invoke(this, new());
    }
}
