import React from "react";
import {
  TouchableOpacity,
  Text,
  ActivityIndicator,
  type TouchableOpacityProps,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { useTheme } from "@/hooks";

interface ButtonProps extends TouchableOpacityProps {
  title: string;
  variant?: "primary" | "secondary" | "outline" | "ghost";
  size?: "sm" | "md" | "lg";
  loading?: boolean;
  icon?: keyof typeof Ionicons.glyphMap;
  iconPosition?: "left" | "right";
}

export function Button({
  title,
  variant = "primary",
  size = "md",
  loading = false,
  disabled = false,
  icon,
  iconPosition = "left",
  style,
  ...props
}: ButtonProps) {
  const { colors } = useTheme();

  const sizeStyles = {
    sm: { paddingVertical: 8, paddingHorizontal: 16, fontSize: 13, iconSize: 16 },
    md: { paddingVertical: 12, paddingHorizontal: 20, fontSize: 15, iconSize: 18 },
    lg: { paddingVertical: 16, paddingHorizontal: 24, fontSize: 17, iconSize: 20 },
  };

  const variantStyles = {
    primary: {
      container: { backgroundColor: colors.primary },
      text: { color: "#ffffff" },
      iconColor: "#ffffff",
    },
    secondary: {
      container: { backgroundColor: colors.primaryLight },
      text: { color: colors.primary },
      iconColor: colors.primary,
    },
    outline: {
      container: { backgroundColor: "transparent", borderWidth: 1.5, borderColor: colors.border },
      text: { color: colors.text },
      iconColor: colors.text,
    },
    ghost: {
      container: { backgroundColor: "transparent" },
      text: { color: colors.primary },
      iconColor: colors.primary,
    },
  };

  const s = sizeStyles[size];
  const v = variantStyles[variant];

  return (
    <TouchableOpacity
      style={[
        {
          flexDirection: "row",
          alignItems: "center",
          justifyContent: "center",
          borderRadius: 12,
          paddingVertical: s.paddingVertical,
          paddingHorizontal: s.paddingHorizontal,
          gap: 8,
          minHeight: 48,
          opacity: disabled ? 0.5 : 1,
        },
        v.container,
        style,
      ]}
      disabled={disabled || loading}
      activeOpacity={0.7}
      {...props}
    >
      {loading ? (
        <ActivityIndicator color={v.iconColor} size="small" />
      ) : (
        <>
          {icon && iconPosition === "left" && (
            <Ionicons name={icon} size={s.iconSize} color={v.iconColor} />
          )}
          <Text
            style={[
              { fontSize: s.fontSize, fontWeight: "600" },
              v.text,
            ]}
          >
            {title}
          </Text>
          {icon && iconPosition === "right" && (
            <Ionicons name={icon} size={s.iconSize} color={v.iconColor} />
          )}
        </>
      )}
    </TouchableOpacity>
  );
}
