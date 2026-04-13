/**
 * Router and layout composition for CRQC Index.
 *
 * Layout composition lives in JSX because SolidJS requires {props.children}
 * to be in native JSX for the compiler to track route transitions reactively.
 * Partas.Solid's splitProps mechanism breaks this — PARTAS_LOCAL.children is
 * treated as a static read by the Solid compiler, so route swaps never update.
 *
 * All UI components (HamburgerIcon, Sidebar, ThemeToggle, page views) are
 * defined in F#. This file handles only:
 *   - Layout structure (header + sidebar + content area)
 *   - Theme and sidebar state (closures passed as props to F# components)
 *   - Router/Route wiring with reactive {props.children}
 *   - MetaProvider context
 */
import { Router, Route } from '@solidjs/router';
import { MetaProvider } from '@solidjs/meta';
import { createSignal, Show } from 'solid-js';
import { HamburgerIcon, Sidebar } from '../output/src/Layout.fs.jsx';
import { ThemeToggle } from '../output/src/Components.fs.jsx';
import { pageBackground, container, comingSoonBadge } from '../output/src/Styles.fs.jsx';
import { DashboardView, SectionListPage, ArticlePage } from '../output/src/Pages.fs.jsx';

function RootLayout(props) {
  // Theme state
  const [isDark, setIsDark] = createSignal(localStorage.getItem('theme') !== 'light');

  const toggleTheme = () => {
    const newDark = !isDark();
    setIsDark(newDark);
    if (newDark) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  };

  // Sidebar state
  const [sidebarOpen, setSidebarOpen] = createSignal(true);
  const toggleSidebar = () => setSidebarOpen(!sidebarOpen());

  const sidebarClass = () => {
    const base = 'border-r border-speakez-neutral/10 dark:border-speakez-neutral-light/10 transition-all duration-300 ease-in-out overflow-hidden flex-shrink-0 ';
    return sidebarOpen()
      ? base + 'fixed md:relative z-40 md:z-auto w-64 h-full'
      : base + 'hidden md:block w-16';
  };

  return (
    <div class={pageBackground}>
      {/* Header */}
      <header class="py-4 border-b border-speakez-neutral/10 dark:border-speakez-neutral-light/10">
        <div class="max-w-full mx-auto px-4">
          <div class="flex justify-between items-center">
            <div class="flex items-center gap-3">
              <HamburgerIcon isOpen={sidebarOpen} onToggle={toggleSidebar} />
              <div class={sidebarOpen() ? 'hidden' : 'flex items-center gap-3'}>
                <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-crqc-indigo to-speakez-teal flex items-center justify-center">
                  <span class="text-white font-bold font-mono text-sm">Q</span>
                </div>
                <span class="text-lg font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light hidden sm:inline">
                  CRQC Index
                </span>
              </div>
            </div>
            <nav class="flex items-center gap-4">
              <span class={comingSoonBadge + ' hidden sm:inline-flex'}>Coming Soon</span>
              <ThemeToggle isDark={isDark} onToggle={toggleTheme} />
            </nav>
          </div>
        </div>
      </header>

      {/* Sidebar + Content */}
      <div class="flex relative" style="height: calc(100vh - 65px)">
        <Show when={sidebarOpen()}>
          <div
            class="fixed inset-0 bg-black/30 z-30 md:hidden transition-opacity duration-300"
            onClick={() => setSidebarOpen(false)}
          />
        </Show>

        <div class={sidebarClass()}>
          <Sidebar
            onNavigate={() => { if (window.innerWidth < 768) setSidebarOpen(false); }}
            expanded={sidebarOpen}
          />
        </div>

        {/* Route content — {props.children} in native JSX is reactive */}
        <main class="flex-1 h-full overflow-y-auto">
          <div class={container + ' py-4'}>
            {props.children}
          </div>
        </main>
      </div>
    </div>
  );
}

export function AppRouter() {
  return (
    <MetaProvider>
      <Router root={RootLayout}>
        <Route path="/" component={DashboardView} />
        <Route path="/:section" component={SectionListPage} />
        <Route path="/:section/:slug" component={ArticlePage} />
      </Router>
    </MetaProvider>
  );
}
