namespace Application.TaskItems.DTOs;

public record ExportedTaskItemsDto(byte[] Content, string MimeType);