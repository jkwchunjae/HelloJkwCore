namespace HelloJkwCore.Tetration;

public record struct TetrationTask(string TaskId, bool Allocated, TeRectangle Rectangle, TeSize ImageSize, TeOptions Options);

public class TetrationGlobalService
{
    private readonly object _tasksLock = new();
    private readonly List<TetrationTask> _tasks = new();
    private readonly Dictionary<string, TaskCompletionSource<TetrationResult>> _tcsDictionary = new();
    private readonly Dictionary<string, TetrationService> _serviceDictionary = new();

    /// <summary>
    /// TetrationService가 global service에 요청한다.
    /// </summary>
    /// <returns></returns>
    public Task<TetrationResult> CreateTetrationImage(TetrationService service, TeRectangle rectangle, TeSize imageSize, TeOptions options)
    {
        lock (_tasksLock)
        {
            var taskId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<TetrationResult>();
            _tasks.Add(new TetrationTask(taskId, false, rectangle, imageSize, options));
            _tcsDictionary.Add(taskId, tcs);
            _serviceDictionary.Add(taskId, service);
            return tcs.Task;
        }
    }

    public TetrationTask? GetAnyTask()
    {
        lock (_tasksLock)
        {
            var allocatedTask = _tasks.FirstOrDefault(t => !t.Allocated);
            if (allocatedTask != default)
            {
                _tasks.Remove(allocatedTask);
                _tasks.Add(allocatedTask with { Allocated = true });
                return allocatedTask;
            } else {
                return null;
            }
        }
    }

    public void CompleteTask(string taskId, string base64Image)
    {
        lock (_tasksLock)
        {
            var task = _tasks.FirstOrDefault(t => t.TaskId == taskId);
            if (task != default)
            {
                var center = new TePoint((task.Rectangle.RightBottom.X + task.Rectangle.LeftTop.X) / 2, (task.Rectangle.RightBottom.Y + task.Rectangle.LeftTop.Y) / 2);
                var result = new TetrationResult(base64Image, center, task.ImageSize, task.Options);
                _tasks.Remove(task);
                _tcsDictionary[taskId].SetResult(result);
                _tcsDictionary.Remove(taskId);
                _serviceDictionary.Remove(taskId);
            }
        }
    }

    public void ProgressTask(string taskId, string base64Image, int progress, int total)
    {
        lock (_tasksLock)
        {
            var task = _tasks.FirstOrDefault(t => t.TaskId == taskId);
            if (task != default)
            {
                var center = new TePoint((task.Rectangle.RightBottom.X + task.Rectangle.LeftTop.X) / 2, (task.Rectangle.RightBottom.Y + task.Rectangle.LeftTop.Y) / 2);
                var result = new TetrationResult(base64Image, center, task.ImageSize, task.Options);
                var service = _serviceDictionary[taskId];
                service.Progress(result);
            }
        }
    }
}
