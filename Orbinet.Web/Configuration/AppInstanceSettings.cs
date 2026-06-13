namespace OrbitNet.Web.Configuration;

public class AppInstanceSettings
{
    public string Hemisphere { get; set; } = "North";
    public int Port { get; set; } = 5000;
    public string RemoteHemisphereUrl { get; set; } = "http://localhost:5001";
}
