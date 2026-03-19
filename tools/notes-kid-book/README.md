# Notes Kid Book Builder

Generates a single “book” PDF from `Sec 1` to `Sec 8` `notes-kid` markdown files **without modifying the originals**.

## Output

Writes these files to `book/`:
- `notes-kid-sec1-8.book.md`
- `notes-kid-sec1-8.book.html`
- `notes-kid-sec1-8.book.pdf`

## Run

From the repo root:

```powershell
dotnet run --project "tools/notes-kid-book/NotesKidBook.csproj"
```

First run will download Playwright Chromium browser binaries.

