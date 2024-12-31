import type { Config } from "tailwindcss";

export default {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/features/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    container: {
      center: true,
      padding: {
        DEFAULT: '1rem',
      }
    },
    extend: {
      colors: {
        primary: {
          50: '#ebf5ff',
          100: '#d6eaff',
          200: '#add6ff',
          300: '#84c1ff',
          400: '#5badff',
          500: '#2b96ff',
          600: '#007bff', // Primary DEFAULT
          700: '#0066cc',
          800: '#004d99',
          900: '#003366',
          DEFAULT: '#007bff',
        },
        secondary: {
          50: '#e6f9fb',
          100: '#ccf3f6',
          200: '#99e6ec',
          300: '#66dae3',
          400: '#33cdd9',
          500: '#00c1d4', // Secondary DEFAULT
          600: '#00a7b9',
          700: '#008d9e',
          800: '#007482',
          900: '#005a67',
          DEFAULT: '#00c1d4',
        },
      },
    }

  },
  plugins: [],
} satisfies Config;
