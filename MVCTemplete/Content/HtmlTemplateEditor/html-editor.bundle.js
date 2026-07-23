/*!
 * HtmlEditor — single-file bundle (CSS + JS in one script tag).
 * Just include this one file. Styles are injected automatically on load.
 */
(function() {
  'use strict';
  var css = `/* ==========================================================================
   HtmlEditor — reusable editor styles
   Scoped under .hte-root so multiple instances / host page styles never clash.
   Theme is switched via [data-hte-theme="light|dark"] on the root element.
   ========================================================================== */

.hte-root {
  position: relative;
  --hte-paper: #F6F4EE;
  --hte-surface: #FFFFFF;
  --hte-ink: #20242A;
  --hte-ink-soft: #6B7178;
  --hte-accent: #146C6A;
  --hte-accent-dark: #0E5250;
  --hte-accent-soft: #E4F1EF;
  --hte-border: #E1DCCF;
  --hte-code-bg: #1e1e1e;
  --hte-code-fg: #d4d4d4;
  --hte-radius: 8px;

  box-sizing: border-box;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  color: var(--hte-ink);
  background: var(--hte-paper);
  border: 1px solid var(--hte-border);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 1px 2px rgba(32,36,42,0.04), 0 8px 24px rgba(32,36,42,0.05);
}
.hte-root, .hte-root *, .hte-root *::before, .hte-root *::after { box-sizing: border-box; }

.hte-root[data-hte-theme="dark"] {
  --hte-paper: #1B1D21;
  --hte-surface: #23262B;
  --hte-ink: #E8E9EC;
  --hte-ink-soft: #9A9FA8;
  --hte-accent: #35C9C1;
  --hte-accent-dark: #6FE0D9;
  --hte-accent-soft: #1F3836;
  --hte-border: #33363C;
  --hte-code-bg: #16171A;
  --hte-code-fg: #d4d4d4;
}

/* ---------- Toolbar ---------- */
.hte-toolbar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px;
  padding: 8px 10px;
  border-bottom: 1px solid var(--hte-border);
  background: var(--hte-surface);
}
.hte-tgroup {
  display: flex;
  align-items: center;
  gap: 2px;
  padding: 0 8px;
  border-right: 1px solid var(--hte-border);
}
.hte-tgroup:last-of-type { border-right: none; }
.hte-btn {
  width: 32px;
  height: 32px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 1px solid transparent;
  background: transparent;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  color: var(--hte-ink);
  line-height: 1;
  transition: background .12s ease, color .12s ease;
}
.hte-btn svg { width: 16px; height: 16px; stroke: var(--hte-ink); fill: none; stroke-width: 2; stroke-linecap: round; stroke-linejoin: round; }
.hte-btn:hover { background: var(--hte-accent-soft); color: var(--hte-accent-dark); }
.hte-btn:hover svg { stroke: var(--hte-accent-dark); }
.hte-btn.hte-italic { font-style: italic; }
.hte-btn.hte-underline { text-decoration: underline; }
.hte-btn.hte-strike { text-decoration: line-through; }
.hte-btn.hte-active { background: var(--hte-accent-soft); color: var(--hte-accent-dark); }

.hte-sel {
  height: 32px;
  border: 1px solid var(--hte-border);
  border-radius: 6px;
  background: var(--hte-surface);
  color: var(--hte-ink);
  font-size: 12.5px;
  padding: 0 6px;
  cursor: pointer;
}

.hte-spacer { flex: 1; }

.hte-wrap { position: relative; }
.hte-btn-wide {
  height: 32px;
  padding: 0 12px;
  border: 1px solid var(--hte-border);
  border-radius: 6px;
  background: var(--hte-surface);
  font-size: 12.5px;
  font-weight: 600;
  color: var(--hte-ink);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
.hte-btn-wide:hover { border-color: var(--hte-accent); color: var(--hte-accent-dark); }
.hte-btn-primary { background: var(--hte-accent); color: #fff; border-color: var(--hte-accent); }
.hte-btn-primary:hover { background: var(--hte-accent-dark); color: #fff; }

.hte-panel {
  position: absolute;
  top: 38px;
  right: 0;
  width: 260px;
  background: var(--hte-surface);
  border: 1px solid var(--hte-border);
  border-radius: 10px;
  box-shadow: 0 12px 28px rgba(32,36,42,0.14);
  padding: 6px;
  z-index: 40;
  display: none;
}
.hte-panel.hte-open { display: block; }
.hte-row { display: flex; flex-direction: column; padding: 8px 10px; border-radius: 6px; cursor: pointer; }
.hte-row:hover { background: var(--hte-accent-soft); }
.hte-row .hte-name { font-size: 13px; font-weight: 600; }
.hte-row .hte-desc { font-size: 11.5px; color: var(--hte-ink-soft); }
.hte-divider { height: 1px; background: var(--hte-border); margin: 4px 2px; }
.hte-save-row {
  display: flex; align-items: center; gap: 6px;
  padding: 8px 10px; font-size: 12.5px; font-weight: 600; color: var(--hte-accent-dark); cursor: pointer; border-radius: 6px;
}
.hte-save-row:hover { background: var(--hte-accent-soft); }

/* ---------- View tabs ---------- */
.hte-viewtabs { display: flex; gap: 2px; padding: 8px 10px 0; background: var(--hte-surface); }
.hte-vtab {
  padding: 7px 16px;
  font-size: 12.5px;
  font-weight: 600;
  color: var(--hte-ink-soft);
  background: transparent;
  border: 1px solid transparent;
  border-bottom: none;
  border-radius: 8px 8px 0 0;
  cursor: pointer;
}
.hte-vtab.hte-active { color: var(--hte-ink); background: var(--hte-paper); border-color: var(--hte-border); }

/* ---------- Panes ---------- */
.hte-panes { position: relative; background: var(--hte-paper); flex: 1; min-height: 0; overflow: hidden; }
.hte-pane {
  display: none;
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
}
.hte-pane.hte-active { display: block; }

.hte-editor {
  width: 100%;
  height: 100%;
  min-height: 0;
  max-height: none;
  overflow: auto;
  padding: 24px 30px;
  font-size: 16px;
  line-height: 1.7;
  outline: none;
  background: var(--hte-surface);
}
.hte-editor h1 { font-size: 26px; margin: .3em 0 .2em; }
.hte-editor h2 { font-size: 21px; margin: .5em 0 .2em; }
.hte-editor h3 { font-size: 18px; margin: .5em 0 .2em; }
.hte-editor blockquote { margin: 12px 0; padding: 4px 16px; border-left: 3px solid var(--hte-accent); color: var(--hte-ink-soft); font-style: italic; }
.hte-editor img { max-width: 100%; border-radius: 4px; }
.hte-editor table { border-collapse: collapse; width: 100%; margin: 10px 0; }
.hte-editor table td, .hte-editor table th { border: 1px solid var(--hte-border); padding: 6px 10px; font-size: 14px; }
.hte-editor a { color: var(--hte-accent-dark); }
.hte-editor:empty:before { content: attr(data-placeholder); color: var(--hte-ink-soft); font-style: italic; }

.hte-code {
  display: block;
  box-sizing: border-box;
  width: 100%;
  height: 100%;
  min-height: 0;
  max-width: 100%;
  border: none;
  outline: none;
  resize: none;
  margin: 0;
  padding: 18px;
  font-family: Consolas, "Cascadia Code", "JetBrains Mono", monospace;
  font-size: 13.5px;
  line-height: 1.6;
  background: var(--hte-code-bg);
  color: var(--hte-code-fg);
  overflow: auto;
  white-space: pre;
}

.hte-preview-frame { display: block; box-sizing: border-box; width: 100%; height: 100%; min-height: 0; border: none; background: #fff; }

.hte-statusbar {
  display: flex;
  justify-content: space-between;
  padding: 7px 14px;
  border-top: 1px solid var(--hte-border);
  font-size: 11px;
  color: var(--hte-ink-soft);
  background: var(--hte-surface);
}

@media (max-width: 640px) {
  .hte-editor { padding: 16px; }
  .hte-panel { right: auto; left: 0; width: 240px; }
}

/* ---------- Custom modal (replaces window.prompt / alert) ---------- */
.hte-modal-root { position: absolute; inset: 0; pointer-events: none; z-index: 200; }
.hte-modal-overlay {
  position: absolute;
  inset: 0;
  background: rgba(20, 22, 26, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  pointer-events: auto;
}
.hte-modal {
  background: var(--hte-surface);
  border: 1px solid var(--hte-border);
  border-radius: 10px;
  padding: 18px 20px;
  width: 320px;
  max-width: calc(100% - 32px);
  box-shadow: 0 20px 50px rgba(0,0,0,0.25);
}
.hte-modal h3 { margin: 0 0 4px; font-size: 15px; color: var(--hte-ink); }
.hte-modal label { display: block; font-size: 12px; color: var(--hte-ink-soft); margin: 12px 0 4px; }
.hte-modal input {
  width: 100%;
  padding: 8px 10px;
  border: 1px solid var(--hte-border);
  border-radius: 6px;
  font-size: 13.5px;
  background: var(--hte-paper);
  color: var(--hte-ink);
  outline: none;
}
.hte-modal input:focus { border-color: var(--hte-accent); }
.hte-modal-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 18px; }
.hte-modal-btn {
  padding: 7px 14px;
  border-radius: 6px;
  border: 1px solid var(--hte-border);
  background: var(--hte-surface);
  cursor: pointer;
  font-size: 13px;
  font-weight: 600;
  color: var(--hte-ink);
}
.hte-modal-btn:hover { border-color: var(--hte-accent); }
.hte-modal-btn.hte-primary { background: var(--hte-accent); border-color: var(--hte-accent); color: #fff; }
.hte-modal-btn.hte-primary:hover { background: var(--hte-accent-dark); border-color: var(--hte-accent-dark); }

/* ---------- Toast (replaces window.alert for confirmations) ---------- */
.hte-toast {
  position: absolute;
  bottom: 46px;
  left: 50%;
  transform: translate(-50%, 8px);
  background: var(--hte-ink);
  color: var(--hte-surface);
  padding: 9px 16px;
  border-radius: 7px;
  font-size: 12.5px;
  font-weight: 500;
  opacity: 0;
  transition: opacity 0.2s ease, transform 0.2s ease;
  pointer-events: none;
  white-space: nowrap;
  box-shadow: 0 8px 20px rgba(0,0,0,0.18);
}
.hte-root[data-hte-theme="dark"] .hte-toast { background: var(--hte-accent); color: #0E1512; }
.hte-toast.hte-show { opacity: 1; transform: translate(-50%, 0); }

/* ---------- Drag-to-resize handle ---------- */
.hte-resize-handle {
  height: 12px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--hte-surface);
  border-top: 1px solid var(--hte-border);
  cursor: ns-resize;
  touch-action: none;
}
.hte-resize-handle .hte-grip {
  width: 36px;
  height: 4px;
  border-radius: 2px;
  background: var(--hte-border);
  transition: background 0.12s ease;
}
.hte-resize-handle:hover .hte-grip { background: var(--hte-accent); }
`;
  var style = document.createElement('style');
  style.setAttribute('data-hte-injected', 'true');
  style.textContent = css;
  document.head.appendChild(style);
})();

