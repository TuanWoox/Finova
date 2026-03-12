import type {
  Category,
  Account,
  Transaction,
  Budget,
  Group,
  GroupMember,
  AIMessage,
} from "@/types";

// ─── Categories ──────────────────────────────────────────
export const categories: Category[] = [
  { id: "cat-1", name: "Food & Dining", icon: "restaurant", color: "#f59e0b", type: "expense" },
  { id: "cat-2", name: "Transportation", icon: "car", color: "#3b82f6", type: "expense" },
  { id: "cat-3", name: "Shopping", icon: "cart", color: "#8b5cf6", type: "expense" },
  { id: "cat-4", name: "Entertainment", icon: "game-controller", color: "#f43f5e", type: "expense" },
  { id: "cat-5", name: "Bills & Utilities", icon: "flash", color: "#06b6d4", type: "expense" },
  { id: "cat-6", name: "Health", icon: "medkit", color: "#10b981", type: "expense" },
  { id: "cat-7", name: "Groceries", icon: "basket", color: "#84cc16", type: "expense" },
  { id: "cat-8", name: "Salary", icon: "wallet", color: "#10b981", type: "income" },
  { id: "cat-9", name: "Freelance", icon: "laptop", color: "#6366f1", type: "income" },
  { id: "cat-10", name: "Investment", icon: "trending-up", color: "#0ea5e9", type: "income" },
];

// ─── Accounts ────────────────────────────────────────────
export const accounts: Account[] = [
  { id: "acc-1", name: "Main Wallet", type: "cash", balance: 450.0, currency: "USD", icon: "wallet", color: "#10b981" },
  { id: "acc-2", name: "Bank Account", type: "bank", balance: 12580.5, currency: "USD", icon: "card", color: "#3b82f6" },
  { id: "acc-3", name: "Credit Card", type: "credit", balance: -1240.0, currency: "USD", icon: "card", color: "#f43f5e" },
  { id: "acc-4", name: "Savings", type: "savings", balance: 8500.0, currency: "USD", icon: "cash", color: "#f59e0b" },
];

// ─── Transactions ────────────────────────────────────────
export const transactions: Transaction[] = [
  { id: "tx-1", amount: 45.5, type: "expense", categoryId: "cat-1", accountId: "acc-1", note: "Lunch with team", date: "2026-03-12T12:30:00Z", createdAt: "2026-03-12T12:30:00Z" },
  { id: "tx-2", amount: 3500, type: "income", categoryId: "cat-8", accountId: "acc-2", note: "March salary", date: "2026-03-10T09:00:00Z", createdAt: "2026-03-10T09:00:00Z" },
  { id: "tx-3", amount: 120, type: "expense", categoryId: "cat-5", accountId: "acc-2", note: "Electricity bill", date: "2026-03-09T14:00:00Z", createdAt: "2026-03-09T14:00:00Z" },
  { id: "tx-4", amount: 89.99, type: "expense", categoryId: "cat-3", accountId: "acc-3", note: "New headphones", date: "2026-03-08T16:30:00Z", createdAt: "2026-03-08T16:30:00Z" },
  { id: "tx-5", amount: 15, type: "expense", categoryId: "cat-2", accountId: "acc-1", note: "Uber to office", date: "2026-03-08T08:15:00Z", createdAt: "2026-03-08T08:15:00Z" },
  { id: "tx-6", amount: 250, type: "income", categoryId: "cat-9", accountId: "acc-2", note: "Logo design project", date: "2026-03-07T11:00:00Z", createdAt: "2026-03-07T11:00:00Z" },
  { id: "tx-7", amount: 62.3, type: "expense", categoryId: "cat-7", accountId: "acc-1", note: "Weekly groceries", date: "2026-03-06T17:45:00Z", createdAt: "2026-03-06T17:45:00Z" },
  { id: "tx-8", amount: 35, type: "expense", categoryId: "cat-4", accountId: "acc-3", note: "Netflix & Spotify", date: "2026-03-05T10:00:00Z", createdAt: "2026-03-05T10:00:00Z" },
  { id: "tx-9", amount: 200, type: "expense", categoryId: "cat-6", accountId: "acc-2", note: "Dentist appointment", date: "2026-03-04T15:30:00Z", createdAt: "2026-03-04T15:30:00Z" },
  { id: "tx-10", amount: 500, type: "income", categoryId: "cat-10", accountId: "acc-4", note: "Dividend payout", date: "2026-03-03T09:00:00Z", createdAt: "2026-03-03T09:00:00Z" },
];

