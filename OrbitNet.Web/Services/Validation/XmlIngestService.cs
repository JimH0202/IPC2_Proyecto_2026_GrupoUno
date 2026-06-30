using System.Text.RegularExpressions;
using System.Xml;
using OrbitNet.Web.Models.Entities;

public class XmlIngestResult
{
    public bool Success { get; set; }
    public int NodosProcesados { get; set; }
    public string? ErrorCode { get; set; }
    public string? Details { get; set; }
    public int SatellitesLoaded { get; set; }
    public int AntennasLoaded { get; set; }
    public int PolarOrbitsLoaded { get; set; }
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

        _store.Clear();

        int satelitesCargados = 0;
        int antenasCargadas = 0;
        int orbitasCargadas = 0;

        XmlNodeList? orbitasPolares = doc.SelectNodes("//orbitas_polares/polar");
        if (orbitasPolares != null)
        {
            foreach (XmlNode orbita in orbitasPolares)
            {
                string orbitaId = orbita.Attributes?["id"]?.Value ?? $"POLAR-{orbitasCargadas + 1}";

                var polarOrbit = new PolarOrbit
                {
                    Id = orbitaId,
                    Name = orbitaId
                };

                _store.PolarOrbits.Add(polarOrbit);

                XmlNodeList? satelites = orbita.SelectNodes("satelite");
                if (satelites != null)
                {
                    int satelliteIndexEnOrbita = 1;

                    foreach (XmlNode satelite in satelites)
                    {
                        string? id = satelite.Attributes?["id"]?.Value;
                        string name = satelite.SelectSingleNode("nombre")?.InnerText ?? id ?? string.Empty;

                        if (!RegexSatelliteId.IsMatch(id ?? ""))
                        {
                            return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                        }

                        string ip = $"10.250.{orbitasCargadas + 1}.{satelliteIndexEnOrbita}";

                        var satellite = new Satellite
                        {
                            Id = id!,
                            Name = name,
                            Ip = ip,
                            AnomaliaOrbital = 0
                        };

                        _store.Satellites.Add(satellite);
                        _store.SatelliteRuntime.Register(satellite);
                        satelitesCargados++;

                        int rowIndex = satelliteIndexEnOrbita;
                        int colIndex = _store.PolarOrbits.Count;

                        _store.RedSatellites.Insert(rowIndex, colIndex, satellite.Id, satellite.Ip);

                        satelliteIndexEnOrbita++;
                    }
                }

                orbitasCargadas++;
            }
        }

        XmlNodeList? satelitesEcuatoriales = doc.SelectNodes("//constelaciones_ecuatoriales/satelite");
        if (satelitesEcuatoriales != null)
        {
            foreach (XmlNode satelite in satelitesEcuatoriales)
            {
                string? id = satelite.Attributes?["id"]?.Value;
                string? ip = satelite.SelectSingleNode("enlace_ip")?.InnerText;

                if (!RegexSatelliteId.IsMatch(id ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                var satellite = new Satellite
                {
                    Id = id!,
                    Name = satelite.SelectSingleNode("nombre")?.InnerText ?? id!,
                    Ip = ip!,
                    AnomaliaOrbital = 0
                };
                _store.Satellites.Add(satellite);
                _store.SatelliteRuntime.Register(satellite);
                satelitesCargados++;

                _store.RedSatellites.Insert(0, satelitesCargados, satellite.Id, satellite.Ip);
            }
        }

        XmlNodeList? antenas = doc.SelectNodes("//antenas_terrestres/antena");
        if (antenas != null)
        {
            foreach (XmlNode antena in antenas)
            {
                string? coords = antena.SelectSingleNode("coordenadas")?.InnerText;
                string? ip = antena.SelectSingleNode("ip_nodo")?.InnerText;

                if (!RegexCoords.IsMatch(coords ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "Las coordenadas '" + coords + "' no cumplen con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip ?? ""))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                var groundAntenna = new GroundAntenna
                {
                    Id = antena.Attributes?["id"]?.Value ?? $"ANT-{antenasCargadas + 1}",
                    Name = antena.SelectSingleNode("nombre")?.InnerText ?? "",
                    Coords = coords!,
                    Ip = ip!,
                    PosicionAngular = 0
                };
                _store.Antenas.Add(groundAntenna);
                antenasCargadas++;
            }
        }

        _store.NodosProcesados = satelitesCargados + antenasCargadas;
        _store.ConfigLoaded = true;

        _store.LogAuditoria.WriteEvent("Info", $"Configuracion cargada: {satelitesCargados} satelites, {antenasCargadas} antenas, {orbitasCargadas} orbitas.");

        return new XmlIngestResult
        {
            Success = true,
            NodosProcesados = _store.NodosProcesados,
            SatellitesLoaded = satelitesCargados,
            AntennasLoaded = antenasCargadas,
            PolarOrbitsLoaded = orbitasCargadas
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
