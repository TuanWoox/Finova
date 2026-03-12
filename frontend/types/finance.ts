export type TransactionType = "income" | "expense" | "transfer";

export interface Category {
  id: string;
  name: string;
  icon: string;
  color: string;
  type: TransactionType;
}

export interface Account {
  id: string;
  name: string;
  type: "cash" | "bank" | "credit" | "savings" | "investment";
  balance: number;
  currency: string;
  icon: string;
  color: string;
}

export interface Transaction {
  id: string;
  amount: number;
  type: TransactionType;
  categoryId: string;
  accountId: string;
  note: string;
  date: string;
  createdAt: string;
}

export interface Budget {
  id: string;
  categoryId: string;
  amount: number;
  spent: number;
  period: "weekly" | "monthly" | "yearly";
  startDate: string;
  endDate: string;
}

export interface GroupMember {
  id: string;
  name: string;
  avatar?: string;
  email?: string;
}

export interface GroupExpense {
  id: string;
  groupId: string;
  description: string;
  amount: number;
  paidById: string;
  splitType: "equal" | "exact" | "percentage";
  splits: { memberId: string; amount: number }[];
  date: string;
  categoryId?: string;
}

export interface Group {
  id: string;
  name: string;
  description?: string;
  icon: string;
  members: GroupMember[];
  expenses: GroupExpense[];
  createdAt: string;
}

export interface Debt {
  from: GroupMember;
  to: GroupMember;
  amount: number;
}

export interface AIMessage {
  id: string;
  role: "user" | "assistant";
  content: string;
  timestamp: string;
}