// ─── Budgets ─────────────────────────────────────────────
export const budgets: Budget[] = [
  { id: "bud-1", categoryId: "cat-1", amount: 400, spent: 285, period: "monthly", startDate: "2026-03-01", endDate: "2026-03-31" },
  { id: "bud-2", categoryId: "cat-2", amount: 200, spent: 145, period: "monthly", startDate: "2026-03-01", endDate: "2026-03-31" },
  { id: "bud-3", categoryId: "cat-3", amount: 300, spent: 89.99, period: "monthly", startDate: "2026-03-01", endDate: "2026-03-31" },
  { id: "bud-4", categoryId: "cat-5", amount: 250, spent: 120, period: "monthly", startDate: "2026-03-01", endDate: "2026-03-31" },
  { id: "bud-5", categoryId: "cat-7", amount: 350, spent: 312, period: "monthly", startDate: "2026-03-01", endDate: "2026-03-31" },
];

// ─── Group Members ───────────────────────────────────────
const members: GroupMember[] = [
  { id: "mem-1", name: "You", avatar: undefined },
  { id: "mem-2", name: "Sarah Chen", avatar: undefined },
  { id: "mem-3", name: "Mike Johnson", avatar: undefined },
  { id: "mem-4", name: "Emma Wilson", avatar: undefined },
  { id: "mem-5", name: "Alex Turner", avatar: undefined },
];

// ─── Groups ──────────────────────────────────────────────
export const groups: Group[] = [
  {
    id: "grp-1",
    name: "Bali Trip 2026",
    description: "Spring break vacation",
    icon: "airplane",
    members: [members[0], members[1], members[2], members[3]],
    expenses: [
      { id: "gx-1", groupId: "grp-1", description: "Hotel booking", amount: 800, paidById: "mem-1", splitType: "equal", splits: [{ memberId: "mem-1", amount: 200 }, { memberId: "mem-2", amount: 200 }, { memberId: "mem-3", amount: 200 }, { memberId: "mem-4", amount: 200 }], date: "2026-03-10" },
      { id: "gx-2", groupId: "grp-1", description: "Dinner at beach club", amount: 240, paidById: "mem-2", splitType: "equal", splits: [{ memberId: "mem-1", amount: 60 }, { memberId: "mem-2", amount: 60 }, { memberId: "mem-3", amount: 60 }, { memberId: "mem-4", amount: 60 }], date: "2026-03-11" },
    ],
    createdAt: "2026-02-15",
  },
  {
    id: "grp-2",
    name: "Apartment Shared",
    description: "Monthly apartment expenses",
    icon: "home",
    members: [members[0], members[1], members[4]],
    expenses: [
      { id: "gx-3", groupId: "grp-2", description: "March rent", amount: 2400, paidById: "mem-1", splitType: "equal", splits: [{ memberId: "mem-1", amount: 800 }, { memberId: "mem-2", amount: 800 }, { memberId: "mem-5", amount: 800 }], date: "2026-03-01" },
      { id: "gx-4", groupId: "grp-2", description: "Internet bill", amount: 90, paidById: "mem-5", splitType: "equal", splits: [{ memberId: "mem-1", amount: 30 }, { memberId: "mem-2", amount: 30 }, { memberId: "mem-5", amount: 30 }], date: "2026-03-05" },
    ],
    createdAt: "2025-09-01",
  },
];

// ─── AI Messages ─────────────────────────────────────────
export const aiMessages: AIMessage[] = [
  { id: "ai-1", role: "assistant", content: "Hi! I'm your Finova AI assistant. I can help you understand your spending patterns, plan budgets, and answer financial questions. What would you like to know?", timestamp: "2026-03-12T10:00:00Z" },
];

// ─── AI Prompt Suggestions ──────────────────────────────
export const aiSuggestions = [
  "Can I afford a vacation?",
  "Where does my money go?",
  "How can I save more?",
  "Monthly spending insight",
  "Am I over budget?",
  "Compare this month vs last",
];

// ─── Helpers ─────────────────────────────────────────────
export function getCategoryById(id: string): Category | undefined {
  return categories.find((c) => c.id === id);
}

export function getAccountById(id: string): Account | undefined {
  return accounts.find((a) => a.id === id);
}

export function getTotalBalance(): number {
  return accounts.reduce((sum, acc) => sum + acc.balance, 0);
}

export function getMonthlyIncome(): number {
  return transactions
    .filter((t) => t.type === "income")
    .reduce((sum, t) => sum + t.amount, 0);
}

export function getMonthlyExpense(): number {
  return transactions
    .filter((t) => t.type === "expense")
    .reduce((sum, t) => sum + t.amount, 0);
}
