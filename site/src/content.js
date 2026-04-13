/**
 * Content index for CRQC Index.
 *
 * solid-marked + vite-plugin-solid-marked compile each .md file into a SolidJS
 * component module with:
 *   - default export: the rendered Component
 *   - named export: frontmatter (parsed YAML/TOML)
 *   - named export: TableOfContents (auto-generated heading nav)
 *
 * This module eagerly imports all content and builds a navigable index.
 */

const contentModules = import.meta.glob('/content/**/*.md', { eager: true });

export const contentIndex = {};
export const sectionIndex = { bulletins: [], analysis: [], methodology: [], 'fud-files': [] };

for (const [path, mod] of Object.entries(contentModules)) {
  const parts = path.replace('/content/', '').replace('.md', '').split('/');
  const section = parts[0];
  const slug = parts.slice(1).join('/');

  const entry = {
    Component: mod.default,
    TableOfContents: mod.TableOfContents,
    frontmatter: mod.frontmatter || {},
    section,
    slug,
    path: `${section}/${slug}`,
  };

  contentIndex[entry.path] = entry;
  if (sectionIndex[section]) {
    sectionIndex[section].push(entry);
  }
}

// Sort each section by date descending
for (const section of Object.values(sectionIndex)) {
  section.sort((a, b) => (b.frontmatter.date || '').localeCompare(a.frontmatter.date || ''));
}

export function getContent(sectionSlug) {
  return contentIndex[sectionSlug] || null;
}

export function getSection(section) {
  return sectionIndex[section] || [];
}
