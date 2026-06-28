using System.Globalization;
using System.Linq;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services.Graphviz;

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
            Hemisphere = "North",
            IsSimulationRunning = true,
            Satellites = GetSatelliteList()
        };
    }

    private static bool IsSpanishCulture()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.StartsWith("es", StringComparison.OrdinalIgnoreCase);
    }

    private static string Localize(string english, string spanish)
    {
        return IsSpanishCulture() ? spanish : english;
    }

    public static SimulationViewModel GetSimulationViewModel(int additionalTicks = 0, bool isRunning = true)
    {
        return new SimulationViewModel
        {
            CurrentTick = 150 + additionalTicks,
            ActiveSatellites = 28,
            PendingMessages = 15,
            ProcessedMessages = 450,
            IsSimulationRunning = isRunning,
            Satellites = GetSatelliteList()
        };
    }

    public static List<SatelliteViewModel> GetSatelliteList()
    {
        return new List<SatelliteViewModel>
        {
            new () { Id = "SAT-001", Name = "Tierra-1", OrbitId = "ORB-N1", Type = "Polar", X = 120, Y = 210, IsActive = true, MessagesSent = 42, MessagesReceived = 38, Resends = 5, DiscardedMessages = 2, LastUpdate = DateTime.Now.AddMinutes(-5), RecentHistory = GetLogEntries() },
            new () { Id = "SAT-002", Name = "Luna-2", OrbitId = "ORB-S3", Type = "Geoestacionaria", X = 310, Y = 140, IsActive = false, MessagesSent = 28, MessagesReceived = 30, Resends = 3, DiscardedMessages = 1, LastUpdate = DateTime.Now.AddMinutes(-20), RecentHistory = GetLogEntries() },
            new () { Id = "SAT-003", Name = "Solar-3", OrbitId = "ORB-N4", Type = "Heliostacionaria", X = 520, Y = 90, IsActive = true, MessagesSent = 55, MessagesReceived = 49, Resends = 7, DiscardedMessages = 4, LastUpdate = DateTime.Now.AddMinutes(-2), RecentHistory = GetLogEntries() }
        };
    }

    public static SatelliteViewModel GetSatelliteDetails(string id)
    {
        return GetSatelliteList().FirstOrDefault(s => s.Id == id) ?? new SatelliteViewModel
        {
            Id = id,
            Name = Localize("Unknown satellite", "Satélite desconocido"),
            OrbitId = "N/A",
            Type = "N/A",
            X = 0,
            Y = 0,
            IsActive = false,
            MessagesSent = 0,
            MessagesReceived = 0,
            Resends = 0,
            DiscardedMessages = 0,
            LastUpdate = DateTime.Now,
            RecentHistory = GetLogEntries()
        };
    }

    public static UploadViewModel GetUploadResultViewModel()
    {
        return new UploadViewModel
        {
            FileName = "configuracion_norte.xml",
            Success = true,
            Message = Localize("Upload completed successfully.", "Carga completada con éxito."),
            SatellitesLoaded = 6,
            AntennasLoaded = 2,
            PolarOrbitsLoaded = 3,
            Errors = new List<string>()
        };
    }

    public static MatrixViewModel GetMatrixViewModel()
    {
        var report = GetSparseMatrixReportData();
        var dot = MatrixGraphGenerator.BuildDot(report);
        var svg = DotCompiler.CompileDotToSvg(dot) ?? GetPlaceholderMatrixSvg();

        return new MatrixViewModel
        {
            Rows = report.Rows,
            Columns = report.Columns,
            OccupiedNodes = report.Cells.Count,
            SvgContent = svg
        };
    }

    public static SparseMatrixReportData GetSparseMatrixReportData()
    {
        return new SparseMatrixReportData
        {
            Rows = 6,
            Columns = 6,
            Cells = new List<SparseMatrixCell>
            {
                new() { Row = 1, Column = 2, SatelliteId = "SAT-001", Capacity = 100, Occupied = 78, OccupancyPercent = 78, Messages = "M001,M008" },
                new() { Row = 1, Column = 4, SatelliteId = "SAT-002", Capacity = 100, Occupied = 45, OccupancyPercent = 45, Messages = "M002" },
                new() { Row = 2, Column = 1, SatelliteId = "SAT-003", Capacity = 100, Occupied = 92, OccupancyPercent = 92, Messages = "M003,M004,M007" },
                new() { Row = 3, Column = 5, SatelliteId = "SAT-004", Capacity = 100, Occupied = 23, OccupancyPercent = 23, Messages = "" },
                new() { Row = 4, Column = 3, SatelliteId = "SAT-005", Capacity = 100, Occupied = 67, OccupancyPercent = 67, Messages = "M005" }
            }
        };
    }

    private static string GetPlaceholderMatrixSvg()
    {
        return "<svg width='520' height='260' xmlns='http://www.w3.org/2000/svg'><rect width='520' height='260' fill='#f8f9fa' stroke='#ddd' /><text x='20' y='40' font-family='Arial' font-size='18'>Matriz dispersa de ejemplo</text></svg>";
    }

    public static ReportViewModel GetReportViewModel(string reportName)
    {
        var model = new ReportViewModel
        {
            GeneratedAt = DateTime.Now
        };

        switch (reportName)
        {
            case "MemoryLayout":
            {
                model.Title = Localize("Memory Layout", "Mapa de memoria");
                model.Description = Localize("Representation of the system memory structure.", "Representación de la estructura de memoria del sistema.");
                var report = GetMemoryReportData();
                model.Metadata[Localize("Total Nodes", "Nodos totales")] = report.TotalNodes.ToString();
                model.Metadata[Localize("Height", "Altura")] = report.Height.ToString();
                var dot = AvlGraphGenerator.BuildDot(report);
                model.SvgContent = DotCompiler.CompileDotToSvg(dot) ?? GetPlaceholderSvg(model.Title);
                break;
            }
            case "Routing":
            {
                model.Title = Localize("Relay Routing", "Ruta de relés");
                model.Description = Localize("Message routing view between satellites and antennas.", "Vista del enrutamiento de mensajes entre satélites y antenas.");
                var report = GetRoutesData();
                model.Metadata[Localize("Total Routes", "Rutas totales")] = report.TotalRoutes.ToString();
                model.Metadata[Localize("Active Routes", "Rutas activas")] = report.ActiveRoutes.ToString();
                var dot = RouteGraphGenerator.BuildDot(report);
                model.SvgContent = DotCompiler.CompileDotToSvg(dot) ?? GetPlaceholderSvg(model.Title);
                break;
            }
            case "Buffers":
            {
                model.Title = Localize("Buffer Capacity", "Capacidad de buffers");
                model.Description = Localize("Occupancy and capacity details for buffers.", "Detalle de ocupación y capacidad de los buffers.");
                var report = GetBuffersData();
                model.Metadata[Localize("Total Buffers", "Buffers totales")] = report.TotalBuffers.ToString();
                model.Metadata[Localize("Average Occupancy", "Ocupación promedio")] = report.AverageOccupancy + "%";
                var dot = BufferGraphGenerator.BuildDot(report);
                model.SvgContent = DotCompiler.CompileDotToSvg(dot) ?? GetPlaceholderSvg(model.Title);
                break;
            }
            case "Matrix":
            {
                model.Title = Localize("Sparse Matrix", "Matriz dispersa");
                model.Description = Localize("Spatial layout of the sparse matrix and occupancy state.", "Diseño espacial de la matriz dispersa y estado de ocupación.");
                var report = GetSparseMatrixReportData();
                model.Metadata[Localize("Rows", "Filas")] = report.Rows.ToString();
                model.Metadata[Localize("Columns", "Columnas")] = report.Columns.ToString();
                var dot = MatrixGraphGenerator.BuildDot(report);
                model.SvgContent = DotCompiler.CompileDotToSvg(dot) ?? GetPlaceholderSvg(model.Title);
                break;
            }
            default:
                model.Title = Localize("Generic Report", "Reporte genérico");
                model.Description = Localize("Example system report.", "Reporte de sistema de ejemplo.");
                model.SvgContent = GetPlaceholderSvg(model.Title);
                break;
        }

        return model;
    }

    private static string GetPlaceholderSvg(string title)
    {
        return $"<svg width='420' height='220' xmlns='http://www.w3.org/2000/svg'><rect width='420' height='220' fill='#f1f3f5' stroke='#ced4da'/><text x='20' y='40' font-family='Arial' font-size='16'>Reporte: {title}</text></svg>";
    }

    public static List<LogViewModel> GetLogEntries()
    {
        return new List<LogViewModel>
        {
            new () { Timestamp = DateTime.Now.AddMinutes(-12), Level = "Info", Event = Localize("Simulation started", "Inicio de simulación"), Details = Localize("The simulation engine started successfully.", "El motor de simulación inició correctamente.") },
            new () { Timestamp = DateTime.Now.AddMinutes(-8), Level = "Warning", Event = Localize("Message delay", "Retraso en mensaje"), Details = Localize("Packet SAT-002 was held 4s in the buffer.", "El paquete SAT-002 se demoró 4s en el buffer.") },
            new () { Timestamp = DateTime.Now.AddMinutes(-2), Level = "Info", Event = Localize("Message processed", "Mensaje procesado"), Details = Localize("Five messages were processed in the last tick.", "Se procesaron 5 mensajes en el último tick.") }
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
    public static RouteReportData GetRoutesData()
    {
        return new RouteReportData
        {
            Routes = new List<RouteReportItem>
            {
                new() { Id = "R001", Source = "SAT-001", Destination = "ANT-N1", Hops = 1, Status = Localize("active", "activa"), Packets = 42 },
                new() { Id = "R002", Source = "SAT-002", Destination = "ANT-N2", Hops = 2, Status = Localize("active", "activa"), Packets = 28 },
                new() { Id = "R003", Source = "SAT-003", Destination = "ANT-N3", Hops = 1, Status = Localize("inactive", "inactiva"), Packets = 0 },
                new() { Id = "R004", Source = "ANT-N1", Destination = "SAT-001", Hops = 1, Status = Localize("active", "activa"), Packets = 38 },
                new() { Id = "R005", Source = "ANT-N2", Destination = "SAT-002", Hops = 2, Status = Localize("active", "activa"), Packets = 30 }
            },
            TotalRoutes = 5,
            ActiveRoutes = 4
        };
    }

    public static RelayStatusDto GetRelayStatus()
    {
        var routes = GetRoutesData();
        var buffers = GetBuffersData();

        return new RelayStatusDto
        {
            ActiveRelays = routes.ActiveRoutes,
            InactiveRelays = Math.Max(0, routes.TotalRoutes - routes.ActiveRoutes),
            TotalPacketsProcessed = routes.Routes.Sum(r => r.Packets),
            AvgQueueOccupancy = buffers.AverageOccupancy
        };
    }

    // Datos para buffers (ABB)
    public static BufferReportData GetBuffersData()
    {
        return new BufferReportData
        {
            Buffers = new List<BufferReportItem>
            {
                new() { Id = "BUF-001", Satellite = "SAT-001", Capacity = 100, Occupied = 78, OccupancyPercent = 78, Status = Localize("high", "alto") },
                new() { Id = "BUF-002", Satellite = "SAT-002", Capacity = 100, Occupied = 45, OccupancyPercent = 45, Status = Localize("medium", "medio") },
                new() { Id = "BUF-003", Satellite = "SAT-003", Capacity = 100, Occupied = 92, OccupancyPercent = 92, Status = Localize("critical", "critico") },
                new() { Id = "BUF-004", Satellite = "SAT-004", Capacity = 100, Occupied = 23, OccupancyPercent = 23, Status = Localize("low", "bajo") },
                new() { Id = "BUF-005", Satellite = "SAT-005", Capacity = 100, Occupied = 67, OccupancyPercent = 67, Status = Localize("medium", "medio") }
            },
            TotalBuffers = 5,
            AverageOccupancy = 61
        };
    }

    // Datos para reporte de memoria AVL
    public static AvlReportData GetMemoryReportData()
    {
        return new AvlReportData
        {
            Title = "Estructura de Memoria (AVL)",
            Nodes = new List<AvlReportNode>
            {
                new() { Id = "1", Value = 50, Children = new[] { 2, 3 } },
                new() { Id = "2", Value = 30, Children = new[] { 4, 5 } },
                new() { Id = "3", Value = 70, Children = Array.Empty<int>() },
                new() { Id = "4", Value = 20, Children = Array.Empty<int>() },
                new() { Id = "5", Value = 40, Children = Array.Empty<int>() }
            },
            TotalNodes = 5,
            Height = 3
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
