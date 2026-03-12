import React from "react";
import { View, Text, ScrollView, TouchableOpacity } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { Ionicons } from "@expo/vector-icons";
import { useTheme } from "@/hooks";
import { Card, ProgressBar } from "@/components/ui";
import {
  budgets,
  getCategoryById,
  getMonthlyExpense,
  getMonthlyIncome,
  getTotalBalance,
  transactions
} from "@/data/mock";
import { calculatePercentage, formatCurrency, formatDate } from "@/lib";

export default function HomeScreen() {
  const { colors } = useTheme();

  const totalBalance = getTotalBalance();
  const monthlyIncome = getMonthlyIncome();
  const monthlyExpense = getMonthlyExpense();
  const recentTransactions = transactions.slice(0, 5);

  const quickActions = [
    { icon: "add-circle" as const, label: "Add", color: colors.primary },
    { icon: "wallet" as const, label: "Accounts", color: colors.transfer },
    { icon: "pie-chart" as const, label: "Budget", color: colors.warning },
    { icon: "people" as const, label: "Groups", color: "#8b5cf6" },
  ];

  return (
    <SafeAreaView style={{ flex: 1, backgroundColor: colors.background }}>
      <ScrollView
        style={{ flex: 1 }}
        contentContainerStyle={{ paddingBottom: 24 }}
        showsVerticalScrollIndicator={false}
      >
        {/* Header */}
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-between",
            alignItems: "center",
            paddingHorizontal: 20,
            paddingTop: 8,
            paddingBottom: 16,
          }}
        >
          <View>
            <Text style={{ fontSize: 14, color: colors.textSecondary }}>
              Good evening 👋
            </Text>
            <Text
              style={{ fontSize: 24, fontWeight: "700", color: colors.text, marginTop: 2 }}
            >
              Dashboard
            </Text>
          </View>
          <TouchableOpacity
            style={{
              width: 44,
              height: 44,
              borderRadius: 22,
              backgroundColor: colors.surfaceElevated,
              alignItems: "center",
              justifyContent: "center",
            }}
            accessibilityLabel="Notifications"
          >
            <Ionicons name="notifications-outline" size={22} color={colors.icon} />
          </TouchableOpacity>
        </View>

        {/* Balance Card */}
        <View style={{ paddingHorizontal: 20, marginBottom: 20 }}>
          <Card
            variant="elevated"
            style={{
              backgroundColor: colors.primary,
              padding: 24,
            }}
          >
            <Text style={{ fontSize: 14, color: "rgba(255,255,255,0.8)" }}>
              Total Balance
            </Text>
            <Text
              style={{
                fontSize: 36,
                fontWeight: "700",
                color: "#ffffff",
                marginTop: 4,
              }}
            >
              {formatCurrency(totalBalance)}
            </Text>
            <View
              style={{
                flexDirection: "row",
                marginTop: 20,
                gap: 24,
              }}
            >
              <View style={{ flex: 1 }}>
                <View style={{ flexDirection: "row", alignItems: "center", gap: 6 }}>
                  <View
                    style={{
                      width: 28,
                      height: 28,
                      borderRadius: 14,
                      backgroundColor: "rgba(255,255,255,0.2)",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <Ionicons name="arrow-down" size={14} color="#ffffff" />
                  </View>
                  <Text style={{ fontSize: 12, color: "rgba(255,255,255,0.7)" }}>
                    Income
                  </Text>
                </View>
                <Text
                  style={{
                    fontSize: 18,
                    fontWeight: "600",
                    color: "#ffffff",
                    marginTop: 4,
                  }}
                >
                  {formatCurrency(monthlyIncome)}
                </Text>
              </View>
              <View style={{ flex: 1 }}>
                <View style={{ flexDirection: "row", alignItems: "center", gap: 6 }}>
                  <View
                    style={{
                      width: 28,
                      height: 28,
                      borderRadius: 14,
                      backgroundColor: "rgba(255,255,255,0.2)",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <Ionicons name="arrow-up" size={14} color="#ffffff" />
                  </View>
                  <Text style={{ fontSize: 12, color: "rgba(255,255,255,0.7)" }}>
                    Expense
                  </Text>
                </View>
                <Text
                  style={{
                    fontSize: 18,
                    fontWeight: "600",
                    color: "#ffffff",
                    marginTop: 4,
                  }}
                >
                  {formatCurrency(monthlyExpense)}
                </Text>
              </View>
            </View>
          </Card>
        </View>

        {/* Quick Actions */}
        <View
          style={{
            flexDirection: "row",
            paddingHorizontal: 20,
            marginBottom: 24,
            gap: 12,
          }}
        >
          {quickActions.map((action) => (
            <TouchableOpacity
              key={action.label}
              style={{
                flex: 1,
                alignItems: "center",
                gap: 8,
                paddingVertical: 12,
                backgroundColor: colors.card,
                borderRadius: 14,
                shadowColor: colors.cardShadow,
                shadowOffset: { width: 0, height: 1 },
                shadowOpacity: 1,
                shadowRadius: 4,
                elevation: 1,
              }}
              accessibilityLabel={action.label}
            >
              <View
                style={{
                  width: 44,
                  height: 44,
                  borderRadius: 12,
                  backgroundColor: `${action.color}18`,
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <Ionicons name={action.icon} size={24} color={action.color} />
              </View>
              <Text
                style={{
                  fontSize: 12,
                  fontWeight: "500",
                  color: colors.textSecondary,
                }}
              >
                {action.label}
              </Text>
            </TouchableOpacity>
          ))}
        </View>

        {/* Budget Overview */}
        <View style={{ paddingHorizontal: 20, marginBottom: 24 }}>
          <View
            style={{
              flexDirection: "row",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: 12,
            }}
          >
            <Text style={{ fontSize: 18, fontWeight: "600", color: colors.text }}>
              Budget
            </Text>
          </View>
          <Card padding="md">
            <View style={{ gap: 16 }}>
              {budgets.slice(0, 3).map((budget) => {
                const cat = getCategoryById(budget.categoryId);
                const pct = calculatePercentage(budget.spent, budget.amount);
                return (
                  <View key={budget.id} style={{ gap: 4 }}>
                    <View
                      style={{
                        flexDirection: "row",
                        justifyContent: "space-between",
                      }}
                    >
                      <Text
                        style={{
                          fontSize: 14,
                          fontWeight: "500",
                          color: colors.text,
                        }}
                      >
                        {cat?.name}
                      </Text>
                      <Text
                        style={{
                          fontSize: 13,
                          color: colors.textSecondary,
                        }}
                      >
                        {formatCurrency(budget.spent)} / {formatCurrency(budget.amount)}
                      </Text>
                    </View>
                    <ProgressBar
                      progress={pct}
                      color={cat?.color}
                      height={6}
                    />
                  </View>
                );
              })}
            </View>
          </Card>
        </View>

        {/* Recent Transactions */}
        <View style={{ paddingHorizontal: 20 }}>
          <View
            style={{
              flexDirection: "row",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: 8,
            }}
          >
            <Text style={{ fontSize: 18, fontWeight: "600", color: colors.text }}>
              Recent Transactions
            </Text>
          </View>
          <Card padding="none">
            {recentTransactions.map((tx, index) => {
              const cat = getCategoryById(tx.categoryId);
              return (
                <View
                  key={tx.id}
                  style={{
                    flexDirection: "row",
                    alignItems: "center",
                    gap: 12,
                    padding: 16,
                    borderBottomWidth: index < recentTransactions.length - 1 ? 0.5 : 0,
                    borderBottomColor: colors.borderLight,
                  }}
                >
                  <View
                    style={{
                      width: 44,
                      height: 44,
                      borderRadius: 12,
                      backgroundColor: `${cat?.color}20`,
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <Ionicons
                      name={(cat?.icon as keyof typeof Ionicons.glyphMap) ?? "cash"}
                      size={24}
                      color={cat?.color}
                    />
                  </View>
                  <View style={{ flex: 1 }}>
                    <Text
                      style={{
                        fontSize: 16,
                        fontWeight: "600",
                        color: colors.text,
                      }}
                      numberOfLines={1}
                    >
                      {cat?.name}
                    </Text>
                    <Text
                      style={{
                        fontSize: 13,
                        color: colors.textSecondary,
                        marginTop: 2,
                      }}
                      numberOfLines={1}
                    >
                      {tx.note || formatDate(tx.date)}
                    </Text>
                  </View>
                  <Text
                    style={{
                      fontSize: 16,
                      fontWeight: "600",
                      color: tx.type === "expense" ? colors.text : colors.income,
                    }}
                  >
                    {tx.type === "expense" ? "-" : "+"}
                    {formatCurrency(tx.amount)}
                  </Text>
                </View>
              );
            })}
          </Card>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}
