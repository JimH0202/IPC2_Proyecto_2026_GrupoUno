using OrbitNet.Web.Models.ViewModels;

namespace OrbitNet.Web.Services;

public static class MockDataService
{
    public static DashboardViewModel GetDashboardViewModel()
    {
        return new DashboardViewModel
        {
            CurrentTick = 150,
            ActiveSatellites = 28,
            InactiveSatellites = 2,
            PendingMessages = 15,
            ProcessedMessages = 450,
            TotalAntennas = 6,
            Hemisphere = "Norte",
            IsSimulationRunning = true,
            Satellites = GetSatelliteList()
        };
    }

    public static SimulationViewModel GetSimulationViewModel()
    {
        return new SimulationViewModel
        {
            CurrentTick = 150,
            ActiveSatellites = 28,
            PendingMessages = 15,
            ProcessedMessages = 450,
            Satellites = GetSatelliteList()
        };
    }

    public static List<SatelliteViewModel> GetSatelliteList()
    {
        return new List<SatelliteViewModel>
        {
            new () { Id = "SAT-001", Name = "Tierra-1", OrbitId = "ORB-N1", Type = "Polar", X = 120, Y = 210, IsActive = true, MessagesSent = 42, MessagesReceived = 38 },
            new () { Id = "SAT-002", Name = "Luna-2", OrbitId = "ORB-S3", Type = "Geoestacionaria", X = 310, Y = 140, IsActive = false, MessagesSent = 28, MessagesReceived = 30 },
            new () { Id = "SAT-003", Name = "Solar-3", OrbitId = "ORB-N4", Type = "Heliostacionaria", X = 520, Y = 90, IsActive = true, MessagesSent = 55, MessagesReceived = 49 }
        };
    }

    public static SatelliteViewModel GetSatelliteDetails(string id)
    {
        return GetSatelliteList().FirstOrDefault(s => s.Id == id) ?? new SatelliteViewModel
        {
            Id = id,
            Name = "Satélite desconocido",
            OrbitId = "N/A",
            Type = "N/A",
            X = 0,
            Y = 0,
            IsActive = false,
            MessagesSent = 0,
            MessagesReceived = 0
        };
    }

    public static UploadViewModel GetUploadResultViewModel()
    {
        return new UploadViewModel
        {
            FileName = "configuracion_norte.xml",
            Success = true,
            Message = "Carga completada con éxito.",
            SatellitesLoaded = 6,
            AntennasLoaded = 2,
            PolarOrbitsLoaded = 3,
            Errors = new List<string>()
        };
    }

    public static MatrixViewModel GetMatrixViewModel()
    {
        const string svg = "<svg width='360' height='200' xmlns='http://www.w3.org/2000/svg'><rect width='360' height='200' fill='#f8f9fa' stroke='#ddd' /><text x='20' y='40' font-family='Arial' font-size='18'>Matriz dispersa de ejemplo</text></svg>";

        return new MatrixViewModel
        {
            Rows = 10,
            Columns = 12,
            OccupiedNodes = 14,
            SvgContent = svg
        };
    }

    public static ReportViewModel GetReportViewModel(string reportName)
    {
        var model = new ReportViewModel
        {
            GeneratedAt = DateTime.Now,
            SvgContent = "<svg width='380' height='220' xmlns='http://www.w3.org/2000/svg'><rect width='380' height='220' fill='#f1f3f5' stroke='#ced4da'/><text x='20' y='40' font-family='Arial' font-size='16'>Reporte de " + reportName + "</text></svg>"
        };

        switch (reportName)
        {
            case "MemoryLayout":
                model.Title = "Mapa de memoria";
                model.Description = "Representación de la estructura de memoria del sistema.";
                break;
            case "Routing":
                model.Title = "Ruta de relés";
                model.Description = "Vista del enrutamiento de mensajes entre satélites y antenas.";
                break;
            case "Buffers":
                model.Title = "Capacidad de buffers";
                model.Description = "Detalle de ocupación y capacidad de los buffers.";
                break;
            default:
                model.Title = "Reporte genérico";
                model.Description = "Reporte de sistema de ejemplo.";
                break;
        }

        return model;
    }

