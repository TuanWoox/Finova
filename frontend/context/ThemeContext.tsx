import React, { createContext, useContext, useState, useCallback } from "react";
import { useColorScheme as useDeviceColorScheme } from "react-native";
import { Colors, type ThemeColors } from "@/constants";

type ThemePreference = "light" | "dark" | "system";

interface ThemeContextValue {
  colorScheme: "light" | "dark";
  colors: ThemeColors;
  isDark: boolean;
  themePreference: ThemePreference;
  setThemePreference: (pref: ThemePreference) => void;
}

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const deviceScheme = useDeviceColorScheme();
  const [themePreference, setThemePreference] =
    useState<ThemePreference>("system");

  const colorScheme =
    themePreference === "system"
      ? (deviceScheme ?? "light")
      : themePreference;

  const colors = Colors[colorScheme];
  const isDark = colorScheme === "dark";

  const handleSetThemePreference = useCallback((pref: ThemePreference) => {
    setThemePreference(pref);
  }, []);

  return (
    <ThemeContext.Provider
      value={{
        colorScheme,
        colors,
        isDark,
        themePreference,
        setThemePreference: handleSetThemePreference,
      }}
    >
      {children}
    </ThemeContext.Provider>
  );
}

export function useTheme(): ThemeContextValue {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error("useTheme must be used within a ThemeProvider");
  }
  return context;
}
