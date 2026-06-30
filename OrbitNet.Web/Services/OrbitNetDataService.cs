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
            Satellites = MapSatellites()
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
            Satellites = MapSatellites()
        };
    }

    public List<SatelliteViewModel> GetSatelliteList()
    {
        return MapSatellites();
    }

    public SimulationViewModel GetSimulationViewModel(int additionalTicks)
    {
        var model = GetSimulationViewModel();
        model.CurrentTick += additionalTicks;
        model.ProcessedMessages += additionalTicks * 3;
        return model;
    }

    public SatelliteViewModel GetSatelliteDetails(string id)
    {
        var sat = _store.Satellites.Find(s => s.Id == id);
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
        var result = new List<LogViewModel>();
        _store.LogAuditoria.ForEach(entry => result.Add(new LogViewModel
        {
            Timestamp = entry.Timestamp,
            Level = entry.Severity,
            Event = entry.Message,
            Details = entry.Message
        }));
        return result;
    }

    private List<SatelliteViewModel> MapSatellites()
    {
        var result = new List<SatelliteViewModel>();
        _store.Satellites.ForEach(s => result.Add(MapSatellite(s)));
        return result;
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
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 800 500' font-family='Consolas,monospace' font-size='12'>");
        sb.AppendLine("<rect width='800' height='500' fill='#f8f9fa' rx='8'/>");
        sb.AppendLine("<text x='20' y='30' font-size='16' font-weight='bold' fill='#2c3e50'>Matriz Dispersa - Red Satelital</text>");

        var rowHeader = _store.RedSatellites.GetFirstRowHeader();
        int y = 60;
        while (rowHeader != null)
        {
            sb.AppendLine($"<text x='10' y='{y + 14}' fill='#7f8c8d' font-size='10'>R{rowHeader.Index}</text>");
            var current = rowHeader.Access;
            int x = 40;
            while (current != null)
            {
                string color = current.SatelliteId.StartsWith("SAT-POL-") ? "#3498db" : "#2ecc71";
                sb.AppendLine($"<rect x='{x}' y='{y}' width='18' height='18' fill='{color}' rx='3' stroke='#fff' stroke-width='1'/>");
                sb.AppendLine($"<title>{current.SatelliteId} ({current.Row},{current.Column})</title>");
                x += 22;
                current = current.Right;
            }
            y += 24;
            rowHeader = rowHeader.Next;
        }

        sb.AppendLine("<text x='20' y='470' fill='#95a5a6' font-size='11'>Verde: Ecuatorial | Azul: Polar</text>");
        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    private string GenerateMemoryLayoutSvg()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 600 300' font-family='Consolas,monospace' font-size='12'>");
        sb.AppendLine("<rect width='600' height='300' fill='#f8f9fa' rx='8'/>");
        sb.AppendLine("<text x='20' y='30' font-size='16' font-weight='bold' fill='#2c3e50'>Mapa de Memoria - OrbitNetStore</text>");

        int y = 60;
        void WriteBox(int x, int w, string title, string content, string color)
        {
            sb.AppendLine($"<rect x='{x}' y='{y}' width='{w}' height='50' fill='{color}' rx='6' stroke='#bdc3c7' stroke-width='1'/>");
            sb.AppendLine($"<text x='{x + 10}' y='{y + 20}' font-weight='bold' fill='#2c3e50'>{title}</text>");
            sb.AppendLine($"<text x='{x + 10}' y='{y + 38}' fill='#34495e'>{content}</text>");
        }

        WriteBox(20, 160, "Satélites", $"{_store.ActiveSatellites} activos", "#d5f5e3");
        WriteBox(200, 160, "Antenas", $"{_store.TotalAntennas} terrestres", "#d6eaf8");
        WriteBox(380, 160, "Simulación", $"Tick {_store.CurrentTick}", "#fdebd0");
        y += 70;

        WriteBox(20, 160, "Cola Mensajes", $"{_store.PendingMessages} pendientes", "#e8daef");
        WriteBox(200, 160, "Eventos", $"{_store.EventsProcessed} procesados", "#fadbd8");
        WriteBox(380, 160, "Ocupación", $"{_store.QueueOccupancyPercentage:F1}%", "#d5f5e3");

        sb.AppendLine($"<text x='20' y='270' fill='#95a5a6'>Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</text>");
        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    private string GenerateRoutingSvg()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 800 400' font-family='Consolas,monospace' font-size='12'>");
        sb.AppendLine("<rect width='800' height='400' fill='#f8f9fa' rx='8'/>");
        sb.AppendLine("<text x='20' y='30' font-size='16' font-weight='bold' fill='#2c3e50'>Ruta de Relés</text>");

        int y = 60;
        var rowHeader = _store.RedSatellites.GetFirstRowHeader();
        int index = 1;
        while (rowHeader != null)
        {
            var current = rowHeader.Access;
            while (current != null)
            {
                int x = 40 + (index - 1) % 4 * 180;
                int row = (index - 1) / 4;
                int cy = y + row * 60;

                sb.AppendLine($"<rect x='{x}' y='{cy}' width='150' height='40' fill='#eaf2f8' rx='6' stroke='#3498db' stroke-width='1'/>");
                sb.AppendLine($"<text x='{x + 10}' y='{cy + 18}' font-weight='bold' fill='#2c3e50'>{current.SatelliteId}</text>");
                sb.AppendLine($"<text x='{x + 10}' y='{cy + 32}' fill='#7f8c8d' font-size='10'>{current.IpAddress}</text>");

                if (index > 1 && index <= _store.RedSatellites.Count)
                {
                    int prevX = 40 + (index - 2) % 4 * 180;
                    int prevRow = (index - 2) / 4;
                    int prevCy = y + prevRow * 60;
                    sb.AppendLine($"<line x1='{prevX + 150}' y1='{prevCy + 20}' x2='{x}' y2='{cy + 20}' stroke='#2ecc71' stroke-width='2' marker-end='url(#arrow)'/>");
                }

                index++;
                current = current.Right;
            }
            rowHeader = rowHeader.Next;
        }

        sb.AppendLine("<defs><marker id='arrow' viewBox='0 0 10 10' refX='8' refY='5' markerWidth='6' markerHeight='6' orient='auto'><path d='M0,0 L10,5 L0,10' fill='#2ecc71'/></marker></defs>");
        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    private string GenerateBuffersSvg()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 800 400' font-family='Consolas,monospace' font-size='12'>");
        sb.AppendLine("<rect width='800' height='400' fill='#f8f9fa' rx='8'/>");
        sb.AppendLine("<text x='20' y='30' font-size='16' font-weight='bold' fill='#2c3e50'>Capacidad de Buffers</text>");

        int x = 40;
        int satCount = _store.Satellites.Count;
        int barWidth = Math.Max(30, 600 / Math.Max(1, satCount));
        _store.Satellites.ForEach(s =>
        {
            int occupied = s.PaquetesABordo?.Count ?? 0;
            int capacity = 100;
            int percent = Math.Min(100, capacity > 0 ? (int)((double)occupied / capacity * 100) : 0);
            int barHeight = Math.Max(4, percent * 2);
            string color = percent >= 80 ? "#e74c3c" : percent >= 50 ? "#f39c12" : "#2ecc71";
            int cx = x;
            sb.AppendLine($"<rect x='{cx}' y='{320 - barHeight}' width='{barWidth - 4}' height='{barHeight}' fill='{color}' rx='3'/>");
            sb.AppendLine($"<text x='{cx}' y='340' fill='#2c3e50' font-size='9'>{s.Id}</text>");
            sb.AppendLine($"<text x='{cx}' y='352' fill='#7f8c8d' font-size='8'>{percent}%</text>");
            x += barWidth;
        });

        sb.AppendLine("<line x1='20' y1='320' x2='780' y2='320' stroke='#bdc3c7' stroke-width='1'/>");
        sb.AppendLine("<text x='20' y='380' fill='#95a5a6' font-size='11'>Rojo: crítico (>80%) | Amarillo: normal (50-80%) | Verde: bajo (<50%)</text>");
        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    public string GetDotContent(string reportName)
    {
        switch (reportName)
        {
            case "MemoryLayout":
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
            case "Routing":
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
            case "Buffers":
            {
                var buffers = new List<BufferReportItem>();
                var totalPercent = 0;
                _store.Satellites.ForEach(s =>
                {
                    int occupied = s.PaquetesABordo?.Count ?? 0;
                    int capacity = 100;
                    int percent = capacity > 0 ? (int)((double)occupied / capacity * 100) : 0;
                    totalPercent += percent;
                    buffers.Add(new BufferReportItem
                    {
                        Id = $"BUF-{s.Id}",
                        Satellite = s.Id,
                        Capacity = capacity,
                        Occupied = occupied,
                        OccupancyPercent = percent,
                        Status = percent >= 80 ? "critico" : percent >= 50 ? "normal" : "bajo"
                    });
                });

                var bufData = new BufferReportData
                {
                    Buffers = buffers,
                    TotalBuffers = buffers.Count,
                    AverageOccupancy = buffers.Count > 0 ? totalPercent / buffers.Count : 0
                };
                return BufferGraphGenerator.BuildDot(bufData);
            }
            case "Matrix":
            {
                var cells = new List<SparseMatrixCell>();
                var rh = _store.RedSatellites.GetFirstRowHeader();
                while (rh != null)
                {
                    var cur = rh.Access;
                    while (cur != null)
                    {
                        cells.Add(new SparseMatrixCell
                        {
                            Row = cur.Row, Column = cur.Column,
                            SatelliteId = cur.SatelliteId,
                            Capacity = 100, Occupied = 0,
                            OccupancyPercent = 0, Status = "active", Messages = ""
                        });
                        cur = cur.Right;
                    }
                    rh = rh.Next;
                }
                var matData = new SparseMatrixReportData
                {
                    Rows = cells.Any() ? cells.Max(c => c.Row) : 0,
                    Columns = cells.Any() ? cells.Max(c => c.Column) : 0,
                    Cells = cells
                };
                return MatrixGraphGenerator.BuildDot(matData);
            }
            default:
                return "digraph { }";
        }
    }
}
