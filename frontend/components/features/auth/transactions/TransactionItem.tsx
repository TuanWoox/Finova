import React from "react";
import { View, Text, TouchableOpacity } from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { useTheme } from "@/hooks";
import { Card } from "@/components/ui";
import type { Transaction, Category } from "@/types";
import { formatCurrency, formatDate } from "@/lib";

interface TransactionItemProps {
  transaction: Transaction;
  category?: Category;
  onPress?: () => void;
}

export function TransactionItem({
  transaction,
  category,
  onPress,
}: TransactionItemProps) {
  const { colors } = useTheme();
  const isIncome = transaction.type === "income";

  return (
    <TouchableOpacity
      onPress={onPress}
      activeOpacity={0.7}
      style={{ minHeight: 48 }}
      accessibilityLabel={`${transaction.note} ${formatCurrency(transaction.amount)}`}
    >
      <View
        style={{
          flexDirection: "row",
          alignItems: "center",
          paddingVertical: 12,
          paddingHorizontal: 4,
          gap: 12,
        }}
      >
        <View
          style={{
            width: 44,
            height: 44,
            borderRadius: 12,
            backgroundColor: category?.color
              ? `${category.color}20`
              : colors.surfaceElevated,
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Ionicons
            name={(category?.icon as keyof typeof Ionicons.glyphMap) ?? "cash"}
            size={22}
            color={category?.color ?? colors.icon}
          />
        </View>

        <View style={{ flex: 1 }}>
          <Text
            style={{ fontSize: 15, fontWeight: "500", color: colors.text }}
            numberOfLines={1}
          >
            {transaction.note || category?.name || "Transaction"}
          </Text>
          <Text
            style={{
              fontSize: 13,
              color: colors.textSecondary,
              marginTop: 2,
            }}
          >
            {category?.name} · {formatDate(transaction.date)}
          </Text>
        </View>

        <Text
          style={{
            fontSize: 15,
            fontWeight: "600",
            color: isIncome ? colors.income : colors.expense,
          }}
        >
          {isIncome ? "+" : "-"}{formatCurrency(transaction.amount)}
        </Text>
      </View>
    </TouchableOpacity>
  );
}
