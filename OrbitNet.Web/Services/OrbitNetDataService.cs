using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.DataStructures.Matrix;
using OrbitNet.Web.Models.Entities;
using OrbitNet.Web.Models.Enums;
using OrbitNet.Web.Models.ViewModels;
using OrbitNet.Web.Services.Graphviz;

namespace OrbitNet.Web.Services;

public class OrbitNetDataService
{
    private readonly OrbitNetStore _store;
    private readonly AppInstanceSettings _settings;

    public OrbitNetDataService(OrbitNetStore store, IOptions<AppInstanceSettings> settings)
    {
        _store = store;
        _settings = settings.Value;
    }

    public DashboardViewModel GetDashboardViewModel()
    {
        return new DashboardViewModel
        {
            CurrentTick = _store.CurrentTick,
            ActiveSatellites = _store.ActiveSatellites,
            InactiveSatellites = 0,
            PendingMessages = _store.PendingMessages,
            ProcessedMessages = _store.EventsProcessed,
            TotalAntennas = _store.TotalAntennas,
            Hemisphere = _settings.Hemisphere,
            Port = _settings.Port,
            RemoteHemisphereUrl = _settings.RemoteHemisphereUrl,
            IsSimulationRunning = _store.IsSimulationRunning,
            Satellites = _store.Satellites.Select(s => MapSatellite(s)).ToList()
        };
    }

    public SimulationViewModel GetSimulationViewModel()
    {
        return new SimulationViewModel
        {
            CurrentTick = _store.CurrentTick,
            ActiveSatellites = _store.ActiveSatellites,
            PendingMessages = _store.PendingMessages,
            ProcessedMessages = _store.EventsProcessed,
            IsSimulationRunning = _store.IsSimulationRunning,
            Satellites = _store.Satellites.Select(s => MapSatellite(s)).ToList()
        };
    }

    public List<SatelliteViewModel> GetSatelliteList()
    {
        return _store.Satellites.Select(s => MapSatellite(s)).ToList();
    }

    public SatelliteViewModel GetSatelliteDetails(string id)
    {
        var sat = _store.Satellites.FirstOrDefault(s => s.Id == id);
        if (sat == null) return new SatelliteViewModel { Id = id, Name = "Desconocido" };
        return MapSatellite(sat);
    }

    public UploadViewModel GetUploadResultViewModel(string fileName, bool success, string message, int satLoaded, int antLoaded, int orbLoaded, List<string>? errors = null)
    {
        return new UploadViewModel
        {
            FileName = fileName,
            Success = success,
            Message = message,
            SatellitesLoaded = satLoaded,
            AntennasLoaded = antLoaded,
            PolarOrbitsLoaded = orbLoaded,
            Errors = errors ?? new List<string>()
        };
    }

    public MatrixViewModel GetMatrixViewModel()
    {
        int rows = 0;
        int cols = 0;
        var firstRow = _store.RedSatellites.GetFirstRowHeader();
        if (firstRow != null)
        {
            var cur = firstRow;
            while (cur != null) { rows++; cur = cur.Next; }
        }
        var firstCol = _store.RedSatellites.GetColumnHeader(1);
        if (firstCol != null)
        {
            var cur = firstCol;
            while (cur != null) { cols++; cur = cur.Next; }
        }

        return new MatrixViewModel
        {
            Rows = rows,
            Columns = cols,
            OccupiedNodes = _store.RedSatellites.Count,
            SvgContent = GenerateMatrixSvg()
        };
    }

    public ReportViewModel GetReportViewModel(string reportName)
    {
        var model = new ReportViewModel
        {
            GeneratedAt = DateTime.Now,
            SvgContent = "",
            Metadata = new Dictionary<string, string>()
        };

        switch (reportName)
        {
            case "MemoryLayout":
                model.Title = "Mapa de memoria";
                model.Description = "Representación de la estructura de memoria del sistema.";
                model.SvgContent = GenerateMemoryLayoutSvg();
                break;
            case "Routing":
                model.Title = "Ruta de relés";
                model.Description = "Vista del enrutamiento de mensajes entre satélites y antenas.";
                model.SvgContent = GenerateRoutingSvg();
                break;
            case "Buffers":
                model.Title = "Capacidad de buffers";
                model.Description = "Detalle de ocupación y capacidad de los buffers.";
                model.SvgContent = GenerateBuffersSvg();
                break;
            default:
                model.Title = "Reporte";
                model.Description = "Reporte de sistema";
                break;
        }

        return model;
    }

