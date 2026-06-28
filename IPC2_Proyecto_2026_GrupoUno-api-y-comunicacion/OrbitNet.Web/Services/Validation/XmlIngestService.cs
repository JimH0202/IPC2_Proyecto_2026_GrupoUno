using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using OrbitNet.Web.DataStructures.Antenas;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;

public class XmlIngestResult
{
    public bool Success { get; set; }
    public int NodosProcesados { get; set; }
    public string ErrorCode { get; set; }
    public string Details { get; set; }

    public XmlIngestResult()
    {
        ErrorCode = string.Empty;
        Details = string.Empty;
    }
}

public class XmlIngestService
{
    private readonly OrbitNetStore _store;

    // Validaciones RegEx del proyecto
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

        // =========================================================
        // FASE 1: VALIDACIÓN COMPLETA
        // Primero validamos todo el documento.
        // Si algo falla, no tocamos el estado actual del store.
        // =========================================================

        XmlNodeList? satelitesEcuatoriales = doc.SelectNodes("//constelaciones_ecuatoriales/satelite");
        if (satelitesEcuatoriales != null)
        {
            foreach (XmlNode satelite in satelitesEcuatoriales)
            {
                string id = satelite.Attributes?["id"]?.Value ?? string.Empty;
                string ip = satelite.SelectSingleNode("enlace_ip")?.InnerText ?? string.Empty;

                if (!RegexSatelliteId.IsMatch(id))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        XmlNodeList? satelitesPolares = doc.SelectNodes("//orbitas_polares//satelite");
        if (satelitesPolares != null)
        {
            foreach (XmlNode satelite in satelitesPolares)
            {
                string id = satelite.Attributes?["id"]?.Value ?? string.Empty;

                if (!RegexSatelliteId.IsMatch(id))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "El ID de Satelite '" + id + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        XmlNodeList? antenas = doc.SelectNodes("//antenas_terrestres/antena");
        if (antenas != null)
        {
            foreach (XmlNode antena in antenas)
            {
                string coords = antena.SelectSingleNode("coordenadas")?.InnerText ?? string.Empty;
                string ip = antena.SelectSingleNode("ip_nodo")?.InnerText ?? string.Empty;

                if (!RegexCoords.IsMatch(coords))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "Las coordenadas '" + coords + "' no cumplen con el formato RegEx exigido.");
                }

                if (!RegexIpv4.IsMatch(ip))
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "La IP '" + ip + "' no cumple con el formato RegEx exigido.");
                }

                nodos++;
            }
        }

        // =========================================================
        // FASE 2: LIMPIEZA DEL ESTADO ANTERIOR
        // Solo llegamos aquí si TODA la validación pasó.
        // =========================================================
        _store.RedSatellites.Clear();
        _store.Antenas.Clear();
        _store.SatelliteStates.Clear();
        _store.SatelliteRuntime.Clear();

        _store.NodosProcesados = 0;
        _store.CurrentTick = 0;
        _store.EventsProcessed = 0;
        _store.LogicalJumps = 0;
        _store.QueueOccupancyPercentage = 0.0;
        _store.ConfigLoaded = false;

        // =========================================================
        // FASE 3: CARGA REAL DE ESTRUCTURAS
        // Aquí sí insertamos en matriz, runtime y lista de antenas.
        // =========================================================

        // --- 3.1 Carga de satélites ecuatoriales ---
        // Se colocan en una fila fija (row = 1) y se distribuyen por columnas.
        if (satelitesEcuatoriales != null)
        {
            int row = 1;
            int column = 1;

            foreach (XmlNode satelite in satelitesEcuatoriales)
            {
                string id = satelite.Attributes?["id"]?.Value ?? string.Empty;
                string name = satelite.SelectSingleNode("nombre")?.InnerText ?? id;
                string ip = satelite.SelectSingleNode("enlace_ip")?.InnerText ?? "127.0.0.1";

                bool inserted = _store.RedSatellites.Insert(row, column, id, ip);
                if (!inserted)
                {
                    return CrearError("XML_SCHEMA_VIOLATION", "No se pudo insertar el satelite ecuatorial '" + id + "' en la matriz.");
                }

                Satellite satellite = new Satellite
                {
                    Id = id,
                    Name = name,
                    Ip = ip,
                    OrbitType = OrbitType.Ecuatorial,
                    OrbitalAngle = CalcularAnguloInicialEcuatorial(column)
                };

                _store.SatelliteRuntime.Register(satellite);

                column++;
                _store.NodosProcesados++;
            }
        }

