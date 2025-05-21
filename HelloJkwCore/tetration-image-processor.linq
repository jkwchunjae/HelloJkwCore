<Query Kind="Program">
  <NuGetReference>SixLabors.ImageSharp</NuGetReference>
  <Namespace>SixLabors.ImageSharp</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Numerics</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>SixLabors.ImageSharp.PixelFormats</Namespace>
  <Namespace>SixLabors.ImageSharp.Formats.Png</Namespace>
</Query>

async Task Main()
{
//    _httpClient.BaseAddress = new Uri("https://localhost:44333");
    _httpClient.BaseAddress = new Uri("https://hellojkwcore.azurewebsites.net");

    while (true)
    {
        try
        {
            var task = await GetTetrationTask();
            if (task != default && task != null)
            {
                task.Dump(0);
                var image = await GetTetrationDivergedTable(task);
                await SendImage(task, image);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        catch (Exception ex)
        {
//            ex.Message.Dump();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}

HttpClient _httpClient = new();

public record struct TePoint(double X, double Y);
public record struct TeSize(int Width, int Height);
public record struct TeRectangle(TePoint LeftTop, TePoint RightBottom);
public record struct TeOptions(int MaxIterations, double DivergenceRadius, double EpsX);
public record struct TetrationTask(string TaskId, bool Allocated, TeRectangle Rectangle, TeSize ImageSize, TeOptions Options);

async Task<TetrationTask> GetTetrationTask()
{
    var response = await _httpClient.GetFromJsonAsync<TetrationTask>($"/api/Hello/tetration/task");
    
    return response;
}

async Task SendImage(TetrationTask task, string image)
{
    await _httpClient.PostAsJsonAsync($"/api/Hello/tetration/result", new
    {
        taskId = task.TaskId,
        base64Image = image,
    });
}

async Task SendProgress(TetrationTask task, string image, int progress, int total)
{
    await _httpClient.PostAsJsonAsync($"/api/Hello/tetration/progress", new
    {
        taskId = task.TaskId,
        base64Image = image,
        progress,
        total,
    });
}


public async Task<string> GetTetrationDivergedTable(TetrationTask task)
{
    var imageSize = task.ImageSize;
    var rectangle = task.Rectangle;
    var options = task.Options;
    
    bool[,] divergenceMap = new bool[imageSize.Width, imageSize.Height];

    var chunked = Enumerable.Range(0, imageSize.Width)
        .Join(Enumerable.Range(0, imageSize.Height), i => true, j => true, (i, j) => (Column: i, Row: j))
        .Chunk(imageSize.Width * imageSize.Height / 10)
        .ToArray();

    foreach (var (chunk, index) in chunked.Select((chunk, index) => (chunk, index)))
    {
        chunk
            .AsParallel()
            .Select(x => new
            {
                x.Column,
                x.Row,
                real = x.Column / (double)imageSize.Width * (rectangle.RightBottom.X - rectangle.LeftTop.X) + rectangle.LeftTop.X,
                imag = x.Row / (double)imageSize.Height * (rectangle.RightBottom.Y - rectangle.LeftTop.Y) + rectangle.LeftTop.Y,
                //     (   0   -  5  ) + 2 * 5 * i   
                // real = center.X - epsX + 2.0 * epsX * x.i / size.Width,
                // imag = center.Y - epsY + 2.0 * epsY * x.j / size.Height,
            })
            .Select(x => new { x.Column, x.Row, C = new Complex(x.real, x.imag) })
            .Select(x => new { x.Column, x.Row, Diverged = TetrationDivergedTest(x.C, options.MaxIterations, options.DivergenceRadius) })
            .ToList()
            .ForEach(x =>
            {
                divergenceMap[x.Column, x.Row] = x.Diverged;
            });

        var imagePath = Path.Join(Directory.GetParent(Util.CurrentScriptPath)!.FullName, "tetration.png");
        var base64Image = await SaveBoolArrayAsImage(divergenceMap, imagePath);
        await SendProgress(task, base64Image, index, chunked.Count());
    }

    var imagePath2 = Path.Join(Directory.GetParent(Util.CurrentScriptPath)!.FullName, "tetration.png");
    var base64Image2 = await SaveBoolArrayAsImage(divergenceMap, imagePath2);
    return base64Image2;
}

async Task<string> CreateTetrationImage(TetrationTask task)
{
    var imageSize = task.ImageSize;
    var rectangle = task.Rectangle;
    var options = task.Options;

    bool[,] divergenceMap = new bool[imageSize.Width, imageSize.Height];

    Enumerable.Range(0, imageSize.Width)
        .Join(Enumerable.Range(0, imageSize.Height), i => true, j => true, (i, j) => (Column: i, Row: j))
        .AsParallel()
        .Select(x => new
        {
            x.Column,
            x.Row,
            real = x.Column / (double)imageSize.Width * (rectangle.RightBottom.X - rectangle.LeftTop.X) + rectangle.LeftTop.X,
            imag = x.Row / (double)imageSize.Height * (rectangle.RightBottom.Y - rectangle.LeftTop.Y) + rectangle.LeftTop.Y,
            //     (   0   -  5  ) + 2 * 5 * i   
            // real = center.X - epsX + 2.0 * epsX * x.i / size.Width,
            // imag = center.Y - epsY + 2.0 * epsY * x.j / size.Height,
        })
        .Select(x => new { x.Column, x.Row, C = new Complex(x.real, x.imag) })
        .Select(x => new { x.Column, x.Row, Diverged = TetrationDivergedTest(x.C, options.MaxIterations, options.DivergenceRadius) })
        .ToList()
        .ForEach(x =>
        {
            divergenceMap[x.Column, x.Row] = x.Diverged;
        });

    var imagePath = Path.Join(Directory.GetParent(Util.CurrentScriptPath)!.FullName, "tetration.png");
    var base64Image = await SaveBoolArrayAsImage(divergenceMap, imagePath);

    return base64Image;
}

private bool TetrationDivergedTest(Complex c, int maxIteration, double escapeRadius = 1e10)
{
    Complex z = c;
    for (int k = 0; k < maxIteration; k++)
    {
        try
        {
            z = Complex.Pow(c, z);
        }
        catch
        {
            return true;
        }

        if (Math.Abs(z.Magnitude) > escapeRadius)
        {
            return true;
        }
    }

    return false;
}

public static async Task<string> SaveBoolArrayAsImage(bool[,] data, string imagePath)
{
    int width = data.GetLength(0);
    int height = data.GetLength(1);

    var parent = Path.GetDirectoryName(imagePath);
    if (!Directory.Exists(parent))
    {
        Directory.CreateDirectory(parent!);
    }

    using (var image = new Image<L8>(width, height)) // L8: grayscale 8-bit
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte color = data[x, y] ? (byte)255 : (byte)0;
                image[x, y] = new L8(color);
            }
        }

        await image.SaveAsync(imagePath, new PngEncoder());
    }

    var imageBytes = await File.ReadAllBytesAsync(imagePath);
    var base64Image = Convert.ToBase64String(imageBytes);
    File.Delete(imagePath);

    return base64Image;
}