using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using Microsoft.Playwright;

static int ParseLeadingNumberOrMax(string filename)
{
    var m = Regex.Match(filename, @"^\s*(\d+)\.");
    return m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
        ? n
        : int.MaxValue;
}

static string CleanupSourceMarkdown(string raw)
{
    // Keep original structure; only fix clear typos and remove per-file footer noise.
    var s = raw.Replace("\r\n", "\n");

    s = s.Replace("Awaut", "Await", StringComparison.OrdinalIgnoreCase);
    s = s.Replace("Cancelation", "Cancellation", StringComparison.OrdinalIgnoreCase);
    s = s.Replace("ConcurrentQueu", "ConcurrentQueue", StringComparison.OrdinalIgnoreCase);

    // Remove file-level trailing boilerplate line(s) that don't belong in a book.
    s = Regex.Replace(s, @"\n\*Same style as the other kid notes.*\n?", "\n", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\n\*Next time you get a new transcript.*\n?", "\n", RegexOptions.IgnoreCase);

    // Trim excessive blank lines.
    s = Regex.Replace(s, @"\n{3,}", "\n\n");
    return s.Trim() + "\n";
}

static (string title, string body) SplitTitleFromBody(string cleaned)
{
    // Expect first line to be an H1 like "# Notes: ...."
    var lines = cleaned.Replace("\r\n", "\n").Split('\n');
    if (lines.Length == 0) return ("", cleaned);

    var first = lines[0].Trim();
    if (!first.StartsWith("#"))
        return ("", cleaned);

    var title = first.TrimStart('#').Trim();

    // Drop title line + optional blank line right after.
    var startIdx = 1;
    while (startIdx < lines.Length && string.IsNullOrWhiteSpace(lines[startIdx])) startIdx++;

    var body = string.Join("\n", lines.Skip(startIdx));
    body = CleanupSourceMarkdown(body);
    return (title, body);
}

static string DemoteHeadingsForEmbedding(string markdown, int demoteBy)
{
    // We only want book-level headings to control layout.
    // So we demote headings inside each lesson body (e.g. "##" -> "####") while preserving code fences.
    var s = markdown.Replace("\r\n", "\n");
    var lines = s.Split('\n');
    var inFence = false;

    for (var i = 0; i < lines.Length; i++)
    {
        var line = lines[i];

        if (line.StartsWith("```"))
        {
            inFence = !inFence;
            continue;
        }

        if (inFence) continue;

        var m = Regex.Match(line, @"^(#{1,6})\s+");
        if (!m.Success) continue;

        var hashes = m.Groups[1].Value;
        var newLevel = Math.Min(6, hashes.Length + demoteBy);
        if (newLevel != hashes.Length)
            lines[i] = new string('#', newLevel) + line[hashes.Length..];
    }

    return string.Join("\n", lines);
}

static Dictionary<int, string> LoadSectionTitlesFromRoadmap(string repoRoot)
{
    var map = new Dictionary<int, string>();
    var path = Path.Combine(repoRoot, "ROADMAP.md");
    if (!File.Exists(path)) return map;

    var text = File.ReadAllText(path, Encoding.UTF8).Replace("\r\n", "\n");
    var lines = text.Split('\n');
    var inSectionBySectionTable = false;

    for (var i = 0; i < lines.Length; i++)
    {
        var line = lines[i];

        // Prefer the canonical section-by-section plan table:
        // | Section | Topic | Lectures | ...
        if (!inSectionBySectionTable)
        {
            if (Regex.IsMatch(line, @"^\|\s*Section\s*\|\s*Topic\s*\|", RegexOptions.IgnoreCase))
                inSectionBySectionTable = true;
            continue;
        }

        // Stop when the table ends (blank line or separator section).
        if (string.IsNullOrWhiteSpace(line)) break;
        if (!line.TrimStart().StartsWith("|", StringComparison.Ordinal)) break;

        // Match rows like:
        // | **1** | Introduction | ...
        var m = Regex.Match(line, @"^\|\s*\*\*(\d+)\*\*\s*\|\s*([^|]+?)\s*\|");
        if (!m.Success) continue;

        if (!int.TryParse(m.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var sec)) continue;
        var title = m.Groups[2].Value.Trim();
        if (sec is >= 1 and <= 9 && !string.IsNullOrWhiteSpace(title) && title != "—")
            map[sec] = title;
    }

    return map;
}

static string BuildHtmlDocument(string bodyHtml, string css)
{
    return $"""
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Threading, Async, and Parallel Programming (Kid Notes)</title>
  <style>
{css}
  </style>
</head>
<body>
  <main class="book">
{bodyHtml}
  </main>
</body>
</html>
""";
}

var repoRoot = Directory.GetCurrentDirectory();
var outDir = Path.Combine(repoRoot, "book");
Directory.CreateDirectory(outDir);

var bookMdPath = Path.Combine(outDir, "notes-kid-sec1-8.book.md");
var bookHtmlPath = Path.Combine(outDir, "notes-kid-sec1-8.book.html");
var bookPdfPath = Path.Combine(outDir, "notes-kid-sec1-8.book.pdf");

var cssPath = Path.Combine(AppContext.BaseDirectory, "book.css");
if (!File.Exists(cssPath))
{
    // When running via dotnet run, base dir is bin/...; fall back to project dir.
    var alt = Path.Combine(repoRoot, "tools", "notes-kid-book", "book.css");
    cssPath = alt;
}

var css = File.Exists(cssPath) ? File.ReadAllText(cssPath, Encoding.UTF8) : "";
var sectionTitles = LoadSectionTitlesFromRoadmap(repoRoot);

var md = new StringBuilder();
// Cover page (forces TOC to start on page 2 in the PDF).
md.AppendLine("<div class=\"cover\">");
md.AppendLine("  <div class=\"cover-title\">Threading, Async, and Parallel Programming</div>");
md.AppendLine("  <div class=\"cover-subtitle\">Kid Notes (Sec 1–Sec 8)</div>");
md.AppendLine("  <div class=\"cover-meta\">Generated from your `notes-kid` files, with light cleanup for consistency.</div>");
md.AppendLine("</div>");
md.AppendLine();

md.AppendLine("<a id=\"toc\"></a>");
md.AppendLine();
md.AppendLine("## Table of contents");
md.AppendLine();
md.AppendLine("> Tip: Use this TOC (clickable), and also your PDF viewer’s left sidebar (outline/bookmarks) if available.");
md.AppendLine();
md.AppendLine("<div class=\"toc\">");
md.AppendLine();
md.AppendLine("<table class=\"toc-table\">");
md.AppendLine("<thead><tr><th>Section</th><th>Chapter</th></tr></thead>");

for (var sec = 1; sec <= 8; sec++)
{
    var secDir = Path.Combine(repoRoot, $"Sec {sec}", "notes-kid");
    if (!Directory.Exists(secDir)) continue;

    var files = Directory.GetFiles(secDir, "*.md", SearchOption.TopDirectoryOnly)
        .OrderBy(f => ParseLeadingNumberOrMax(Path.GetFileName(f)))
        .ThenBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
        .ToList();

    if (files.Count == 0) continue;

    var secTitle = sectionTitles.TryGetValue(sec, out var t) ? t : $"Section {sec}";
    var secLabel = $"Section {sec} — {secTitle}";
    var secLink = $"<a href=\"#section-{sec}\">{secLabel}</a>";

    // Prevent "orphaned" section headers at the bottom of a page by grouping
    // the section row with the next 1–2 chapter rows in a non-breakable tbody.
    md.AppendLine("<tbody class=\"toc-group\">");
    md.AppendLine($"<tr class=\"toc-section\"><td colspan=\"2\">{secLink}</td></tr>");

    var firstBatch = files.Take(2).ToList();
    foreach (var f in firstBatch)
    {
        var lessonName = Path.GetFileNameWithoutExtension(f);
        var anchor = Regex.Replace(lessonName.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        md.AppendLine($"<tr><td class=\"toc-sec\">{sec}</td><td class=\"toc-ch\"><a href=\"#sec-{sec}-{anchor}\">{lessonName}</a></td></tr>");
    }
    md.AppendLine("</tbody>");

    // Remaining chapter rows can break across pages normally.
    var rest = files.Skip(2).ToList();
    if (rest.Count > 0)
    {
        md.AppendLine("<tbody class=\"toc-rest\">");
        foreach (var f in rest)
        {
            var lessonName = Path.GetFileNameWithoutExtension(f);
            var anchor = Regex.Replace(lessonName.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
            md.AppendLine($"<tr><td class=\"toc-sec\">{sec}</td><td class=\"toc-ch\"><a href=\"#sec-{sec}-{anchor}\">{lessonName}</a></td></tr>");
        }
        md.AppendLine("</tbody>");
    }
}

md.AppendLine();
md.AppendLine("</table>");
md.AppendLine("</div>");
md.AppendLine();

for (var sec = 1; sec <= 8; sec++)
{
    var secDir = Path.Combine(repoRoot, $"Sec {sec}", "notes-kid");
    if (!Directory.Exists(secDir)) continue;

    var files = Directory.GetFiles(secDir, "*.md", SearchOption.TopDirectoryOnly)
        .OrderBy(f => ParseLeadingNumberOrMax(Path.GetFileName(f)))
        .ThenBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
        .ToList();

    if (files.Count == 0) continue;

    var secTitle = sectionTitles.TryGetValue(sec, out var roadmapTitle) ? roadmapTitle : null;
    md.AppendLine(secTitle is { Length: > 0 } ? $"## Section {sec} — {secTitle}" : $"## Section {sec}");
    md.AppendLine();

    foreach (var f in files)
    {
        var raw = File.ReadAllText(f, Encoding.UTF8);
        var cleaned = CleanupSourceMarkdown(raw);
        var (title, body) = SplitTitleFromBody(cleaned);

        var lessonName = Path.GetFileNameWithoutExtension(f);
        var anchor = Regex.Replace(lessonName.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        // Add a class to the chapter heading (Markdown-native) so we can page-break chapters via CSS.
        // IMPORTANT: Do not wrap chapters in raw HTML blocks, or Markdown headings may stop rendering.
        md.AppendLine($"### {lessonName} {{.chapter-title}}");
        md.AppendLine();
        md.AppendLine($"<a id=\"sec-{sec}-{anchor}\"></a>");
        md.AppendLine();

        // Preserve the original title line content as a small lead if useful.
        if (!string.IsNullOrWhiteSpace(title))
        {
            var t = title;
            if (t.StartsWith("Notes:", StringComparison.OrdinalIgnoreCase))
                t = t["Notes:".Length..].Trim();
            md.AppendLine($"> **Lesson focus:** {t}");
            md.AppendLine();
        }

        var embeddedBody = DemoteHeadingsForEmbedding(body.Trim(), demoteBy: 2);
        md.AppendLine(embeddedBody);
        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }
}

File.WriteAllText(bookMdPath, md.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var bodyHtml = Markdig.Markdown.ToHtml(md.ToString(), pipeline);
var fullHtml = BuildHtmlDocument(bodyHtml, css);
File.WriteAllText(bookHtmlPath, fullHtml, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

// Render PDF via Playwright (Chromium).
// This will download browser binaries on first run (playwright install).
Console.WriteLine("Ensuring Playwright browsers are installed...");
var installExitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
if (installExitCode != 0)
{
    throw new Exception($"Playwright browser install failed with exit code {installExitCode}.");
}

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
var page = await browser.NewPageAsync();

await page.GotoAsync(new Uri(bookHtmlPath).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

Console.WriteLine("Generating PDF...");
await page.PdfAsync(new PagePdfOptions
{
    Path = bookPdfPath,
    Format = "A4",
    PrintBackground = true,
    Margin = new Margin
    {
        Top = "18mm",
        Bottom = "18mm",
        Left = "16mm",
        Right = "16mm"
    }
});

Console.WriteLine($"Wrote:\n- {bookMdPath}\n- {bookHtmlPath}\n- {bookPdfPath}");
