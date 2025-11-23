namespace ProjectChatting.Models;

public record ChatMessage(UserId Sender, string SenderName, string Content, DateTimeOffset SentAt);