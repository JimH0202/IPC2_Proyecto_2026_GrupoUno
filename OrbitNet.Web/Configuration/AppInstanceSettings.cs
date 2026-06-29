namespace OrbitNet.Web.Configuration;

/// Configuración de la instancia de simulación cargada desde el appsettings.
public class AppInstanceSettings
{
    public string Hemisphere { get; set; } = "North";
    public int Port { get; set; } = 5000;
    
    // Tu variable original para no romper tu API
    public int SiblingPort { get; set; } = 5001; 
    
    // La variable de tu compañero para no romper su UI
    public string RemoteHemisphereUrl { get; set; } = "http://localhost:5001";
}