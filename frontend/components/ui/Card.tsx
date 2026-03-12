import React from "react";
import { View, type ViewProps } from "react-native";
import { useTheme } from "@/hooks";

interface CardProps extends ViewProps {
  variant?: "default" | "elevated" | "outlined";
  padding?: "none" | "sm" | "md" | "lg";
}

export function Card({
  children,
  variant = "default",
  padding = "md",
  style,
  ...props
}: CardProps) {
  const { colors } = useTheme();

  const paddingMap = { none: 0, sm: 8, md: 16, lg: 24 };

  const baseStyle = {
    borderRadius: 16,
    padding: paddingMap[padding],
    backgroundColor: colors.card,
  };

  const variantStyles = {
    default: {
      shadowColor: colors.cardShadow,
      shadowOffset: { width: 0, height: 2 },
      shadowOpacity: 1,
      shadowRadius: 8,
      elevation: 2,
    },
    elevated: {
      shadowColor: colors.cardShadow,
      shadowOffset: { width: 0, height: 4 },
      shadowOpacity: 1,
      shadowRadius: 16,
      elevation: 4,
    },
    outlined: {
      borderWidth: 1,
      borderColor: colors.border,
    },
  };

  return (
    <View style={[baseStyle, variantStyles[variant], style]} {...props}>
      {children}
    </View>
  );
}
