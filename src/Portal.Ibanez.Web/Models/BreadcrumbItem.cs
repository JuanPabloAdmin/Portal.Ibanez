namespace Portal.Ibanez.Web.Models;

public class BreadcrumbItem
{
    public string Text { get; set; } = "";

    public string? Url { get; set; }

    public bool Active { get; set; }
}