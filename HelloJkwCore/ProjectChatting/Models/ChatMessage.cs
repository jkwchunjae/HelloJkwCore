namespace ProjectChatting.Models;

public record ChatMessage(UserId Sender, string Content, DateTimeOffset SentAt);