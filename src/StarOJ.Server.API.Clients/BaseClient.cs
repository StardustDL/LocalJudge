namespace StarOJ.Server.API.Clients
{
    public abstract class BaseClient
    {
        public static string Url { get; set; }

        public string BaseUrl => Url;
    }
}
