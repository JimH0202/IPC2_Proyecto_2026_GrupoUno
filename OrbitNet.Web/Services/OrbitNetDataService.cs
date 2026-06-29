using Microsoft.Extensions.Options;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Models.ViewModels;

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
            Satellites = _store.Satellites.Select(s => new SatelliteViewModel
            {
                Id = s.Id,
                Name = s.Name,
                OrbitId = "",
                Type = "",
                X = 0,
                Y = 0,
                IsActive = true,
                MessagesSent = 0,
                MessagesReceived = 0
            }).ToList()
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
            Satellites = _store.Satellites.Select(s => new SatelliteViewModel
            {
                Id = s.Id,
                Name = s.Name,
                OrbitId = "",
                Type = "",
                IsActive = true
            }).ToList()
        };
    }

    public SimulationViewModel GetSimulationViewModel(int additionalTicks)
    {
        var model = GetSimulationViewModel();
        model.CurrentTick += additionalTicks;
        model.ProcessedMessages += additionalTicks * 3;
        return model;
    }

    public List<SatelliteViewModel> GetSatelliteList()
    {
        return _store.Satellites.Select(s => new SatelliteViewModel
        {
            Id = s.Id,
            Name = s.Name,
            OrbitId = "",
            Type = "",
            X = 0,
            Y = 0,
            IsActive = true,
            MessagesSent = 0,
            MessagesReceived = 0
        }).ToList();
    }

    public SatelliteViewModel GetSatelliteDetails(string id)
    {
        var sat = _store.Satellites.FirstOrDefault(s => s.Id == id);
        if (sat == null) return new SatelliteViewModel { Id = id, Name = "Desconocido" };
        return new SatelliteViewModel
        {
            Id = sat.Id,
            Name = sat.Name,
            OrbitId = "",
            Type = "",
            X = 0,
            Y = 0,
            IsActive = true,
            MessagesSent = 0,
            MessagesReceived = 0
        };
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
        var firstRow = _store.Matrix.GetFirstRowHeader();
        if (firstRow != null)
        {
            var cur = firstRow;
            while (cur != null) { rows++; cur = cur.Next; }
        }
        var firstCol = _store.Matrix.GetColumnHeader(1);
        if (firstCol != null)
        {
            var cur = firstCol;
            while (cur != null) { cols++; cur = cur.Next; }
        }

        return new MatrixViewModel
        {
            Rows = rows,
            Columns = cols,
            OccupiedNodes = _store.Matrix.Count,
            SvgContent = ""
        };
    }

    public ReportViewModel GetReportViewModel(string reportName)
    {
        return new ReportViewModel
        {
            Title = reportName switch
            {
                "MemoryLayout" => "Mapa de memoria",
                "Routing" => "Ruta de relés",
                "Buffers" => "Capacidad de buffers",
                _ => "Reporte"
            },
            Description = reportName switch
            {
                "MemoryLayout" => "Representación de la estructura de memoria del sistema.",
                "Routing" => "Vista del enrutamiento de mensajes entre satélites y antenas.",
                "Buffers" => "Detalle de ocupación y capacidad de los buffers.",
                _ => "Reporte de sistema"
            },
            GeneratedAt = DateTime.Now,
            SvgContent = "",
            Metadata = new Dictionary<string, string>()
        };
    }

    public List<LogViewModel> GetLogEntries()
    {
        return new List<LogViewModel>();
    }
}
