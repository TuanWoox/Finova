import React from "react";
import { View, Text, ScrollView, TouchableOpacity } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { Ionicons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import { useTheme } from "@/hooks";
import { Card, Avatar } from "@/components/ui";

type ThemeOption = "light" | "dark" | "system";

const settingsSections = [
  {
    title: "General",
    items: [
      { icon: "wallet", label: "Accounts", route: null },
      { icon: "pricetags", label: "Categories", route: null },
      { icon: "pie-chart", label: "Budgets", route: null },
    ],
  },
  {
    title: "Preferences",
    items: [
      { icon: "notifications", label: "Notifications", route: null },
      { icon: "globe", label: "Currency", route: null },
      { icon: "language", label: "Language", route: null },
    ],
  },
  {
    title: "Data",
    items: [
      { icon: "download", label: "Export Data", route: null },
      { icon: "cloud-upload", label: "Backup", route: null },
    ],
  },
  {
    title: "About",
    items: [
      { icon: "help-circle", label: "Help & Support", route: null },
      { icon: "document-text", label: "Privacy Policy", route: null },
      { icon: "information-circle", label: "About Finova", route: null },
    ],
  },
] as const;

export default function ProfileScreen() {
  const { colors, themePreference, setThemePreference, isDark } = useTheme();
  const router = useRouter();

  const themeOptions: { key: ThemeOption; icon: keyof typeof Ionicons.glyphMap; label: string }[] = [
    { key: "light", icon: "sunny", label: "Light" },
    { key: "dark", icon: "moon", label: "Dark" },
    { key: "system", icon: "phone-portrait", label: "Auto" },
  ];

  return (
    <SafeAreaView style={{ flex: 1, backgroundColor: colors.background }}>
      <ScrollView
        style={{ flex: 1 }}
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}
      >
        {/* Header */}
        <View
          style={{
            paddingHorizontal: 20,
            paddingTop: 8,
            paddingBottom: 24,
          }}
        >
          <Text style={{ fontSize: 24, fontWeight: "700", color: colors.text }}>
            Profile
          </Text>
        </View>

        {/* Profile Card */}
        <View style={{ paddingHorizontal: 20, marginBottom: 24 }}>
          <Card variant="elevated" padding="lg">
            <View
              style={{
                flexDirection: "row",
                alignItems: "center",
                gap: 16,
              }}
            >
              <Avatar name="John Doe" size="lg" />
              <View style={{ flex: 1 }}>
                <Text
                  style={{
                    fontSize: 20,
                    fontWeight: "700",
                    color: colors.text,
                  }}
                >
                  John Doe
                </Text>
                <Text
                  style={{
                    fontSize: 14,
                    color: colors.textSecondary,
                    marginTop: 2,
                  }}
                >
                  john.doe@email.com
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
                accessibilityLabel="Edit profile"
              >
                <Ionicons name="create-outline" size={20} color={colors.icon} />
              </TouchableOpacity>
            </View>
          </Card>
        </View>

        {/* Theme Selector */}
        <View style={{ paddingHorizontal: 20, marginBottom: 24 }}>
          <Text
            style={{
              fontSize: 14,
              fontWeight: "600",
              color: colors.textSecondary,
              marginBottom: 10,
              marginLeft: 4,
            }}
          >
            APPEARANCE
          </Text>
          <Card padding="sm">
            <View
              style={{
                flexDirection: "row",
                gap: 8,
                padding: 4,
              }}
            >
              {themeOptions.map((option) => {
                const isActive = themePreference === option.key;
                return (
                  <TouchableOpacity
                    key={option.key}
                    onPress={() => setThemePreference(option.key)}
                    style={{
                      flex: 1,
                      flexDirection: "row",
                      alignItems: "center",
                      justifyContent: "center",
                      gap: 6,
                      paddingVertical: 12,
                      borderRadius: 10,
                      backgroundColor: isActive
                        ? colors.primary
                        : "transparent",
                      minHeight: 48,
                    }}
                    accessibilityLabel={`${option.label} theme`}
                  >
                    <Ionicons
                      name={option.icon}
                      size={18}
                      color={isActive ? "#ffffff" : colors.textSecondary}
                    />
                    <Text
                      style={{
                        fontSize: 14,
                        fontWeight: "600",
                        color: isActive ? "#ffffff" : colors.textSecondary,
                      }}
                    >
                      {option.label}
                    </Text>
                  </TouchableOpacity>
                );
              })}
            </View>
          </Card>
        </View>

        {/* Settings Sections */}
        {settingsSections.map((section) => (
          <View key={section.title} style={{ paddingHorizontal: 20, marginBottom: 24 }}>
            <Text
              style={{
                fontSize: 14,
                fontWeight: "600",
                color: colors.textSecondary,
                marginBottom: 10,
                marginLeft: 4,
              }}
            >
              {section.title.toUpperCase()}
            </Text>
            <Card padding="none">
              {section.items.map((item, index) => (
                <TouchableOpacity
                  key={item.label}
                  onPress={() => {
                    if (item.route) router.push(item.route as any);
                  }}
                  style={{
                    flexDirection: "row",
                    alignItems: "center",
                    paddingHorizontal: 16,
                    paddingVertical: 14,
                    gap: 14,
                    minHeight: 48,
                    borderBottomWidth:
                      index < section.items.length - 1 ? 0.5 : 0,
                    borderBottomColor: colors.borderLight,
                  }}
                  accessibilityLabel={item.label}
                >
                  <Ionicons
                    name={item.icon as keyof typeof Ionicons.glyphMap}
                    size={20}
                    color={colors.icon}
                  />
                  <Text
                    style={{
                      flex: 1,
                      fontSize: 15,
                      color: colors.text,
                    }}
                  >
                    {item.label}
                  </Text>
                  <Ionicons
                    name="chevron-forward"
                    size={18}
                    color={colors.textTertiary}
                  />
                </TouchableOpacity>
              ))}
            </Card>
          </View>
        ))}

        {/* Logout */}
        <View style={{ paddingHorizontal: 20 }}>
          <TouchableOpacity
            style={{
              flexDirection: "row",
              alignItems: "center",
              justifyContent: "center",
              gap: 8,
              paddingVertical: 14,
              borderRadius: 14,
              backgroundColor: `${colors.expense}15`,
              minHeight: 48,
            }}
            accessibilityLabel="Log out"
          >
            <Ionicons name="log-out-outline" size={20} color={colors.expense} />
            <Text
              style={{
                fontSize: 15,
                fontWeight: "600",
                color: colors.expense,
              }}
            >
              Log Out
            </Text>
          </TouchableOpacity>
        </View>

        {/* Version */}
        <Text
          style={{
            textAlign: "center",
            fontSize: 12,
            color: colors.textTertiary,
            marginTop: 16,
          }}
        >
          Finova v1.0.0
        </Text>
      </ScrollView>
    </SafeAreaView>
  );
}
