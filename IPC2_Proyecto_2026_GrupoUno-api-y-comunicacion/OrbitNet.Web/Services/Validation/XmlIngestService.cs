using System.Text.RegularExpressions;
using System.Xml;
using OrbitNet.Web.Models.Entities; // Para asegurar la comunicación con la raíz de tus modelos
using OrbitNet.Web.DataStructures.Interfaces; // Para asegurar que IMessageBuffer se encuentra 

public class XmlIngestResult
{
    public bool Success { get; set; }
    public int NodosProcesados { get; set; }
    public string ErrorCode { get; set; }
    public string Details { get; set; }
}

public class XmlIngestService
{
    private readonly OrbitNetStore _store;

    private static readonly Regex RegexSatelliteId = new Regex(@"^SAT-(ECU|POL)-\d{4}$");
    private static readonly Regex RegexIpv4 = new Regex(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    private static readonly Regex RegexCoords = new Regex(@"^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$");

    public XmlIngestService(OrbitNetStore store)
    {
        _store = store;
    }

    public XmlIngestResult ProcesarConfiguracion(string xmlContent)
    {
        if (string.IsNullOrWhiteSpace(xmlContent))
        {
            return CrearError("XML_SCHEMA_VIOLATION", "El contenido XML esta vacio.");
        }

        XmlDocument doc = new XmlDocument();
        doc.XmlResolver = null;

        try
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
            using StringReader stringReader = new StringReader(xmlContent);
            using XmlReader reader = XmlReader.Create(stringReader, settings);
            doc.Load(reader);
        }
        catch (XmlException ex)
        {
            return CrearError("XML_SCHEMA_VIOLATION", "XML malformado o invalido: " + ex.Message);
        }

        int nodos = 0;

        XmlNodeList satelitesEcuatoriales = doc.SelectNodes("//constelaciones_ecuatoriales/satelite");
        if (satelitesEcuatoriales != null)
        {
            foreach (XmlNode satelite in satelitesEcuatoriales)
            {
                string id = satelite.Attributes?["id"]?.Value;
                string ip = satelite.SelectSingleNode("enlace_ip")?.InnerText;

                if (!RegexSatelliteId.IsMatch(id ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        XmlNodeList satelitesPolares = doc.SelectNodes("//orbitas_polares//satelite");
        if (satelitesPolares != null)
        {
            foreach (XmlNode satelite in satelitesPolares)
            {
                string id = satelite.Attributes?["id"]?.Value;

                if (!RegexSatelliteId.IsMatch(id ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        XmlNodeList antenas = doc.SelectNodes("//antenas_terrestres/antena");
        if (antenas != null)
        {
            foreach (XmlNode antena in antenas)
            {
                string coords = antena.SelectSingleNode("coordenadas")?.InnerText;
                string ip = antena.SelectSingleNode("ip_nodo")?.InnerText;

                if (!RegexCoords.IsMatch(coords ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "Las coordenadas '" + coords + "' no cumplen con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        _store.NodosProcesados = nodos;
        _store.ConfigLoaded = true;

        return new XmlIngestResult
        {
            Success = true,
            NodosProcesados = nodos
        };
    }

    private XmlIngestResult CrearError(string errorCode, string details)
    {
        return new XmlIngestResult
        {
            Success = false,
            ErrorCode = errorCode,
            Details = details
        };
    }
}
