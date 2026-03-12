import React from "react";
import { View, TextInput, Text, type TextInputProps } from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { useTheme } from "@/hooks";

interface InputProps extends TextInputProps {
  label?: string;
  error?: string;
  leftIcon?: keyof typeof Ionicons.glyphMap;
  rightIcon?: keyof typeof Ionicons.glyphMap;
  onRightIconPress?: () => void;
}

export function Input({
  label,
  error,
  leftIcon,
  rightIcon,
  onRightIconPress,
  style,
  ...props
}: InputProps) {
  const { colors } = useTheme();

  return (
    <View style={{ gap: 6 }}>
      {label && (
        <Text
          style={{
            fontSize: 13,
            fontWeight: "500",
            color: colors.textSecondary,
            marginLeft: 4,
          }}
        >
          {label}
        </Text>
      )}
      <View
        style={{
          flexDirection: "row",
          alignItems: "center",
          backgroundColor: colors.surfaceElevated,
          borderRadius: 12,
          borderWidth: error ? 1.5 : 1,
          borderColor: error ? colors.expense : colors.border,
          paddingHorizontal: 14,
          minHeight: 48,
          gap: 10,
        }}
      >
        {leftIcon && (
          <Ionicons name={leftIcon} size={20} color={colors.icon} />
        )}
        <TextInput
          style={[
            {
              flex: 1,
              fontSize: 15,
              color: colors.text,
              paddingVertical: 12,
            },
            style,
          ]}
          placeholderTextColor={colors.textTertiary}
          {...props}
        />
        {rightIcon && (
          <Ionicons
            name={rightIcon}
            size={20}
            color={colors.icon}
            onPress={onRightIconPress}
          />
        )}
      </View>
      {error && (
        <Text
          style={{
            fontSize: 12,
            color: colors.expense,
            marginLeft: 4,
          }}
        >
          {error}
        </Text>
      )}
    </View>
  );
}
