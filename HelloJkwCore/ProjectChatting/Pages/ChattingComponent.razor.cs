using ProjectChatting.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Microsoft.AspNetCore.Components.Web;

namespace ProjectChatting.Pages;

public partial class ChattingComponent : JkwPageBase, IAsyncDisposable
{
    [Parameter] public ChattingRoom Room { get; set; } = default!;

    private Guid? subId = null;
    private List<ChatMessage> Messages { get; } = new();

    private string newMessage = string.Empty;
    private ElementReference chatContainer;
    private int lastMessageCount = 0;
    private DateTime lastScroll = DateTime.MinValue;

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

        if (Messages.Count != lastMessageCount && Messages.Count > 0)
        {
            lastMessageCount = Messages.Count;
            var now = DateTime.UtcNow;
            if (now - lastScroll > TimeSpan.FromMilliseconds(50))
            {
                lastScroll = now;
                _ = Js.InvokeVoidAsync("scrollToBottom", chatContainer);
            }
        }

        return Task.CompletedTask;
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(newMessage))
            return;
        if (User == null)
            return;

        var msg = new ChatMessage(User.Id, newMessage.Trim(), DateTimeOffset.Now);
        await Room.SendMessageAsync(msg);
        newMessage = string.Empty;
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            // Shift+Enter for newline
            if (e.ShiftKey)
            {
                newMessage += "\n";
            }
            else
            {
                await SendMessageAsync();
            }
        }
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