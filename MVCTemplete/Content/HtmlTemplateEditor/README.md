# HtmlEditor

A small, dependency-free, reusable rich text / HTML source / preview editor,
refactored out of the standalone Inkline editor into a `HtmlEditor.create()`
library that supports multiple instances on one page.

Includes 9 built-in templates (Blank, Article, Blog post, Newsletter,
Invoice/Receipt, Business letter, Resume/CV, Landing page, FAQ page), custom
in-editor dialogs and toast notifications (no native `alert`/`prompt`
popups), and paste sanitizing so content pasted from Word or web pages can't
inject scripts or broken markup.

Not yet included from the original big spec: Monaco source editor,
drag-and-drop email builder, plugin system, merge-field variables, ZIP
export. Those can be layered on top of this base incrementally.

## Install

Copy the `HtmlTemplateEditor` folder into your project (anywhere — `/lib`,
`/wwwroot/lib`, `/scripts`, doesn't matter, just keep the folder as a whole).
There are two ways to include it — pick one.

### Option A — single file (`html-editor.bundle.js`)

CSS is embedded in the JS and injected automatically. Only **one** `<script>`
tag needed, no `<link>` at all:

```html
<script src="/HtmlTemplateEditor/html-editor.bundle.js"></script>

<div id="editor"></div>

<script>
  const editor = HtmlEditor.create({
    element: "#editor",
    theme: "light",   // or "dark"
    height: "560px"
  });
</script>
```

This is the one to use if you want the same two lines on every page with the
least chance of forgetting a file.

### Option B — separate CSS + JS (`css/editor.css` + `js/html-editor.js`)

Same result, just split into two files if you'd rather cache/version them
separately or run the CSS through your own bundler:

```html
<link rel="stylesheet" href="/HtmlTemplateEditor/css/editor.css">
<script src="/HtmlTemplateEditor/js/html-editor.js"></script>

<div id="editor"></div>

<script>
  const editor = HtmlEditor.create({ element: "#editor" });
</script>
```

Both options expose the exact same `HtmlEditor` global and API — pick
whichever, don't include both on the same page.

No build step, no other dependencies, everything runs fully offline.

## Using it on every page of a site

1. Put the `HtmlTemplateEditor` folder once, at the root of your static
   assets (e.g. for an ASP.NET app: `/wwwroot/HtmlTemplateEditor/`).
2. On any page/view that needs the editor, add the one `<script>` tag from
   Option A (or the `<link>` + `<script>` pair from Option B) to that page's
   `<head>` or before `</body>` — same path every time, e.g.:
   ```html
   <script src="/HtmlTemplateEditor/html-editor.bundle.js"></script>
   ```
   If you have a shared layout (`_Layout.cshtml`, a master page, a common
   header include, etc.), put the tag there once instead of repeating it —
   then every page that uses that layout has `HtmlEditor` available for free.
3. On each page, call `HtmlEditor.create({ element: "#yourDivId" })` wherever
   you actually want an editor to appear. You can have zero, one, or several
   editors on a given page — the include is the same regardless.
4. To get the content back out (e.g. to save to your database on form
   submit), call `editor.getHTML()` and put it in a hidden field:
   ```html
   <input type="hidden" id="ContentHtml" name="ContentHtml">
   <script>
     document.querySelector('form').addEventListener('submit', () => {
       document.getElementById('ContentHtml').value = editor.getHTML();
     });
   </script>
   ```

## Options

| Option | Type | Default | Description |
|---|---|---|---|
| `element` | string \| HTMLElement | required | CSS selector or element to mount into |
| `theme` | `"light"` \| `"dark"` | `"light"` | Initial color theme |
| `height` | CSS length | `"480px"` | Total height of the editor shell |
| `placeholder` | string | `"Start writing..."` | Empty-state placeholder text |
| `initialHTML` | string | Article template | HTML to load on init |
| `templates` | array | `[]` | Extra `{ name, desc, html }` entries appended to the built-in template list |

## API

```js
editor.getHTML()              // -> current HTML string
editor.setHTML(html)          // replace content
editor.insertHTML(html)       // insert at cursor
editor.setTheme("dark")       // switch theme live
editor.format()               // pretty-print the HTML source pane
editor.preview()              // switch to the Preview tab
editor.exportHTML()           // download as document.html
editor.on("change", fn)       // fn(html) fires on edits / tab switches
editor.on("ready", fn)        // fn(instance) fires once, after init
editor.destroy()              // tear down and clear the host element

HtmlEditor.create(options)    // create + mount an instance, returns it
HtmlEditor.destroy(target)    // destroy the instance mounted on target
```

## Multiple instances

Each `HtmlEditor.create()` call is fully independent — mount as many as you
like on one page, each with its own theme, templates, and state.

```js
const a = HtmlEditor.create({ element: "#editorA" });
const b = HtmlEditor.create({ element: "#editorB", theme: "dark" });
```

## Files

```
HtmlTemplateEditor/
  css/editor.css        theme + layout, scoped under .hte-root
  js/html-editor.js      the library
  index.html             usage demo
  README.md
```

## Next layers (not built yet)

Roughly in order of value if you want to keep extending this:

1. Merge-field variables (`{{FirstName}}` etc.) — small addition to the toolbar + insertHTML.
2. Import (paste/drag a `.html` file in).
3. A real code-editing experience in the HTML tab (syntax highlighting, line numbers) — CodeMirror is a much lighter offline-embeddable option than Monaco for this.
4. Email-safe export (inline CSS instead of a `<style>` block) for the "email builder" use case.
5. A plugin hook (`HtmlEditor.use(plugin)`) once there's a second or third feature that benefits from being pluggable rather than baked in.