    public List<LogViewModel> GetLogEntries()
    {
        return _store.LogAuditoria.GetAll().Select(entry => new LogViewModel
        {
            Timestamp = entry.Timestamp,
            Level = entry.Severity,
            Event = entry.Message,
            Details = entry.Message
        }).ToList();
    }

    private SatelliteViewModel MapSatellite(Satellite s)
    {
        var state = _store.SatelliteStates.FindBySatelliteId(s.Id);
        string orbitType = "Ecuatorial";
        if (s.Id.StartsWith("SAT-POL-"))
            orbitType = "Polar";

        return new SatelliteViewModel
        {
            Id = s.Id,
            Name = s.Name,
            OrbitId = orbitType,
            Type = orbitType,
            X = state?.CurrentColumn ?? 0,
            Y = state?.CurrentRow ?? 0,
            IsActive = true,
            MessagesSent = 0,
            MessagesReceived = 0
        };
    }

    private string GenerateMatrixSvg()
    {
        var cells = new List<SparseMatrixCell>();
        var rowHeader = _store.RedSatellites.GetFirstRowHeader();
        while (rowHeader != null)
        {
            var current = rowHeader.Access;
            while (current != null)
            {
                cells.Add(new SparseMatrixCell
                {
                    Row = current.Row,
                    Column = current.Column,
                    SatelliteId = current.SatelliteId,
                    Capacity = 100,
                    Occupied = 0,
                    OccupancyPercent = 0,
                    Status = "active",
                    Messages = ""
                });
                current = current.Right;
            }
            rowHeader = rowHeader.Next;
        }

        var data = new SparseMatrixReportData
        {
            Rows = cells.Any() ? cells.Max(c => c.Row) : 0,
            Columns = cells.Any() ? cells.Max(c => c.Column) : 0,
            Cells = cells
        };

        return MatrixGraphGenerator.BuildDot(data);
    }

    private string GenerateMemoryLayoutSvg()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("digraph MemoryLayout {");
        sb.AppendLine("  rankdir=TB;");
        sb.AppendLine("  node [shape=record, fontname=\"Consolas\", fontsize=10];");
        sb.AppendLine();
        sb.AppendLine($"  store [label=\"{{OrbitNetStore|satellites: {_store.ActiveSatellites}|antennas: {_store.TotalAntennas}|tick: {_store.CurrentTick}}}\"];");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string GenerateRoutingSvg()
    {
        var routes = new List<RouteReportItem>();
        int index = 1;
        var rowHeader = _store.RedSatellites.GetFirstRowHeader();
        while (rowHeader != null)
        {
            var current = rowHeader.Access;
            while (current != null)
            {
                routes.Add(new RouteReportItem
                {
                    Id = $"R{index}",
                    Source = current.SatelliteId,
                    Destination = current.IpAddress,
                    Hops = 0,
                    Status = "active",
                    Packets = 0
                });
                index++;
                current = current.Right;
            }
            rowHeader = rowHeader.Next;
        }

        var data = new RouteReportData
        {
            Routes = routes,
            TotalRoutes = routes.Count,
            ActiveRoutes = routes.Count
        };

        return RouteGraphGenerator.BuildDot(data);
    }

    private string GenerateBuffersSvg()
    {
        var buffers = _store.Satellites.Select(s =>
        {
            int occupied = s.PaquetesABordo?.Count ?? 0;
            int capacity = 100;
            int percent = capacity > 0 ? (int)((double)occupied / capacity * 100) : 0;
            return new BufferReportItem
            {
                Id = $"BUF-{s.Id}",
                Satellite = s.Id,
                Capacity = capacity,
                Occupied = occupied,
                OccupancyPercent = percent,
                Status = percent >= 80 ? "critico" : percent >= 50 ? "normal" : "bajo"
            };
        }).ToList();

        var data = new BufferReportData
        {
            Buffers = buffers,
            TotalBuffers = buffers.Count,
            AverageOccupancy = buffers.Any() ? (int)buffers.Average(b => b.OccupancyPercent) : 0
        };

        return BufferGraphGenerator.BuildDot(data);
    }
}
