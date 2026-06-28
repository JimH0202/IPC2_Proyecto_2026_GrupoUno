namespace OrbitNet.Web.Services.Graphviz;

using System;
using System.Text;
using OrbitNet.Web.Models.ViewModels;

public class RouteGraphGenerator
{
    public static string BuildDot(RouteReportData data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("digraph Route {");
        builder.AppendLine("  rankdir=LR;");
        builder.AppendLine("  node [shape=record, fontname=\"Consolas\", fontsize=10, style=filled];");
        builder.AppendLine("  edge [arrowsize=0.8, penwidth=3.0, color=\"#27AE60\"];");
        builder.AppendLine();

        foreach (var route in data.Routes)
        {
            var isInactive = route.Status.Contains("inactive", StringComparison.OrdinalIgnoreCase) || route.Status.Contains("inactiva", StringComparison.OrdinalIgnoreCase);
            var fill = isInactive ? "#E74C3C" : "#2ECC71";
            var style = isInactive ? "dashed,filled" : "filled";
            var label = $"{{{route.Source} | {route.Destination} | hops={route.Hops} | {route.Status}}}";
            builder.AppendLine($"  route_{route.Id} [label=\"{label}\", fillcolor=\"{fill}\", style=\"{style}\"];" );
        }

        for (var i = 0; i < data.Routes.Count - 1; i++)
        {
            var current = data.Routes[i];
            var next = data.Routes[i + 1];
            builder.AppendLine($"  route_{current.Id} -> route_{next.Id};");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}
