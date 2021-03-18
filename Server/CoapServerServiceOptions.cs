namespace Server
{
    using System.Net;

    public class CoapServerServiceOptions
    {
        public const string KEY = "CoapServerService";

        public string LocalAddress { get; set; } = IPAddress.Loopback.ToString();

        public int Port { get; set; } = 5683;
    }
}