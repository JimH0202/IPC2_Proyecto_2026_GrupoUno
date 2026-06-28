using System.Text;

public class BasicAuthService
{
    public string CrearCabeceraAuthorization()
    {
        string credenciales = Constants.AuthUser + ":" + Constants.AuthPassword;
        string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credenciales));
        return "Basic " + base64;
    }

    public bool EsCabeceraValida(string? authHeader)
    {
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return false;
        }

        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string base64 = authHeader.Substring(6).Trim();
        string credencialesDecodificadas = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        string credencialesEsperadas = Constants.AuthUser + ":" + Constants.AuthPassword;

        return credencialesDecodificadas == credencialesEsperadas;
    }
}