/*!
 * HtmlEditor — minimal reusable rich text / HTML source / preview editor.
 * Usage:
 *   HtmlEditor.create({ element: "#editor", theme: "light", height: "500px" });
 *
 * No dependencies. Drop in editor.css + this file. Multiple instances supported.
 */
(function (global) {
  'use strict';

  const ICONS = {
    undo: '<svg viewBox="0 0 24 24"><path d="M3 10h10a5 5 0 0 1 0 10H9"/><path d="M8 5 3 10l5 5"/></svg>',
    redo: '<svg viewBox="0 0 24 24"><path d="M21 10H11a5 5 0 0 0 0 10h4"/><path d="M16 5l5 5-5 5"/></svg>',
    alignLeft: '<svg viewBox="0 0 24 24"><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="15" y2="12"/><line x1="3" y1="18" x2="18" y2="18"/></svg>',
    alignCenter: '<svg viewBox="0 0 24 24"><line x1="3" y1="6" x2="21" y2="6"/><line x1="6" y1="12" x2="18" y2="12"/><line x1="4" y1="18" x2="20" y2="18"/></svg>',
    alignJustify: '<svg viewBox="0 0 24 24"><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/></svg>',
    ul: '<svg viewBox="0 0 24 24"><circle cx="4.5" cy="6" r="1"/><circle cx="4.5" cy="12" r="1"/><circle cx="4.5" cy="18" r="1"/><line x1="9" y1="6" x2="21" y2="6"/><line x1="9" y1="12" x2="21" y2="12"/><line x1="9" y1="18" x2="21" y2="18"/></svg>',
    ol: '<svg viewBox="0 0 24 24"><text x="1" y="8" font-size="7" stroke="none" fill="currentColor">1</text><text x="1" y="14" font-size="7" stroke="none" fill="currentColor">2</text><text x="1" y="20" font-size="7" stroke="none" fill="currentColor">3</text><line x1="9" y1="6" x2="21" y2="6"/><line x1="9" y1="12" x2="21" y2="12"/><line x1="9" y1="18" x2="21" y2="18"/></svg>',
    link: '<svg viewBox="0 0 24 24"><path d="M10 13a5 5 0 0 0 7 0l3-3a5 5 0 0 0-7-7l-1.5 1.5"/><path d="M14 11a5 5 0 0 0-7 0l-3 3a5 5 0 0 0 7 7l1.5-1.5"/></svg>',
    image: '<svg viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="16" rx="2"/><circle cx="9" cy="10" r="1.5"/><path d="M21 16l-5.5-5.5L4 21"/></svg>',
    table: '<svg viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="16" rx="1"/><line x1="3" y1="10" x2="21" y2="10"/><line x1="3" y1="16" x2="21" y2="16"/><line x1="10.5" y1="4" x2="10.5" y2="20"/></svg>',
    hr: '<svg viewBox="0 0 24 24"><line x1="3" y1="12" x2="21" y2="12"/></svg>',
    clear: '<svg viewBox="0 0 24 24"><path d="M4 7V4h13"/><path d="M9 20l4-13"/><line x1="4" y1="20" x2="12" y2="20"/><line x1="15" y1="15" x2="21" y2="21"/><line x1="21" y1="15" x2="15" y2="21"/></svg>'
  };

  const DEFAULT_TEMPLATES = [
    { name: 'Blank page', desc: 'Start fresh', html: '<p>Start writing here...</p>' },
    {
      name: 'Article', desc: 'Title, byline, body',
      html: '<h1>Article Title</h1>' +
        '<p style="color:#6B7178;font-style:italic;">By Author Name &middot; Date</p>' +
        '<p>Open with a strong first paragraph.</p>' +
        '<h2>A section heading</h2>' +
        '<p>Body copy goes here.</p>'
    },
    {
      name: 'Blog post', desc: 'Cover line, tags, sections',
      html: '<h1>Your Blog Post Title</h1>' +
        '<p style="color:#6B7178;">Posted on <strong>Date</strong> in <em>Category</em></p>' +
        '<p>Start with a hook that makes the reader want to keep going &mdash; one or two sentences that frame what this post is about.</p>' +
        '<h2>First section heading</h2>' +
        '<p>Write your main point here, with any supporting detail or example.</p>' +
        '<blockquote>An optional pull-quote or key takeaway can go here.</blockquote>' +
        '<h2>Second section heading</h2>' +
        '<ul><li>Key point one</li><li>Key point two</li><li>Key point three</li></ul>' +
        '<h2>Wrap up</h2>' +
        '<p>Close with a summary or a call to action.</p>'
    },
    {
      name: 'Newsletter', desc: 'Header banner + sections',
      html: '<h1 style="text-align:center;">Newsletter Title</h1>' +
        '<p style="text-align:center;color:#6B7178;">Monthly update &middot; Month Year</p>' +
        '<hr>' +
        '<h2>Headline story</h2>' +
        '<p>Summary of the main update goes here.</p>' +
        '<h2>In brief</h2>' +
        '<ul><li>Update one</li><li>Update two</li><li>Update three</li></ul>' +
        '<p style="text-align:center;color:#6B7178;font-size:13px;">You are receiving this email because you subscribed.</p>'
    },
    {
      name: 'Invoice / Receipt', desc: 'Billing table with totals',
      html: '<h1>Invoice</h1>' +
        '<p><strong>Invoice #:</strong> INV-0001 &nbsp; <strong>Date:</strong> DD/MM/YYYY</p>' +
        '<p><strong>Billed to:</strong><br>Customer Name<br>Address line<br>City, State</p>' +
        '<table><tr><th>Description</th><th>Qty</th><th>Rate</th><th>Amount</th></tr>' +
        '<tr><td>Service / item</td><td>1</td><td>0.00</td><td>0.00</td></tr>' +
        '<tr><td>Service / item</td><td>1</td><td>0.00</td><td>0.00</td></tr>' +
        '</table>' +
        '<p style="text-align:right;"><strong>Total: 0.00</strong></p>' +
        '<p style="color:#6B7178;font-size:14px;">Thank you for your business.</p>'
    },
    {
      name: 'Business letter', desc: 'Formal letter layout',
      html: '<p>Your Name<br>Your Address<br>City, State ZIP</p>' +
        '<p>Date</p>' +
        '<p>Recipient Name<br>Recipient Title<br>Company Name<br>Address</p>' +
        '<p>Dear Recipient Name,</p>' +
        '<p>Open with the purpose of the letter in one or two sentences.</p>' +
        '<p>Use the body paragraph(s) to provide detail, context, or requests.</p>' +
        '<p>Close with next steps or a call to action.</p>' +
        '<p>Sincerely,<br><br>Your Name</p>'
    },
    {
      name: 'Resume / CV', desc: 'Name, summary, experience, education',
      html: '<h1>Full Name</h1>' +
        '<p style="color:#6B7178;">City, State &middot; email@example.com &middot; (000) 000-0000</p>' +
        '<h2>Summary</h2>' +
        '<p>One or two sentences on your background and what you are looking for.</p>' +
        '<h2>Experience</h2>' +
        '<p><strong>Job Title</strong> &mdash; Company Name <span style="color:#6B7178;">(Start &ndash; End)</span></p>' +
        '<ul><li>Key responsibility or achievement</li><li>Key responsibility or achievement</li></ul>' +
        '<p><strong>Job Title</strong> &mdash; Company Name <span style="color:#6B7178;">(Start &ndash; End)</span></p>' +
        '<ul><li>Key responsibility or achievement</li><li>Key responsibility or achievement</li></ul>' +
        '<h2>Education</h2>' +
        '<p>Degree, School Name <span style="color:#6B7178;">(Year)</span></p>' +
        '<h2>Skills</h2>' +
        '<p>Skill one, skill two, skill three, skill four</p>'
    },
    {
      name: 'Landing page', desc: 'Hero, features, CTA',
      html: '<h1 style="text-align:center;">Product or Service Name</h1>' +
        '<p style="text-align:center;color:#6B7178;font-size:18px;">A one-line statement of the value you provide.</p>' +
        '<p style="text-align:center;"><a href="#">Get started &rarr;</a></p>' +
        '<hr>' +
        '<h2>Feature one</h2><p>Describe the first key benefit.</p>' +
        '<h2>Feature two</h2><p>Describe the second key benefit.</p>' +
        '<h2>Feature three</h2><p>Describe the third key benefit.</p>' +
        '<hr>' +
        '<p style="text-align:center;"><strong>Ready to get started?</strong></p>' +
        '<p style="text-align:center;"><a href="#">Sign up now</a></p>'
    },
    {
      name: 'FAQ page', desc: 'Question / answer pairs',
      html: '<h1>Frequently Asked Questions</h1>' +
        '<h3>Question one goes here?</h3><p>Answer to question one.</p>' +
        '<h3>Question two goes here?</h3><p>Answer to question two.</p>' +
        '<h3>Question three goes here?</h3><p>Answer to question three.</p>' +
        '<h3>Question four goes here?</h3><p>Answer to question four.</p>'
    }
  ];

  let instanceCounter = 0;

  function el(tag, attrs, html) {
    const node = document.createElement(tag);
    if (attrs) for (const k in attrs) node.setAttribute(k, attrs[k]);
    if (html !== undefined) node.innerHTML = html;
    return node;
  }

  function formatHTML(html) {
    html = (html || '').trim();
    const voidTags = new Set(['area','base','br','col','embed','hr','img','input','link','meta','param','source','track','wbr']);
    html = html.replace(/\r\n/g, '\n').replace(/>\s+</g, '><').replace(/</g, '\n<').replace(/>/g, '>\n');
    const lines = html.split('\n').map(x => x.trim()).filter(Boolean);
    let indent = 0;
    const out = [];
    for (const line of lines) {
      if (/^<\//.test(line)) indent = Math.max(indent - 1, 0);
      out.push('    '.repeat(indent) + line);
      if (/^<[^!?/][^>]*>$/.test(line) && !line.endsWith('/>')) {
        const tag = line.match(/^<([a-zA-Z0-9-]+)/);
        if (tag && !voidTags.has(tag[1].toLowerCase()) && !line.includes('</')) indent++;
      }
    }
    return out.join('\n');
  }

  function sanitizePastedHTML(html) {
    const container = document.createElement('div');
    container.innerHTML = html || '';
    container.querySelectorAll('script, style, meta, link, iframe, object, embed').forEach(node => node.remove());
    container.querySelectorAll('*').forEach(node => {
      [...node.attributes].forEach(attr => {
        const name = attr.name.toLowerCase();
        if (name.startsWith('on') || name === 'style' && /expression\(/i.test(attr.value)) {
          node.removeAttribute(attr.name);
        }
      });
    });
    return container.innerHTML;
  }

  function buildDocument(bodyHTML) {
    return '<!DOCTYPE html><html><head><meta charset="UTF-8"><style>' +
      'body{font-family:-apple-system,sans-serif;font-size:16px;line-height:1.7;color:#20242A;max-width:100%;margin:28px auto;padding:0 24px;}' +
      'blockquote{border-left:3px solid #146C6A;margin:12px 0;padding:4px 16px;color:#6B7178;font-style:italic;}' +
      'table{border-collapse:collapse;width:100%;margin:10px 0;} td,th{border:1px solid #E1DCCF;padding:6px 10px;font-size:14px;}' +
      'img{max-width:100%;border-radius:4px;} a{color:#0E5250;}' +
      '</style></head><body>' + bodyHTML + '</body></html>';
  }

  class HtmlEditorInstance {
    constructor(hostEl, options) {
      this.host = hostEl;
      this.opts = Object.assign({
        theme: 'light',
        height: '480px',
        placeholder: 'Start writing...',
        initialHTML: null,
        templates: [],
        resizable: true,
        minHeight: 220
      }, options || {});
      this.id = 'hte' + (++instanceCounter);
      this.currentTab = 'edit';
      this.savedRange = null;
      this.templates = DEFAULT_TEMPLATES.concat(this.opts.templates || []);
      this.savedTemplates = [];
      this.listeners = { change: [], ready: [] };
      this._build();
      this._bind();
      this.setHTML(this.opts.initialHTML != null ? this.opts.initialHTML : this.templates[1].html);
      this._emit('ready', this);
    }

    _build() {
      this.host.innerHTML = '';
      this.root = el('div', { class: 'hte-root', 'data-hte-theme': this.opts.theme, style: `height:${this.opts.height};display:flex;flex-direction:column;` });

      // Toolbar
      this.toolbar = el('div', { class: 'hte-toolbar' });
      this.toolbar.innerHTML =
        `<div class="hte-tgroup">
           <button class="hte-btn" data-act="undo" title="Undo">${ICONS.undo}</button>
           <button class="hte-btn" data-act="redo" title="Redo">${ICONS.redo}</button>
         </div>
         <div class="hte-tgroup">
           <select class="hte-sel" data-role="formatSelect" title="Paragraph style">
             <option value="p">Paragraph</option>
             <option value="h1">Heading 1</option>
             <option value="h2">Heading 2</option>
             <option value="h3">Heading 3</option>
             <option value="blockquote">Quote</option>
             <option value="pre">Code block</option>
           </select>
         </div>
         <div class="hte-tgroup">
           <button class="hte-btn" data-cmd="bold" title="Bold (Ctrl+B)"><b>B</b></button>
           <button class="hte-btn hte-italic" data-cmd="italic" title="Italic (Ctrl+I)">I</button>
           <button class="hte-btn hte-underline" data-cmd="underline" title="Underline (Ctrl+U)">U</button>
           <button class="hte-btn hte-strike" data-cmd="strikeThrough" title="Strikethrough">S</button>
         </div>
         <div class="hte-tgroup">
           <button class="hte-btn" data-cmd="justifyLeft" title="Align left">${ICONS.alignLeft}</button>
           <button class="hte-btn" data-cmd="justifyCenter" title="Align center">${ICONS.alignCenter}</button>
           <button class="hte-btn" data-cmd="justifyFull" title="Justify">${ICONS.alignJustify}</button>
         </div>
         <div class="hte-tgroup">
           <button class="hte-btn" data-cmd="insertUnorderedList" title="Bullet list">${ICONS.ul}</button>
           <button class="hte-btn" data-cmd="insertOrderedList" title="Numbered list">${ICONS.ol}</button>
         </div>
         <div class="hte-tgroup">
           <button class="hte-btn" data-act="link" title="Insert link">${ICONS.link}</button>
           <button class="hte-btn" data-act="image" title="Insert image">${ICONS.image}</button>
           <button class="hte-btn" data-act="table" title="Insert table">${ICONS.table}</button>
           <button class="hte-btn" data-act="hr" title="Horizontal rule">${ICONS.hr}</button>
         </div>
         <div class="hte-tgroup" style="border-right:none;">
           <button class="hte-btn" data-cmd="removeFormat" title="Clear formatting">${ICONS.clear}</button>
         </div>
         <div class="hte-spacer"></div>
         <div class="hte-wrap">
           <button class="hte-btn-wide" data-act="toggleTemplates">Templates &#9662;</button>
           <div class="hte-panel" data-role="tmplPanel"></div>
         </div>
         <button class="hte-btn-wide" data-act="export" title="Download as .html file">Export</button>`;

      // View tabs
      this.viewtabs = el('div', { class: 'hte-viewtabs' });
      this.viewtabs.innerHTML =
        `<button class="hte-vtab hte-active" data-tab="edit">Edit</button>
         <button class="hte-vtab" data-tab="code">HTML</button>
         <button class="hte-vtab" data-tab="preview">Preview</button>`;

      // Panes
      this.panes = el('div', { class: 'hte-panes' });
      this.paneEdit = el('div', { class: 'hte-pane hte-active' });
      this.editor = el('div', { class: 'hte-editor', contenteditable: 'true', 'data-placeholder': this.opts.placeholder });
      this.paneEdit.appendChild(this.editor);

      this.paneCode = el('div', { class: 'hte-pane' });
      this.codeArea = el('textarea', { class: 'hte-code', spellcheck: 'false' });
      this.paneCode.appendChild(this.codeArea);

      this.panePreview = el('div', { class: 'hte-pane' });
      this.previewFrame = el('iframe', { class: 'hte-preview-frame', title: 'Preview' });
      this.panePreview.appendChild(this.previewFrame);

      this.panes.appendChild(this.paneEdit);
      this.panes.appendChild(this.paneCode);
      this.panes.appendChild(this.panePreview);

      // Status bar
      this.statusbar = el('div', { class: 'hte-statusbar' });
      this.wordCountEl = el('span', {}, '0 words');
      this.statusbar.appendChild(this.wordCountEl);
      this.statusbar.appendChild(el('span', {}, 'HtmlEditor'));

      this.imgInput = el('input', { type: 'file', accept: 'image/*', hidden: 'hidden' });
      this.modalRoot = el('div', { class: 'hte-modal-root' });
      this.resizeHandle = el('div', { class: 'hte-resize-handle', title: 'Drag to resize' }, '<span class="hte-grip"></span>');

      this.root.appendChild(this.toolbar);
      this.root.appendChild(this.viewtabs);
      this.root.appendChild(this.panes);
      this.root.appendChild(this.statusbar);
      if (this.opts.resizable) this.root.appendChild(this.resizeHandle);
      this.root.appendChild(this.imgInput);
      this.root.appendChild(this.modalRoot);
      this.host.appendChild(this.root);

      this.tmplPanel = this.toolbar.querySelector('[data-role="tmplPanel"]');
      this.formatSelect = this.toolbar.querySelector('[data-role="formatSelect"]');
      this._renderTemplatePanel();
    }

    _bind() {
      const editor = this.editor;

      const saveSelection = () => {
        const sel = window.getSelection();
        if (sel && sel.rangeCount > 0 && editor.contains(sel.anchorNode)) {
          this.savedRange = sel.getRangeAt(0).cloneRange();
        }
      };
      const restoreSelection = () => {
        if (this.savedRange) {
          const sel = window.getSelection();
          sel.removeAllRanges();
          sel.addRange(this.savedRange);
        }
      };
      this._saveSelection = saveSelection;
      this._restoreSelection = restoreSelection;

      editor.addEventListener('keyup', saveSelection);
      editor.addEventListener('mouseup', saveSelection);
      editor.addEventListener('input', () => { this._updateWordCount(); this._emit('change', this.getHTML()); });
      editor.addEventListener('paste', (e) => {
        e.preventDefault();
        const clipboard = e.clipboardData || window.clipboardData;
        const rawHTML = clipboard.getData('text/html');
        const rawText = clipboard.getData('text/plain');
        const clean = rawHTML ? sanitizePastedHTML(rawHTML) : (rawText || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, '<br>');
        document.execCommand('insertHTML', false, clean);
        this._updateWordCount();
        this._emit('change', this.getHTML());
      });

      // Prevent toolbar buttons from stealing focus
      this.toolbar.querySelectorAll('button, select').forEach(node => {
        node.addEventListener('mousedown', () => saveSelection());
      });

      this.toolbar.querySelectorAll('[data-cmd]').forEach(btn => {
        btn.addEventListener('click', () => {
          editor.focus();
          restoreSelection();
          document.execCommand(btn.getAttribute('data-cmd'), false, null);
          saveSelection();
          this._updateWordCount();
          this._emit('change', this.getHTML());
        });
      });

      this.formatSelect.addEventListener('change', (e) => {
        editor.focus();
        restoreSelection();
        document.execCommand('formatBlock', false, e.target.value);
        e.target.selectedIndex = 0;
        saveSelection();
      });

      this.toolbar.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-act]');
        if (!btn) return;
        const act = btn.getAttribute('data-act');
        if (act === 'undo') { editor.focus(); document.execCommand('undo'); }
        else if (act === 'redo') { editor.focus(); document.execCommand('redo'); }
        else if (act === 'hr') { editor.focus(); restoreSelection(); document.execCommand('insertHorizontalRule'); }
        else if (act === 'link') {
          const result = await this._showModal({
            title: 'Insert link',
            okText: 'Insert',
            fields: [{ name: 'url', label: 'URL', type: 'url', placeholder: 'https://example.com' }]
          });
          if (result && result.url) {
            editor.focus();
            restoreSelection();
            document.execCommand('createLink', false, result.url);
            this._emit('change', this.getHTML());
          }
        } else if (act === 'table') {
          const result = await this._showModal({
            title: 'Insert table',
            okText: 'Insert',
            fields: [
              { name: 'rows', label: 'Rows', type: 'number', value: 3 },
              { name: 'cols', label: 'Columns', type: 'number', value: 3 }
            ]
          });
          if (result) {
            const rows = parseInt(result.rows) || 3;
            const cols = parseInt(result.cols) || 3;
            editor.focus();
            restoreSelection();
            let html = '<table>';
            for (let r = 0; r < rows; r++) {
              html += '<tr>';
              for (let c = 0; c < cols; c++) html += r === 0 ? '<th>Header</th>' : '<td>Cell</td>';
              html += '</tr>';
            }
            html += '</table><p></p>';
            document.execCommand('insertHTML', false, html);
            this._emit('change', this.getHTML());
          }
        } else if (act === 'image') {
          saveSelection();
          this.imgInput.value = '';
          this.imgInput.click();
        } else if (act === 'toggleTemplates') {
          this.tmplPanel.classList.toggle('hte-open');
        } else if (act === 'export') {
          this._exportHTML();
          this._showToast('Exported document.html');
        }
        this._updateWordCount();
      });

      this.imgInput.addEventListener('change', async () => {
        const file = this.imgInput.files[0];
        if (!file) return;
        const dataUrl = await new Promise((resolve) => {
          const reader = new FileReader();
          reader.onload = () => resolve(reader.result);
          reader.readAsDataURL(file);
        });
        const result = await this._showModal({
          title: 'Insert image',
          okText: 'Insert',
          fields: [{ name: 'alt', label: 'Alt text (optional)', type: 'text', value: file.name.replace(/\.[a-z0-9]+$/i, '') }]
        });
        if (result === null) return;
        editor.focus();
        restoreSelection();
        const alt = (result.alt || '').replace(/"/g, '');
        document.execCommand('insertHTML', false, '<img src="' + dataUrl + '" alt="' + alt + '">');
        this._updateWordCount();
        this._emit('change', this.getHTML());
      });

      document.addEventListener('click', (e) => {
        if (!this.tmplPanel.contains(e.target) && !e.target.closest('[data-act="toggleTemplates"]')) {
          this.tmplPanel.classList.remove('hte-open');
        }
      });

      this.viewtabs.querySelectorAll('[data-tab]').forEach(t => {
        t.addEventListener('click', () => this._switchTab(t.getAttribute('data-tab')));
      });

      if (this.opts.resizable) {
        let resizing = false, startY = 0, startHeight = 0;
        const onMove = (e) => {
          if (!resizing) return;
          const clientY = e.touches ? e.touches[0].clientY : e.clientY;
          const delta = clientY - startY;
          const next = Math.max(this.opts.minHeight, startHeight + delta);
          this.root.style.height = next + 'px';
        };
        const onUp = () => {
          if (!resizing) return;
          resizing = false;
          document.body.style.userSelect = '';
          document.removeEventListener('mousemove', onMove);
          document.removeEventListener('mouseup', onUp);
          document.removeEventListener('touchmove', onMove);
          document.removeEventListener('touchend', onUp);
        };
        const onDown = (e) => {
          resizing = true;
          startY = e.touches ? e.touches[0].clientY : e.clientY;
          startHeight = this.root.getBoundingClientRect().height;
          document.body.style.userSelect = 'none';
          document.addEventListener('mousemove', onMove);
          document.addEventListener('mouseup', onUp);
          document.addEventListener('touchmove', onMove, { passive: false });
          document.addEventListener('touchend', onUp);
          e.preventDefault();
        };
        this.resizeHandle.addEventListener('mousedown', onDown);
        this.resizeHandle.addEventListener('touchstart', onDown, { passive: false });
      }
    }

    _renderTemplatePanel() {
      let html = '';
      this.templates.forEach((t, i) => {
        html += `<div class="hte-row" data-src="built" data-idx="${i}"><div class="hte-name">${t.name}</div><div class="hte-desc">${t.desc || ''}</div></div>`;
      });
      if (this.savedTemplates.length) {
        html += '<div class="hte-divider"></div>';
        this.savedTemplates.forEach((t, i) => {
          html += `<div class="hte-row" data-src="saved" data-idx="${i}"><div class="hte-name">${t.name}</div><div class="hte-desc">saved this session</div></div>`;
        });
      }
      html += '<div class="hte-divider"></div><div class="hte-save-row" data-act="saveTemplate">+ Save current as template</div>';
      this.tmplPanel.innerHTML = html;

      this.tmplPanel.querySelectorAll('.hte-row').forEach(row => {
        row.addEventListener('click', () => {
          const src = row.getAttribute('data-src');
          const idx = parseInt(row.getAttribute('data-idx'));
          const tpl = src === 'built' ? this.templates[idx] : this.savedTemplates[idx];
          this.setHTML(tpl.html);
          this._switchTab('edit');
          this.tmplPanel.classList.remove('hte-open');
        });
      });
      const saveRow = this.tmplPanel.querySelector('[data-act="saveTemplate"]');
      if (saveRow) saveRow.addEventListener('click', async () => {
        const result = await this._showModal({
          title: 'Save as template',
          okText: 'Save',
          fields: [{ name: 'name', label: 'Template name', type: 'text', placeholder: 'e.g. Weekly report' }]
        });
        if (!result || !result.name) return;
        if (this.currentTab === 'code') this.editor.innerHTML = this.codeArea.value;
        this.savedTemplates.push({ name: result.name, html: this.editor.innerHTML });
        this._renderTemplatePanel();
        this._showToast('Template "' + result.name + '" saved');
      });
    }

    _switchTab(tab) {
      if (this.currentTab === 'code') this.editor.innerHTML = this.codeArea.value;
      this.currentTab = tab;

      this.viewtabs.querySelectorAll('.hte-vtab').forEach(t => t.classList.toggle('hte-active', t.getAttribute('data-tab') === tab));
      [this.paneEdit, this.paneCode, this.panePreview].forEach(p => p.classList.remove('hte-active'));
      ({ edit: this.paneEdit, code: this.paneCode, preview: this.panePreview })[tab].classList.add('hte-active');

      if (tab === 'code') this.codeArea.value = formatHTML(this.editor.innerHTML);
      if (tab === 'preview') this.previewFrame.srcdoc = buildDocument(this.editor.innerHTML);
      if (tab === 'edit') this._updateWordCount();
      this._emit('change', this.getHTML());
    }

    _exportHTML() {
      if (this.currentTab === 'code') this.editor.innerHTML = this.codeArea.value;
      const full = buildDocument(this.editor.innerHTML);
      const blob = new Blob([full], { type: 'text/html' });
      const url = URL.createObjectURL(blob);
      const a = el('a', { href: url, download: 'document.html' });
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    }

    _updateWordCount() {
      const text = this.editor.innerText.trim();
      const n = text.length ? text.split(/\s+/).length : 0;
      this.wordCountEl.textContent = n + (n === 1 ? ' word' : ' words');
    }

    _emit(evt, payload) {
      (this.listeners[evt] || []).forEach(cb => { try { cb(payload); } catch (err) { console.error(err); } });
    }

    _showModal({ title, fields = [], okText = 'OK', cancelText = 'Cancel' }) {
      return new Promise((resolve) => {
        const overlay = el('div', { class: 'hte-modal-overlay' });
        const box = el('div', { class: 'hte-modal' });

        let inner = '<h3>' + title + '</h3>';
        fields.forEach(f => {
          inner += '<label>' + f.label + '</label>';
          const val = f.value != null ? String(f.value).replace(/"/g, '&quot;') : '';
          inner += '<input data-field="' + f.name + '" type="' + (f.type || 'text') + '" value="' + val + '" placeholder="' + (f.placeholder || '') + '">';
        });
        inner += '<div class="hte-modal-actions">' +
          '<button type="button" class="hte-modal-btn" data-act="cancel">' + cancelText + '</button>' +
          '<button type="button" class="hte-modal-btn hte-primary" data-act="ok">' + okText + '</button>' +
          '</div>';
        box.innerHTML = inner;
        overlay.appendChild(box);
        this.modalRoot.appendChild(overlay);

        const collect = () => {
          const out = {};
          fields.forEach(f => {
            const node = box.querySelector('[data-field="' + f.name + '"]');
            out[f.name] = node ? node.value : null;
          });
          return out;
        };
        const cleanup = (result) => {
          overlay.remove();
          document.removeEventListener('keydown', onKey);
          resolve(result);
        };

        box.querySelector('[data-act="ok"]').addEventListener('click', () => cleanup(collect()));
        box.querySelector('[data-act="cancel"]').addEventListener('click', () => cleanup(null));
        overlay.addEventListener('mousedown', (e) => { if (e.target === overlay) cleanup(null); });

        const onKey = (e) => {
          if (e.key === 'Escape') cleanup(null);
          else if (e.key === 'Enter') { e.preventDefault(); cleanup(collect()); }
        };
        document.addEventListener('keydown', onKey);

        const firstInput = box.querySelector('input');
        if (firstInput) { firstInput.focus(); firstInput.select(); }
      });
    }

    _showToast(message) {
      const toast = el('div', { class: 'hte-toast' }, message);
      this.modalRoot.appendChild(toast);
      requestAnimationFrame(() => toast.classList.add('hte-show'));
      setTimeout(() => {
        toast.classList.remove('hte-show');
        setTimeout(() => toast.remove(), 200);
      }, 2200);
    }

    // ---------------- Public API ----------------
    getHTML() {
      if (this.currentTab === 'code') this.editor.innerHTML = this.codeArea.value;
      return this.editor.innerHTML;
    }
    setHTML(html) {
      this.editor.innerHTML = html || '';
      if (this.currentTab === 'code') this.codeArea.value = formatHTML(this.editor.innerHTML);
      if (this.currentTab === 'preview') this.previewFrame.srcdoc = buildDocument(this.editor.innerHTML);
      this._updateWordCount();
      this._emit('change', this.getHTML());
    }
    insertHTML(html) {
      this.editor.focus();
      this._restoreSelection();
      document.execCommand('insertHTML', false, html);
      this._updateWordCount();
      this._emit('change', this.getHTML());
    }
    setTheme(theme) {
      this.opts.theme = theme;
      this.root.setAttribute('data-hte-theme', theme);
    }
    setHeight(height) {
      this.root.style.height = typeof height === 'number' ? height + 'px' : height;
    }
    format() {
      if (this.currentTab === 'code') this.codeArea.value = formatHTML(this.codeArea.value);
      else this.codeArea.value = formatHTML(this.editor.innerHTML);
    }
    preview() { this._switchTab('preview'); }
    exportHTML() { this._exportHTML(); }
    on(evt, cb) {
      if (!this.listeners[evt]) this.listeners[evt] = [];
      this.listeners[evt].push(cb);
      return this;
    }
    destroy() {
      this.host.innerHTML = '';
    }
  }

  const HtmlEditor = {
    _instances: new WeakMap(),
    create(options) {
      options = options || {};
      const target = typeof options.element === 'string' ? document.querySelector(options.element) : options.element;
      if (!target) throw new Error('HtmlEditor.create: element not found for "' + options.element + '"');
      const instance = new HtmlEditorInstance(target, options);
      this._instances.set(target, instance);
      return instance;
    },
    destroy(target) {
      const node = typeof target === 'string' ? document.querySelector(target) : target;
      const inst = this._instances.get(node);
      if (inst) { inst.destroy(); this._instances.delete(node); }
    }
  };

  global.HtmlEditor = HtmlEditor;
})(window);
