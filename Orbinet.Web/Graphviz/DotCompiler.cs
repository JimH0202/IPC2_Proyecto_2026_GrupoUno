using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace OrbitNet.Web.Services.Graphviz;

public static class DotCompiler
{
	// Compiles a DOT graph to SVG by invoking the 'dot' executable (Graphviz) via stdin/stdout.
	// Returns a sanitized SVG string. If 'dot' is not available or an error occurs, returns null.
	public static string? CompileDotToSvg(string dotSource, int timeoutMs = 5000)
	{
		if (string.IsNullOrWhiteSpace(dotSource)) return null;

		try
		{
			var psi = new ProcessStartInfo("dot")
			{
				Arguments = "-Tsvg",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};

			using var proc = Process.Start(psi);
			if (proc == null) return null;

			// write dot to stdin
			proc.StandardInput.Write(dotSource);
			proc.StandardInput.Close();

			var outputTask = proc.StandardOutput.ReadToEndAsync();
			var errorTask = proc.StandardError.ReadToEndAsync();

			if (!proc.WaitForExit(timeoutMs))
			{
				try { proc.Kill(); } catch { }
				return null;
			}

			var svg = outputTask.Result;
			var err = errorTask.Result;

			if (string.IsNullOrWhiteSpace(svg)) return null;

			// Basic sanitization to reduce risk of XSS when embedding SVG inline.
			svg = SanitizeSvg(svg);

			return svg;
		}
		catch
		{
			return null;
		}
	}

	public static string SanitizeSvg(string svg)
	{
		if (string.IsNullOrEmpty(svg)) return svg;

		// Remove any <script> tags
		svg = Regex.Replace(svg, @"<script[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);

		// Remove event handler attributes like onload, onclick, onmouseover, etc.
		svg = Regex.Replace(svg, @"\son\w+\s*=\s*""[^""]*""", string.Empty, RegexOptions.IgnoreCase);
		svg = Regex.Replace(svg, @"\son\w+\s*=\s*'[^']*'", string.Empty, RegexOptions.IgnoreCase);

		// Remove foreignObject elements which could contain arbitrary HTML
		svg = Regex.Replace(svg, @"<foreignObject[\s\S]*?</foreignObject>", string.Empty, RegexOptions.IgnoreCase);

		// Strip javascript: URIs
		svg = Regex.Replace(svg, @"javascript:\w+\([^)]+\)", string.Empty, RegexOptions.IgnoreCase);

		return svg;
	}
}