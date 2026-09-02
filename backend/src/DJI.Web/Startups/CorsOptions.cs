namespace DJI.Web.Startups;

public class CorsOptions
{
    public const string SectionName = "Cors";

    public const string DevPolicyName = "dev";

    public string[] AllowedOrigins { get; set; } = [];
}
