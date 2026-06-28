namespace OrbitNet.Web.Services.Graphviz;

using System.Text;
using OrbitNet.Web.Models.ViewModels;

public class AvlGraphGenerator
{
    public static string BuildDot(AvlReportData data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("digraph AVL {");
        builder.AppendLine("  rankdir=TB;");
        builder.AppendLine("  node [shape=record, fontname=\"Consolas\", fontsize=10];");
        builder.AppendLine("  edge [tailclip=false, arrowsize=0.8];");
        builder.AppendLine();

        foreach (var node in data.Nodes)
        {
            var label = $"{{<prev> prev | <data> {node.Value} | <next> next }}";
            builder.AppendLine($"  node_{node.Id} [label=\"{label}\"];" );
        }

        foreach (var node in data.Nodes)
        {
            if (node.Children.Length > 0)
            {
                if (node.Children.Length > 0)
                    builder.AppendLine($"  node_{node.Id}:data:c -> node_{node.Children[0]}:data;");
                if (node.Children.Length > 1)
                    builder.AppendLine($"  node_{node.Id}:data:c -> node_{node.Children[1]}:data;");
            }
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}
