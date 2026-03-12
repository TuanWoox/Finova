import React from "react";
import { View, Text } from "react-native";
import { useTheme } from "@/hooks";

interface ProgressBarProps {
  progress: number; // 0-100
  color?: string;
  height?: number;
  showLabel?: boolean;
  label?: string;
}

export function ProgressBar({
  progress,
  color,
  height = 8,
  showLabel = false,
  label,
}: ProgressBarProps) {
  const { colors } = useTheme();
  const clampedProgress = Math.min(Math.max(progress, 0), 100);
  const barColor =
    color ?? (clampedProgress > 90 ? colors.expense : colors.primary);

  return (
    <View style={{ gap: 6 }}>
      {showLabel && (
        <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
          {label && (
            <Text
              style={{ fontSize: 13, color: colors.textSecondary }}
            >
              {label}
            </Text>
          )}
          <Text
            style={{
              fontSize: 13,
              fontWeight: "600",
              color: clampedProgress > 90 ? colors.expense : colors.text,
            }}
          >
            {clampedProgress}%
          </Text>
        </View>
      )}
      <View
        style={{
          height,
          backgroundColor: colors.surfaceElevated,
          borderRadius: height / 2,
          overflow: "hidden",
        }}
      >
        <View
          style={{
            width: `${clampedProgress}%`,
            height: "100%",
            backgroundColor: barColor,
            borderRadius: height / 2,
          }}
        />
      </View>
    </View>
  );
}
