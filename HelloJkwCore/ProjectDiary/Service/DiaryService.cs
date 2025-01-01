using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ProjectDiary;

public partial class DiaryService : IDiaryService
{
    private readonly IFileSystem _fs;
    private readonly IDiarySearchService _diarySearchService;
    private readonly Dictionary<DiaryName, List<DiaryFileName>> _filesCache = new();
    private readonly IBackgroundTaskQueue _backgroundQueue;

    public DiaryService(
        IBackgroundTaskQueue backgroundQueue,
        IDiarySearchService diarySearchService,
        [FromKeyedServices(nameof(DiaryService))] IFileSystem fileSystem)
    {
        _backgroundQueue = backgroundQueue;
        _diarySearchService = diarySearchService;
        _fs = fileSystem;
    }

    private async Task<List<DiaryFileName>> GetDiaryListAsync(DiaryName diaryName)
    {
        lock (_filesCache)
        {
            if (_filesCache.TryGetValue(diaryName, out var list))
            {
                return list;
            }
        }

        Func<Paths, string> diaryPath = path => path.Diary(diaryName);

        if (!(await _fs.DirExistsAsync(diaryPath)))
        {
            await _fs.CreateDirectoryAsync(diaryPath);
        }

        var files = await _fs.GetFilesAsync(diaryPath, ".diary");

        var diaryFileNameList = files
            .OrderBy(x => x)
            .Where(x => x.Left(8).TryToDate(out var _))
            .Select(x => new DiaryFileName(x))
            .ToList();

        lock (_filesCache)
        {
            _filesCache[diaryName] = diaryFileNameList;
        }

        return diaryFileNameList;
    }

    private void SaveDiaryCache(DiaryName diaryName, List<DiaryFileName> list)
    {
        lock (_filesCache)
        {
            _filesCache[diaryName] = list;
        }
    }

    public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text)
    {
        return await WriteDiaryAsync(user, diary, date, text, false);
    }

    public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password)
    {
        var cipherText = text.Encrypt(password);
        return await WriteDiaryAsync(user, diary, date, cipherText, true);
    }

    private async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, bool isSecret)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        var content = new DiaryContent
        {
            Date = date,
            WriterId = user?.Id,
            WirterName = user?.DisplayName,
            Text = text,
            IsSecret = isSecret,
            RegDate = DateTime.Now,
            LastModifyDate = DateTime.Now,
            Index = MakeNewIndex(list, date),
        };

        Func<Paths, string> diaryPath = path => path.Content(diary.DiaryName, content.GetFileName());
        await _fs.WriteJsonAsync(diaryPath, content);

        var diaryFileName = new DiaryFileName(content.GetFileName());
        list.Add(diaryFileName);

        list = list.OrderBy(x => x).ToList();

        SaveDiaryCache(diary.DiaryName, list);

        if (!diary.IsSecret)
        {
            _backgroundQueue?.QueueBackgroundWorkItem(async token =>
            {
                await _diarySearchService?.AppendDiaryTextAsync(diary.DiaryName, diaryFileName, text);
                await _diarySearchService?.SaveDiaryTrie(diary.DiaryName, date.Year);
            });
        }

        return content;

    }

    private int MakeNewIndex(List<DiaryFileName> fileList, DateTime date)
    {
        var today = fileList.Where(x => x.Date == date).ToList();
        if (today.Any())
        {
            return today.Max(x => x.Index) + 1;
        }
        else
        {
            return 1;
        }
    }

    public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        var deleteFiles = contents.Where(x => string.IsNullOrWhiteSpace(x.Text)).ToList();
        var updateFiles = contents.Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();

        foreach (var deleteFile in deleteFiles)
        {
            Func<Paths, string> deleteFilePath = path => path.Content(diary.DiaryName, deleteFile.GetFileName());
            await _fs.DeleteFileAsync(deleteFilePath);
            list.Remove(new DiaryFileName(deleteFile.GetFileName()));
        }

        foreach (var updateFile in updateFiles)
        {
            Func<Paths, string> updateFilePath = path => path.Content(diary.DiaryName, updateFile.GetFileName());
            await _fs.WriteJsonAsync(updateFilePath, updateFile);
        }

        SaveDiaryCache(diary.DiaryName, list);

        return updateFiles;
    }

    public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents, string password)
    {
        foreach (var content in contents.Where(x => x.IsSecret))
        {
            content.Text = content.Text.Encrypt(password);
        }

        return await UpdateDiaryAsync(user, diary, contents);
    }

    public async Task<DiaryView> UploadImageAsync(AppUser user, DiaryInfo diary, DateTime date, IReadOnlyList<IBrowserFile> files)
    {
        var view = await GetDiaryViewAsync(user, diary, date);
        if (view.DiaryContents?.Any() ?? false)
        {
            var diaryContent = view.DiaryContents.First();
            var pictureLastIndex = diaryContent.PictureLastIndex;
            var images = await files
                .Select(async (file, index) =>
                {
                    var pictureIndex = pictureLastIndex + index + 1;
                    var fileName = $"{date:yyyyMMdd}_{pictureIndex:D3}.{file.Name}";
                    Func<Paths, string> picturePath = path => path.Picture(diary.DiaryName, fileName);
                    const int _10MB = 10 * 1024 * 1024;
                    var imageStream = await ResizeImageAsync(file.OpenReadStream(maxAllowedSize: _10MB), width: 1024);
                    await _fs.WriteBlobAsync(picturePath, imageStream);
                    return fileName;
                })
                .WhenAll();
            diaryContent.Pictures ??= new List<string>();
            diaryContent.Pictures.AddRange(images);
            await UpdateDiaryAsync(user, diary, view.DiaryContents);
            var newView = await GetDiaryViewAsync(user, diary, date);
            return newView;
        }
        else
        {
            return view;
        }
    }

    private async Task<Stream> ResizeImageAsync(Stream opededReadStream, int width)
    {
        using var image = await Image.LoadAsync(opededReadStream);
        if (image.Width > width)
        {
            var height = (int)(image.Height * width / (double)image.Width);
            image.Mutate(x => x.Resize(width, height));
        }
        var outputStream = new MemoryStream();
        await image.SaveAsJpegAsync(outputStream);
        outputStream.Position = 0;
        return outputStream;
    }
}