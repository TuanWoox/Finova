import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: "class",
  content: [
    "./app/**/*.{ts,tsx}",
    "./components/**/*.{ts,tsx}",
    "./features/**/*.{ts,tsx}",
  ],
  presets: [require("nativewind/preset")],
  theme: {
    extend: {
      colors: {
        primary: {
          50: "#ecfdf5",
          100: "#d1fae5",
          200: "#a7f3d0",
          300: "#6ee7b7",
          400: "#34d399",
          500: "#10b981",
          600: "#059669",
          700: "#047857",
          800: "#065f46",
          900: "#064e3b",
        },
        surface: {
          light: "#ffffff",
          DEFAULT: "#f8fafc",
          dark: "#0f172a",
          "dark-card": "#1e293b",
          "dark-elevated": "#334155",
        },
        accent: {
          blue: "#3b82f6",
          amber: "#f59e0b",
          rose: "#f43f5e",
          violet: "#8b5cf6",
          cyan: "#06b6d4",
        },
        semantic: {
          income: "#10b981",
          expense: "#f43f5e",
          transfer: "#3b82f6",
          warning: "#f59e0b",
        },
      },
      fontFamily: {
        sans: ["Inter"],
        "sans-medium": ["Inter-Medium"],
        "sans-semibold": ["Inter-SemiBold"],
        "sans-bold": ["Inter-Bold"],
      },
      borderRadius: {
        "2xl": "16px",
        "3xl": "24px",
      },
    },
  },
  plugins: [],
};

export default config;
