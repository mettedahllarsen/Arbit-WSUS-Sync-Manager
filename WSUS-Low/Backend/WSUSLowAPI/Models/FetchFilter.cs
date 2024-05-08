namespace WSUSLowAPI.Models
{
    public class FetchFilter
    {
        public const string MicrosoftUpdateEndpoint = "microsoft-update";
        public const string NuGetV3Endpoint = "nuget";
        public const string LinuxEndpoint = "linux";
        public const string WebEndpoint = "web";

        public string Endpoint { get; set; } = MicrosoftUpdateEndpoint;
        public string? Title { get; set; }

    }
}
