using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Reflection;
using System.Text;

namespace ProjectWorldCup.Pages;

public partial class FifaFailoverPage : JkwPageBase
{
    [Inject] IFifa Fifa { get; set; }

    IEnumerable<string> fifaFailoverTiles = new List<string>();
    Exception Error = null;
    string SelectedTitle = null;
    string Value = string.Empty;
    string UploadValue = string.Empty;

    protected override async Task OnPageInitializedAsync()
    {
        if (User?.HasRole(UserRole.Admin) ?? false)
        {
            fifaFailoverTiles = await Fifa.GetFailoverList();
        }
        else
        {
            Navi.NavigateTo("/");
        }
    }

    private async Task FailoverValueChanged(string title)
    {
        try
        {
            UploadValue = string.Empty;
            SelectedTitle = title;
            Value = await Fifa.GetFailoverData(title);
            Error = null;
        }
        catch (Exception ex)
        {
            UploadValue = string.Empty;
            Error = ex;
            Value = null;
            SelectedTitle = null;
        }
        StateHasChanged();
    }

    private async Task SaveFailoverData()
    {
        var json = Json.Serialize(Json.Deserialize<object>(UploadValue));
        await Fifa.SaveFailoverData(SelectedTitle, json);
        await FailoverValueChanged(SelectedTitle);
    }

    private async Task DownloadFile()
    {
        Stream fileStream = new MemoryStream(Encoding.UTF8.GetBytes(Value));
        var fileName = $"{SelectedTitle}.json";

        using var streamRef = new DotNetStreamReference(stream: fileStream);

        await Js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }

    private async Task SelectFile(InputFileChangeEventArgs e)
    {
        IBrowserFile file = e.File;
        if (file != null)
        {
            UploadValue = await GetStringFromFile(file);
        }
    }

    private async Task<string> GetStringFromFile(IBrowserFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
