namespace Application.Common.Configurations;

public sealed class EmailSenderOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; }
    public string From { get; set; } = null!;
    public string FromName { get; set; } = null!;
}