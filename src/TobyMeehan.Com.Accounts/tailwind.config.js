/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
      "Pages/**/*.cshtml"
  ],
  theme: {
    colors: {
      transparent: 'transparent',
      current: 'currentColor',
      
      'deep': '#030712',
      'dark': '#111827',
      'base': '#1f2937',
      'content': '#2b3440',
      'flash': '#374151',
      
      'light': {
        DEFAULT: '#d1d5db',
        hover: '#6b7280',
        focus: '#4b5563'
      },
      
      'primary': {
        DEFAULT: '#2b3440',
        hover: '#262e38',
        focus: '#262e38'
      },
      
      'link': {
        DEFAULT: '#0284c7',
        hover: '#0369a1'
      },
      
      'positive': '#16a34a',
      'negative': '#dc2626',
      'neutral': '#ca8a04'
    },
    extend: {},
  },
  plugins: [],
}

