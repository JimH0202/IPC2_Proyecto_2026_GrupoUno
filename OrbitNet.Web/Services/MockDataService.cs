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

    public static SimulationViewModel GetSimulationViewModel(int additionalTicks)
    {
        var model = GetSimulationViewModel();
        model.CurrentTick += additionalTicks;
        model.ProcessedMessages += additionalTicks * 3;
        return model;
    }

    public static SimulationViewModel GetSimulationViewModel(bool isRunning)
    {
        var model = GetSimulationViewModel();
        model.IsSimulationRunning = isRunning;
        return model;
    }

    public static object GetMatrixData()
    {
        return new
        {
            rows = 10,
            columns = 12,
            occupiedNodes = 14,
            svgContent = "<svg width='360' height='200' xmlns='http://www.w3.org/2000/svg'><rect width='360' height='200' fill='#f8f9fa' stroke='#ddd' /><text x='20' y='40' font-family='Arial' font-size='18'>Matriz dispersa de ejemplo</text></svg>"
        };
    }

    public static object GetMemoryReportData()
    {
        return new
        {
            title = "Mapa de memoria",
            description = "Representación de la estructura de memoria del sistema.",
            generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            svgContent = "<svg width='380' height='220' xmlns='http://www.w3.org/2000/svg'><rect width='380' height='220' fill='#f1f3f5' stroke='#ced4da'/><text x='20' y='40' font-family='Arial' font-size='16'>Reporte de MemoryLayout</text></svg>"
        };
    }

    public static object GetLiveSimulationData()
    {
        return new
        {
            currentTick = 150,
            activeSatellites = 28,
            pendingMessages = 15,
            processedMessages = 450,
            isRunning = true
        };
    }

    public static RoutesData GetRoutesData()
    {
        return new RoutesData
        {
            ActiveRoutes = 2,
            TotalRoutes = 3,
            Routes = new[]
            {
                new RouteItem { Id = "R1", Source = "SAT-001", Destination = "ANT-N1", Status = "activa", Hops = 2, Packets = 10 },
                new RouteItem { Id = "R2", Source = "SAT-002", Destination = "ANT-S1", Status = "activa", Hops = 3, Packets = 8 },
                new RouteItem { Id = "R3", Source = "SAT-003", Destination = "ANT-N2", Status = "inactiva", Hops = 0, Packets = 0 }
            }
        };
    }

    public static BuffersData GetBuffersData()
    {
        return new BuffersData
        {
            AverageOccupancy = 42.3,
            Buffers = new[]
            {
                new BufferItem { Id = "BUF-001", Status = "normal", Occupied = 45, OccupancyPercent = 45.0 },
                new BufferItem { Id = "BUF-002", Status = "alta", Occupied = 72, OccupancyPercent = 72.0 },
                new BufferItem { Id = "BUF-003", Status = "baja", Occupied = 10, OccupancyPercent = 10.0 }
            }
        };
    }
}

public class RoutesData
{
    public int ActiveRoutes { get; set; }
    public int TotalRoutes { get; set; }
    public required RouteItem[] Routes { get; set; }
}

public class RouteItem
{
    public required string Id { get; set; }
    public required string Source { get; set; }
    public required string Destination { get; set; }
    public required string Status { get; set; }
    public int Hops { get; set; }
    public int Packets { get; set; }
}

public class BuffersData
{
    public double AverageOccupancy { get; set; }
    public required BufferItem[] Buffers { get; set; }
}

public class BufferItem
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public int Occupied { get; set; }
    public double OccupancyPercent { get; set; }
}