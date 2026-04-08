namespace SmartTab.UI.Services;

public class SmtpSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = "SmartTab Store";
}
