import React from "react";
import { View, Text, Image } from "react-native";
import { useTheme } from "@/hooks";
import { getInitials } from "@/lib";

interface AvatarProps {
  name: string;
  imageUrl?: string;
  size?: "sm" | "md" | "lg";
  color?: string;
}

const sizeMap = { sm: 32, md: 40, lg: 56 };
const fontSizeMap = { sm: 12, md: 14, lg: 20 };

export function Avatar({
  name,
  imageUrl,
  size = "md",
  color,
}: AvatarProps) {
  const { colors } = useTheme();
  const dim = sizeMap[size];

  if (imageUrl) {
    return (
      <Image
        source={{ uri: imageUrl }}
        style={{
          width: dim,
          height: dim,
          borderRadius: dim / 2,
          backgroundColor: colors.surfaceElevated,
        }}
      />
    );
  }

  return (
    <View
      style={{
        width: dim,
        height: dim,
        borderRadius: dim / 2,
        backgroundColor: color ?? colors.primaryLight,
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Text
        style={{
          fontSize: fontSizeMap[size],
          fontWeight: "600",
          color: color ? "#ffffff" : colors.primary,
        }}
      >
        {getInitials(name)}
      </Text>
    </View>
  );
}
