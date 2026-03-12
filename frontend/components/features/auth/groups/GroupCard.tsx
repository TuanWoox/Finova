import React from "react";
import { View, Text, TouchableOpacity } from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { useTheme } from "@/hooks";
import { Avatar } from "@/components/ui";
import type { Group } from "@/types";
import { formatCurrency } from "@/lib";

interface GroupCardProps {
  group: Group;
  onPress?: () => void;
}

export function GroupCard({ group, onPress }: GroupCardProps) {
  const { colors } = useTheme();

  const totalExpenses = group.expenses.reduce(
    (sum, e) => sum + e.amount,
    0
  );

  return (
    <TouchableOpacity
      onPress={onPress}
      activeOpacity={0.7}
      style={{
        backgroundColor: colors.card,
        borderRadius: 16,
        padding: 16,
        shadowColor: colors.cardShadow,
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 1,
        shadowRadius: 8,
        elevation: 2,
        gap: 12,
      }}
      accessibilityLabel={`Group ${group.name}`}
    >
      <View style={{ flexDirection: "row", alignItems: "center", gap: 12 }}>
        <View
          style={{
            width: 44,
            height: 44,
            borderRadius: 12,
            backgroundColor: colors.primaryLight,
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Ionicons
            name={group.icon as keyof typeof Ionicons.glyphMap}
            size={22}
            color={colors.primary}
          />
        </View>
        <View style={{ flex: 1 }}>
          <Text
            style={{ fontSize: 16, fontWeight: "600", color: colors.text }}
            numberOfLines={1}
          >
            {group.name}
          </Text>
          <Text
            style={{ fontSize: 13, color: colors.textSecondary, marginTop: 2 }}
          >
            {group.members.length} members · {group.expenses.length} expenses
          </Text>
        </View>
        <Ionicons name="chevron-forward" size={20} color={colors.textTertiary} />
      </View>

      <View
        style={{
          flexDirection: "row",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <View style={{ flexDirection: "row", marginLeft: 4 }}>
          {group.members.slice(0, 4).map((member, index) => (
            <View
              key={member.id}
              style={{ marginLeft: index > 0 ? -8 : 0, zIndex: 4 - index }}
            >
              <Avatar name={member.name} size="sm" />
            </View>
          ))}
          {group.members.length > 4 && (
            <View
              style={{
                marginLeft: -8,
                width: 32,
                height: 32,
                borderRadius: 16,
                backgroundColor: colors.surfaceElevated,
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Text
                style={{ fontSize: 11, fontWeight: "600", color: colors.textSecondary }}
              >
                +{group.members.length - 4}
              </Text>
            </View>
          )}
        </View>

        <Text
          style={{ fontSize: 14, fontWeight: "600", color: colors.text }}
        >
          {formatCurrency(totalExpenses)}
        </Text>
      </View>
    </TouchableOpacity>
  );
}
