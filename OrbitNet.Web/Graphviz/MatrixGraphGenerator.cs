namespace OrbitNet.Web.Services.Graphviz;

using System.Text;
using OrbitNet.Web.Models.ViewModels;

public class MatrixGraphGenerator
{
    public static string BuildDot(SparseMatrixReportData data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("digraph Matrix {");
        builder.AppendLine("  rankdir=TB;");
        builder.AppendLine("  node [shape=plaintext, fontname=\"Consolas\", fontsize=10];");
        builder.AppendLine("  graph [pad=0.5];");
        builder.AppendLine();

        builder.AppendLine("  matrix [label=<");
        builder.AppendLine("    <table border=\"0\" cellborder=\"1\" cellspacing=\"0\">\n");
        builder.AppendLine("      <tr><td colspan=\"4\"><b>Matriz Dispersa de Capacidad</b></td></tr>");
        builder.AppendLine($"      <tr><td colspan=\"2\"><b>Filas</b></td><td colspan=\"2\">{data.Rows}</td></tr>");
        builder.AppendLine($"      <tr><td colspan=\"2\"><b>Columnas</b></td><td colspan=\"2\">{data.Columns}</td></tr>");
        builder.AppendLine("      <tr><td><b>Fila</b></td><td><b>Col</b></td><td><b>Satélite / Ocupación</b></td><td><b>Mensajes</b></td></tr>");

        foreach (var cell in data.Cells)
        {
            var bar = new string('▓', cell.OccupancyPercent / 10).PadRight(10, '░');
            builder.AppendLine($"      <tr><td>{cell.Row}</td><td>{cell.Column}</td><td>{cell.SatelliteId} {cell.Occupied}/{cell.Capacity} {bar}</td><td>{cell.Messages}</td></tr>");
        }

        builder.AppendLine("    </table>");
        builder.AppendLine("  >];");
        builder.AppendLine("}");
        return builder.ToString();
    }
}
