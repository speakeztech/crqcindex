/**
 * MDX builtins provider for CRQC Index.
 * This module is referenced by vite-plugin-solid-marked via the `source` option.
 * It defines how every markdown node type renders in the site's visual language.
 *
 * solid-marked compiles markdown → SolidJS components that call useMDX()
 * to look up the renderer for each node type. This module supplies those renderers.
 */

export function useMDX() {
  return {
    builtins: {
      Root(props) {
        return <div class="max-w-none">{props.children}</div>;
      },
      Heading(props) {
        const baseClass = "font-heading text-speakez-neutral dark:text-speakez-neutral-light";
        const sizeClass =
          props.depth === 1 ? "text-3xl sm:text-4xl font-bold mt-8 mb-4" :
          props.depth === 2 ? "text-2xl font-bold mt-8 mb-3" :
          props.depth === 3 ? "text-xl font-semibold mt-6 mb-2" :
          "text-lg font-semibold mt-4 mb-2";
        const cls = `${baseClass} ${sizeClass}`;
        switch (props.depth) {
          case 1: return <h1 id={props.id} class={cls}>{props.children}</h1>;
          case 2: return <h2 id={props.id} class={cls}>{props.children}</h2>;
          case 3: return <h3 id={props.id} class={cls}>{props.children}</h3>;
          case 4: return <h4 id={props.id} class={cls}>{props.children}</h4>;
          case 5: return <h5 id={props.id} class={cls}>{props.children}</h5>;
          default: return <h6 id={props.id} class={cls}>{props.children}</h6>;
        }
      },
      Paragraph(props) {
        return <p class="text-base text-speakez-neutral/80 dark:text-speakez-neutral-light/80 leading-relaxed mb-4">{props.children}</p>;
      },
      Blockquote(props) {
        return <blockquote class="border-l-4 border-crqc-indigo/30 dark:border-crqc-indigo-light/30 pl-4 my-4 italic text-speakez-neutral/70 dark:text-speakez-neutral-light/70">{props.children}</blockquote>;
      },
      Code(props) {
        return (
          <pre class="bg-speakez-neutral-dark rounded-lg p-4 my-4 overflow-x-auto">
            <code class={`font-mono text-sm text-speakez-neutral-light ${props.lang ? `language-${props.lang}` : ''}`}>
              {props.children}
            </code>
          </pre>
        );
      },
      InlineCode(props) {
        return <code class="font-mono text-sm bg-speakez-neutral/10 dark:bg-speakez-neutral-light/10 px-1.5 py-0.5 rounded">{props.children}</code>;
      },
      Link(props) {
        return <a href={props.url} title={props.title} class="text-crqc-indigo dark:text-crqc-indigo-light hover:underline font-medium">{props.children}</a>;
      },
      Image(props) {
        return <img src={props.url} alt={props.alt} title={props.title} class="rounded-lg my-4 max-w-full" />;
      },
      List(props) {
        const listClass = "list-inside space-y-1 mb-4 text-speakez-neutral/80 dark:text-speakez-neutral-light/80";
        if (props.ordered) {
          return <ol class={`list-decimal ${listClass}`} start={props.start}>{props.children}</ol>;
        }
        return <ul class={`list-disc ${listClass}`}>{props.children}</ul>;
      },
      ListItem(props) {
        return <li class="leading-relaxed">{props.children}</li>;
      },
      ThematicBreak() {
        return <hr class="my-8 border-t border-speakez-neutral/20 dark:border-speakez-neutral-light/20" />;
      },
      Strong(props) {
        return <strong class="font-semibold text-speakez-neutral dark:text-speakez-neutral-light">{props.children}</strong>;
      },
      Emphasis(props) {
        return <em>{props.children}</em>;
      },
      Delete(props) {
        return <del class="text-speakez-neutral/50 dark:text-speakez-neutral-light/50">{props.children}</del>;
      },
      Table(props) {
        return (
          <div class="overflow-x-auto my-4">
            <table class="min-w-full border border-speakez-neutral/10 dark:border-speakez-neutral-light/10 rounded-lg">
              {props.children}
            </table>
          </div>
        );
      },
      TableHead(props) {
        return <thead class="bg-speakez-neutral/5 dark:bg-speakez-neutral-light/5">{props.children}</thead>;
      },
      TableBody(props) {
        return <tbody>{props.children}</tbody>;
      },
      TableRow(props) {
        return <tr class="border-b border-speakez-neutral/10 dark:border-speakez-neutral-light/10">{props.children}</tr>;
      },
      TableCell(props) {
        return <td class="px-4 py-2 text-sm">{props.children}</td>;
      },
      Break() {
        return <br />;
      },
    },
  };
}
