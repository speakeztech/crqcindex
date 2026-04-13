/**
 * Reactive content router for CRQC Index.
 * This is in JSX because SolidJS reactive conditionals (Show/Switch)
 * need to read signals inside JSX expressions for proper tracking.
 */
import { createMemo, Show, Switch, Match } from 'solid-js';
import { getContent, getSection } from './content.js';

// Section metadata
const sectionMeta = {
  'bulletins': { title: 'Bulletins', icon: '\u26A1' },
  'analysis': { title: 'Analysis', icon: '\uD83D\uDD2C' },
  'methodology': { title: 'Methodology', icon: '\uD83D\uDCCA' },
  'fud-files': { title: 'FUD Files', icon: '\uD83D\uDEE1\uFE0F' },
};

function SectionList(props) {
  const items = () => getSection(props.section);
  const meta = () => sectionMeta[props.section] || { title: props.section, icon: '\uD83D\uDCC4' };
  return (
    <div class="py-8">
      <div class="flex items-center gap-3 mb-8">
        <div class="w-10 h-10 rounded-lg bg-crqc-indigo/10 dark:bg-crqc-indigo-light/10 flex items-center justify-center">
          <span class="text-lg">{meta().icon}</span>
        </div>
        <h1 class="text-3xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light">
          {meta().title}
        </h1>
      </div>
      <div class="space-y-4">
        <Show when={items().length > 0} fallback={
          <div class="bg-white dark:bg-speakez-neutral rounded-lg shadow-lg overflow-hidden border border-speakez-neutral/10 dark:border-speakez-neutral-light/10 p-8 text-center">
            <p class="text-base text-speakez-neutral/70 dark:text-speakez-neutral-light/70">No content published yet.</p>
          </div>
        }>
          {items().map(item => (
            <a href={`#/${item.path}`}
               class="bg-white dark:bg-speakez-neutral rounded-lg shadow-lg overflow-hidden border border-speakez-neutral/10 dark:border-speakez-neutral-light/10 block p-6 hover:shadow-xl transition-shadow group">
              <div class="flex justify-between items-start">
                <div>
                  <h3 class="font-semibold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-1 group-hover:text-crqc-indigo dark:group-hover:text-crqc-indigo-light transition-colors">
                    {item.frontmatter.title}
                  </h3>
                  <span class="text-sm text-speakez-neutral/50 dark:text-speakez-neutral-light/50 font-mono">
                    {item.frontmatter.date}
                  </span>
                </div>
                <span class="text-speakez-neutral/30 dark:text-speakez-neutral-light/30 group-hover:text-crqc-indigo transition-colors">
                  →
                </span>
              </div>
            </a>
          ))}
        </Show>
      </div>
    </div>
  );
}

function ArticlePage(props) {
  const content = () => getContent(props.section + '/' + props.slug);
  return (
    <Show when={content()} fallback={
      <div class="py-12 text-center">
        <h2 class="text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-4">
          Content Not Found
        </h2>
      </div>
    }>
      {entry => {
        const Component = entry().Component;
        const fm = entry().frontmatter;
        return (
          <div class="py-8">
            <header class="mb-8">
              <div class="flex items-center gap-2 mb-3">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light">
                  {props.section}
                </span>
                <span class="text-sm text-speakez-neutral/50 dark:text-speakez-neutral-light/50 font-mono">
                  {fm.date}
                </span>
              </div>
              <h1 class="text-3xl sm:text-4xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light">
                {fm.title}
              </h1>
              <div class="w-16 h-1 rounded-full bg-gradient-to-r from-crqc-indigo to-speakez-teal mt-6" />
            </header>
            <Component />
          </div>
        );
      }}
    </Show>
  );
}

/**
 * Reactive router component.
 * Reads the currentRoute signal inside createMemo so SolidJS tracks it.
 * Switch/Match ensures proper DOM swapping on route changes.
 */
export function ContentRouter(props) {
  const routeInfo = createMemo(() => {
    const route = props.currentRoute();
    const parts = route.replace(/^\//, '').split('/');
    const section = parts[0] || '';
    const slug = parts.length > 1 ? parts.slice(1).join('/') : '';
    return { section, slug };
  });

  return (
    <Switch fallback={props.dashboard}>
      <Match when={routeInfo().section !== '' && routeInfo().slug !== ''}>
        <ArticlePage section={routeInfo().section} slug={routeInfo().slug} />
      </Match>
      <Match when={routeInfo().section !== ''}>
        <SectionList section={routeInfo().section} />
      </Match>
    </Switch>
  );
}
