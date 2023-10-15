namespace GameLibra;


public class RemainTime
{
    public Player Player { get; set; }
    public int Time { get; set; }
}

public class TimeOverHandler
{
    public event EventHandler<RemainTime> RemainTimeChanged;
    public event EventHandler<Player> TimeOver;

    private int[] _alarmTime = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 30, 60, 120, 180, 240, 300 };
    private CancellationTokenSource _cts = null;

    public void Clear()
    {
        _cts?.Cancel();
    }

    public Task StartNew(Player player, TimeSpan remainTime /* 150 */)
    {
        var task = Task.Run(async () =>
        {
            var timeoutTask = Task.Delay(remainTime, _cts.Token);
            await timeoutTask;
            if (!timeoutTask.IsCanceled)
            {
                TimeOver?.Invoke(this, player);
            }
        });

        // 30, 90, 120, 140, 145, 147, 148, 149
        _cts = new CancellationTokenSource();
        var tasks = _alarmTime
            .Select(t => new { remain = t, timeout = (int)remainTime.TotalSeconds - t })
            .Where(x => x.timeout > 0)
            .Select(async x =>
            {
                var timeoutTask = Task.Delay(x.timeout * 1000, _cts.Token);
                await timeoutTask;
                if (!timeoutTask.IsCanceled)
                {
                    RemainTimeChanged?.Invoke(this, new RemainTime { Player = player, Time = x.remain });
                }
            })
            .ToArray();

        _ = Task.WhenAll(tasks);

        return Task.CompletedTask;
    }
}