    public static List<LogViewModel> GetLogEntries()
    {
        return new List<LogViewModel>
        {
            new () { Timestamp = DateTime.Now.AddMinutes(-12), Level = "Info", Event = "Inicio de simulación", Details = "El motor de simulación inició correctamente." },
            new () { Timestamp = DateTime.Now.AddMinutes(-8), Level = "Warning", Event = "Retraso en mensaje", Details = "El paquete SAT-002 se demoró 4s en el buffer." },
            new () { Timestamp = DateTime.Now.AddMinutes(-2), Level = "Info", Event = "Mensaje procesado", Details = "Se procesaron 5 mensajes en el último tick." }
        };
    }

    // Datos para dashboard - Matriz dispersa con puntos
    public static object GetMatrixData()
    {
        return new
        {
            rows = 10,
            columns = 12,
            occupiedNodes = 14,
            nodes = new[] 
            {
                new { x = 20, y = 30 }, new { x = 50, y = 45 }, new { x = 80, y = 20 },
                new { x = 120, y = 60 }, new { x = 150, y = 40 }, new { x = 180, y = 70 },
                new { x = 220, y = 35 }, new { x = 250, y = 55 }, new { x = 280, y = 25 },
                new { x = 310, y = 65 }, new { x = 340, y = 45 }, new { x = 360, y = 50 },
                new { x = 390, y = 30 }, new { x = 420, y = 60 }
            }
        };
    }

    // Datos para rutas/relés
    public static object GetRoutesData()
    {
        return new
        {
            routes = new[]
            {
                new { id = "R001", source = "SAT-001", destination = "ANT-N1", hops = 1, status = "activa", packets = 42 },
                new { id = "R002", source = "SAT-002", destination = "ANT-N2", hops = 2, status = "activa", packets = 28 },
                new { id = "R003", source = "SAT-003", destination = "ANT-N3", hops = 1, status = "inactiva", packets = 0 },
                new { id = "R004", source = "ANT-N1", destination = "SAT-001", hops = 1, status = "activa", packets = 38 },
                new { id = "R005", source = "ANT-N2", destination = "SAT-002", hops = 2, status = "activa", packets = 30 }
            },
            totalRoutes = 5,
            activeRoutes = 4
        };
    }

    // Datos para buffers (ABB)
    public static object GetBuffersData()
    {
        return new
        {
            buffers = new[]
            {
                new { id = "BUF-001", satellite = "SAT-001", capacity = 100, occupied = 78, occupancyPercent = 78, status = "alto" },
                new { id = "BUF-002", satellite = "SAT-002", capacity = 100, occupied = 45, occupancyPercent = 45, status = "medio" },
                new { id = "BUF-003", satellite = "SAT-003", capacity = 100, occupied = 92, occupancyPercent = 92, status = "critico" },
                new { id = "BUF-004", satellite = "SAT-004", capacity = 100, occupied = 23, occupancyPercent = 23, status = "bajo" },
                new { id = "BUF-005", satellite = "SAT-005", capacity = 100, occupied = 67, occupancyPercent = 67, status = "medio" }
            },
            totalBuffers = 5,
            averageOccupancy = 61
        };
    }

    // Datos para reporte de memoria AVL
    public static object GetMemoryReportData()
    {
        return new
        {
            title = "Estructura de Memoria (AVL)",
            nodes = new[]
            {
                new { id = "1", value = 50, balance = 0, children = new int[] { 2, 3 } },
                new { id = "2", value = 30, balance = 0, children = new int[] { 4, 5 } },
                new { id = "3", value = 70, balance = 0, children = new int[] { } },
                new { id = "4", value = 20, balance = 0, children = new int[] { } },
                new { id = "5", value = 40, balance = 0, children = new int[] { } }
            },
            totalNodes = 5,
            height = 3
        };
    }

    // Datos simulados de actualización en tiempo real
    public static object GetLiveSimulationData()
    {
        var random = new Random();
        return new
        {
            tick = 150 + random.Next(10),
            activeSatellites = 28,
            messagesPerTick = random.Next(5, 15),
            processedThisTick = random.Next(20, 60),
            avgLatency = random.Next(100, 500),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}
