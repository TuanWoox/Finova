import { Dimensions } from "react-native";

const { width, height } = Dimensions.get("window");

export const Layout = {
  window: { width, height },
  isSmallDevice: width < 375,

  spacing: {
    xs: 4,
    sm: 8,
    md: 12,
    lg: 16,
    xl: 20,
    "2xl": 24,
    "3xl": 32,
    "4xl": 40,
    "5xl": 48,
  },

  borderRadius: {
    sm: 6,
    md: 10,
    lg: 14,
    xl: 18,
    "2xl": 24,
    full: 9999,
  },

  fontSize: {
    xs: 11,
    sm: 13,
    md: 15,
    lg: 17,
    xl: 20,
    "2xl": 24,
    "3xl": 30,
    "4xl": 36,
  },

  iconSize: {
    sm: 16,
    md: 20,
    lg: 24,
    xl: 28,
    "2xl": 32,
  },

  touchTarget: {
    min: 48,
  },
} as const;
