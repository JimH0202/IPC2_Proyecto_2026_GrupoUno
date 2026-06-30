namespace OrbitNet.Web.Services.Graphviz;

using System.Text;
using OrbitNet.Web.Models.ViewModels;

public class BufferGraphGenerator
{
    public static string BuildDot(BufferReportData data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("digraph Buffers {");
        builder.AppendLine("  rankdir=LR;");
        builder.AppendLine("  node [shape=record, fontname=\"Consolas\", fontsize=10, style=filled, fillcolor=\"#ecf0f1\"];\n");

        foreach (var buffer in data.Buffers)
        {
            var fill = buffer.OccupancyPercent >= 80 ? "#e74c3c" : buffer.OccupancyPercent >= 50 ? "#f1c40f" : "#2ecc71";
            var label = $"{{<id> {buffer.Id} | <sat> {buffer.Satellite} | <cap> {buffer.Occupied}/{buffer.Capacity} ({buffer.OccupancyPercent}%) | <status> {buffer.Status}}}";
            builder.AppendLine($"  buffer_{buffer.Id.Replace('-', '_')} [label=\"{label}\", fillcolor=\"{fill}\"];");
        }

        for (var i = 0; i < data.Buffers.Count - 1; i++)
        {
            var current = data.Buffers[i];
            var next = data.Buffers[i + 1];
            builder.AppendLine($"  buffer_{current.Id.Replace('-', '_')}:id:c -> buffer_{next.Id.Replace('-', '_')}:id:c [style=dashed,color=\"#7f8c8d\"];");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}
