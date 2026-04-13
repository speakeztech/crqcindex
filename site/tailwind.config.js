/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./output/**/*.{js,jsx}",
    "./src/**/*.{fs,jsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: ['Montserrat', 'system-ui', 'sans-serif'],
        heading: ['Nunito', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'Consolas', 'Monaco', 'monospace'],
      },
      colors: {
        // SpeakEZ brand colors
        speakez: {
          teal: '#469c95',
          'teal-dark': '#007067',
          blue: '#0065b2',
          'blue-light': '#3b8ed0',
          orange: '#f58220',
          'orange-light': '#ff9a47',
          neutral: '#323232',
          'neutral-dark': '#1a1a1a',
          'neutral-light': '#eaeaea',
        },
        // CRQC Index accent colors
        crqc: {
          indigo: '#4f46e5',
          'indigo-light': '#818cf8',
          'indigo-dark': '#3730a3',
          slate: '#475569',
          'slate-light': '#94a3b8',
          warning: '#f59e0b',
          critical: '#ef4444',
          safe: '#22c55e',
        },
      },
      animation: {
        'pulse-slow': 'pulse 3s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },
    },
  },
  plugins: [],
}
