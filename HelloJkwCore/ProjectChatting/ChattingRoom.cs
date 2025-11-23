using System.Collections.Concurrent;
using System.Threading.Channels;
using ProjectChatting.Models;

namespace ProjectChatting;

public class ChattingRoom(RoomName name)
{
    private readonly RoomName _name = name;
    private readonly ChannelBus<ChatMessage> _channelBus = new();
    private readonly IList<ChatMessage> _messageHistory = new List<ChatMessage>();

    public async ValueTask EnterAsync(AppUser userId)
    {
        var message = $"{userId.DisplayName}님이 입장했습니다.";
        await SendMessageAsync(new ChatMessage(new UserId("system"), "system", message, DateTimeOffset.Now));
    }

    public async ValueTask LeaveAsync(AppUser userId)
    {
        var message = $"{userId.DisplayName}님이 퇴장했습니다.";
        await SendMessageAsync(new ChatMessage(new UserId("system"), "system", message, DateTimeOffset.Now));
    }

    public (Guid id, ChannelReader<ChatMessage> reader) Subscribe(int capacity = 1024)
        => _channelBus.Subscribe(capacity);

    public void Unsubscribe(Guid id) => _channelBus.Unsubscribe(id);

    public async ValueTask SendMessageAsync(ChatMessage msg, CancellationToken ct = default)
    {
        lock (_messageHistory)
        {
            _messageHistory.Add(msg);
        }
        await _channelBus.PublishAsync(msg, ct);
    }
}

public sealed class ChannelBus<T> {
    private readonly ConcurrentDictionary<Guid, Channel<T>> _subs = new();

    public (Guid id, ChannelReader<T> reader) Subscribe(int capacity = 1024) {
        var ch = Channel.CreateBounded<T>(capacity);
        var id = Guid.NewGuid();
        _subs[id] = ch;
        return (id, ch.Reader);
    }

    public void Unsubscribe(Guid id) => _subs.TryRemove(id, out _);

    public async ValueTask PublishAsync(T msg, CancellationToken ct = default) {
        foreach (var ch in _subs.Values)
            await ch.Writer.WriteAsync(msg, ct);
    }
}
