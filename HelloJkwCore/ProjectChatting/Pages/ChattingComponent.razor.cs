using ProjectChatting.Models;

namespace ProjectChatting.Pages;

public partial class ChattingComponent : JkwPageBase, IAsyncDisposable
{
    [Parameter] public ChattingRoom Room { get; set; } = default!;

    private Guid? subId = null;
    private List<ChatMessage> Messages { get; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        if (User == null)
            throw new InvalidOperationException("User must be logged in to enter the chat room.");
        await Room.EnterAsync(User);
    }

    protected override Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            (subId, var channelReader) = Room.Subscribe();
            _ = Task.Run(async () =>
            {
                await foreach (var message in channelReader.ReadAllAsync())
                {
                    await InvokeAsync(() =>
                    {
                        Messages.Add(message);
                        StateHasChanged();
                    });
                }
            });
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (subId != null && subId.HasValue)
        {
            Room.Unsubscribe(subId.Value);
        }

        if (User != null)
        {
            await Room.LeaveAsync(User!);
        }
    }
}