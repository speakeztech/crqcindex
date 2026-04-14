import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import solidMarkedPlugin from 'vite-plugin-solid-marked';

export default defineConfig({
  plugins: [
    solidMarkedPlugin({
      source: '@mdx',
      noDynamicComponents: true,
    }),
    solidPlugin(),
  ],
  build: {
    target: 'esnext',
    outDir: 'dist',
    rollupOptions: {
      input: {
        main: './index.html'
      }
    }
  },
  server: {
    port: 3000,
    open: true
  },
  resolve: {
    alias: {
      '@output': './output',
      '@mdx': '/output/src/MdxProvider.fs.jsx'
    }
  }
});