        // --- 3.2 Carga de satélites polares ---
        // Cada órbita polar usa una columna fija.
        // Los satélites dentro de esa órbita se distribuyen por filas.
        XmlNodeList? orbitasPolares = doc.SelectNodes("//orbitas_polares/polar");
        if (orbitasPolares != null)
        {
            int polarColumnBase = 20;
            int polarGroupIndex = 0;

            foreach (XmlNode polar in orbitasPolares)
            {
                int fixedColumn = polarColumnBase + polarGroupIndex;
                XmlNodeList? satelitesDentroDeOrbita = polar.SelectNodes("satelite");

                if (satelitesDentroDeOrbita != null)
                {
                    int row = 1;

                    foreach (XmlNode satelite in satelitesDentroDeOrbita)
                    {
                        string id = satelite.Attributes?["id"]?.Value ?? string.Empty;
                        string name = satelite.SelectSingleNode("nombre")?.InnerText ?? id;

                        // Como el XML polar no trae enlace_ip, generamos una IP interna lógica.
                        string ip = GenerarIpInternaPolar(polarGroupIndex + 1, row);

                        bool inserted = _store.RedSatellites.Insert(row, fixedColumn, id, ip);
                        if (!inserted)
                        {
                            return CrearError("XML_SCHEMA_VIOLATION", "No se pudo insertar el satelite polar '" + id + "' en la matriz.");
                        }

                        Satellite satellite = new Satellite
                        {
                            Id = id,
                            Name = name,
                            Ip = ip,
                            OrbitType = OrbitType.Polar,
                            OrbitalAngle = CalcularAnguloInicialPolar(row)
                        };

                        _store.SatelliteRuntime.Register(satellite);

                        row++;
                        _store.NodosProcesados++;
                    }
                }

                polarGroupIndex++;
            }
        }

        // --- 3.3 Carga de antenas terrestres ---
        if (antenas != null)
        {
            foreach (XmlNode antenaNode in antenas)
            {
                string id = antenaNode.Attributes?["id"]?.Value ?? string.Empty;
                string name = antenaNode.SelectSingleNode("nombre")?.InnerText ?? id;
                string coords = antenaNode.SelectSingleNode("coordenadas")?.InnerText ?? string.Empty;
                string ip = antenaNode.SelectSingleNode("ip_nodo")?.InnerText ?? string.Empty;

                GroundAntenna antenna = new GroundAntenna
                {
                    Id = id,
                    Name = name,
                    Coords = coords,
                    Ip = ip,
                    PosicionAngular = CalcularAnguloDesdeCoordenadas(coords)
                };

                _store.Antenas.Add(antenna);
                _store.NodosProcesados++;
            }
        }

        // Si llegamos aquí, la carga quedó lista para el motor.
        _store.ConfigLoaded = true;

        return new XmlIngestResult
        {
            Success = true,
            NodosProcesados = _store.NodosProcesados
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

    // Para satélites ecuatoriales usamos la columna para definir un ángulo inicial.
    private double CalcularAnguloInicialEcuatorial(int column)
    {
        return ((column - 1) * 15.0) % 360.0;
    }

    // Para satélites polares usamos la fila para definir un ángulo inicial.
    private double CalcularAnguloInicialPolar(int row)
    {
        return ((row - 1) * 30.0) % 360.0;
    }

    // Genera una IP interna lógica para satélites polares cuando el XML no trae IP.
    private string GenerarIpInternaPolar(int polarGroupIndex, int row)
    {
        return "10.250." + polarGroupIndex + "." + row;
    }

    // Conversión simple de coordenadas a un ángulo utilizable.
    // Aquí tomamos la longitud como base.
    private double CalcularAnguloDesdeCoordenadas(string coords)
    {
        if (string.IsNullOrWhiteSpace(coords))
        {
            return 0.0;
        }

        string[] partes = coords.Split(',');
        if (partes.Length != 2)
        {
            return 0.0;
        }

        if (!double.TryParse(partes[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double longitud))
        {
            return 0.0;
        }

        double angulo = longitud + 180.0;

        while (angulo >= 360.0)
        {
            angulo -= 360.0;
        }

        while (angulo < 0.0)
        {
            angulo += 360.0;
        }

        return angulo;
    }
}