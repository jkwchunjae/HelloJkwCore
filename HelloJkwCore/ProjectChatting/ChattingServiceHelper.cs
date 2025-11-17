using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace ProjectChatting;

public static class ChattingServiceHelper
{
    public static void AddChattingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ChattingService>();
        services.AddSingleton<JsonConverter>(new StringIdTextJsonConverter<RoomName>(id => new RoomName(id)));
    }
}

[TextJsonConverter(typeof(StringIdTextJsonConverter<RoomName>))]
public record RoomName(string id) : StringId(id);

public class ChattingService
{
    ConcurrentDictionary<RoomName, ChattingRoom> _rooms = new();

    public ChattingRoom CreateOrGetRoom(RoomName roomName)
    {
        while (true)
        {
            if (_rooms.TryGetValue(roomName, out var room))
            {
                return room;
            }
            else
            {
                var newRoom = new ChattingRoom(roomName);
                if (_rooms.TryAdd(roomName, newRoom))
                {
                    return newRoom;
                }
            }
        }
    }
}
