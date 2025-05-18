using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace HelloJkwCore.Tetration;

public record struct TePoint(double X, double Y);
public record struct TeSize(int Width, int Height);
public record struct TeRactangle(TePoint LeftTop, TePoint RightBottom);
public record struct TeOptions(int MaxIterations, double DivergenceRadius, double EpsX);
public record struct TetrationResult(string Base64Image, TePoint Center, TeSize Size, TeOptions Options);
public class TetrationService
{
    public event EventHandler<TetrationResult>? OnTetrationResult;
    public async Task<TetrationResult> CreateTetrationImage(TePoint center, TeSize size, TeOptions options)
    {
        var divergenceMap = GetTetrationDivergedTable(center, size, options);
        var imagePath = @$"./my-tetration/image-{Guid.NewGuid()}.png";
        await SaveBoolArrayAsImage(divergenceMap, imagePath);
        var base64Image = await File.ReadAllBytesAsync(imagePath);
        return new TetrationResult(Convert.ToBase64String(base64Image), center, size, options);
    }
    public async Task<TetrationResult> CreateTetrationImage(TeRactangle rectangle, TeSize imageSize, TeOptions options)
    {
        var divergenceMap = GetTetrationDivergedTable(rectangle, imageSize, options);
        var imagePath = @$"./my-tetration/image-{Guid.NewGuid()}.png";
        await SaveBoolArrayAsImage(divergenceMap, imagePath);
        var base64Image = await File.ReadAllBytesAsync(imagePath);
        return new TetrationResult(Convert.ToBase64String(base64Image), default, default, options);
    }
    public async Task<TetrationResult> CreateTetrationImageChunk(TeRactangle rectangle, TeSize imageSize, TeOptions options)
    {
        var imagePath = @$"./my-tetration/image-{Guid.NewGuid()}.png";
        var divergenceMap = await GetTetrationDivergedTable(rectangle, imageSize, options, async (map) =>
        {
            await SaveBoolArrayAsImage(map, imagePath);
            var base64Image = await File.ReadAllBytesAsync(imagePath);
            var result = new TetrationResult(Convert.ToBase64String(base64Image), default, default, options);
            OnTetrationResult?.Invoke(this, result);
        });
        var base64Image = await File.ReadAllBytesAsync(imagePath);
        return new TetrationResult(Convert.ToBase64String(base64Image), default, default, options);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle">수직선 상의 좌표</param>
    /// <param name="imageSize">이미지 크기</param>
    /// <param name="options"></param>
    /// <returns></returns>
    public bool[,] GetTetrationDivergedTable(TeRactangle rectangle, TeSize imageSize, TeOptions options)
    {
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

        return divergenceMap;
    }
    public async Task<bool[,]> GetTetrationDivergedTable(TeRactangle rectangle, TeSize imageSize, TeOptions options, Func<bool[,], Task> onProgress)
    {
        bool[,] divergenceMap = new bool[imageSize.Width, imageSize.Height];

        var chunked = Enumerable.Range(0, imageSize.Width)
            .Join(Enumerable.Range(0, imageSize.Height), i => true, j => true, (i, j) => (Column: i, Row: j))
            .Chunk(imageSize.Width * imageSize.Height / 10);

        foreach (var chunk in chunked)
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
            await onProgress.Invoke(divergenceMap);
        }

        return divergenceMap;
    }


    public bool[,] GetTetrationDivergedTable(TePoint center, TeSize size, TeOptions options)
    {
        bool[,] divergenceMap = new bool[size.Width, size.Height];

        var epsX = options.EpsX;
        var epsY = epsX * size.Height / size.Width;
        
        Enumerable.Range(0, size.Width)
            .Join(Enumerable.Range(0, size.Height), i => true, j => true, (i, j) => (i, j))
            .AsParallel()
            .Select(x => new
            {
                x.i,
                x.j,
                //     (   0   -  5  ) + 2 * 5 * i   
                real = center.X - epsX + 2.0 * epsX * x.i / size.Width,
                imag = center.Y - epsY + 2.0 * epsY * x.j / size.Height,
            })
            .Select(x => new { x.i, x.j, C = new Complex(x.real, x.imag) })
            .Select(x => new { x.i, x.j, Diverged = TetrationDivergedTest(x.C, options.MaxIterations, options.DivergenceRadius) })
            .ToList()
            .ForEach(x =>
            {
                divergenceMap[x.i, x.j] = x.Diverged;
            });

        return divergenceMap;
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

    public static async Task SaveBoolArrayAsImage(bool[,] data, string filename)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);

        var parent = Path.GetDirectoryName(filename);
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
                    image[x, height - y - 1] = new L8(color); // Flip vertically if needed
                }
            }

            await image.SaveAsync(filename, new PngEncoder());
        }
    }
}