import React from "react";
import { View, Text } from "react-native";

type BadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

interface BadgeProps {
  label: string;
  variant?: BadgeVariant;
}

const variantStyles: Record<BadgeVariant, { bg: string; text: string }> = {
  success: { bg: "#d1fae5", text: "#059669" },
  warning: { bg: "#fef3c7", text: "#d97706" },
  danger: { bg: "#ffe4e6", text: "#e11d48" },
  info: { bg: "#dbeafe", text: "#2563eb" },
  neutral: { bg: "#f1f5f9", text: "#64748b" },
};

export function Badge({ label, variant = "neutral" }: BadgeProps) {
  const colors = variantStyles[variant];

  return (
    <View
      style={{
        backgroundColor: colors.bg,
        paddingHorizontal: 10,
        paddingVertical: 4,
        borderRadius: 20,
        alignSelf: "flex-start",
      }}
    >
      <Text
        style={{
          fontSize: 12,
          fontWeight: "600",
          color: colors.text,
        }}
      >
        {label}
      </Text>
    </View>
  );
}
