import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type { ReminderListQuery, ReminderResponse } from "@/types/reminder";

export const remindersApi = {
  list: (q: ReminderListQuery = {}) =>
    apiGet<PagedResult<ReminderResponse>>("/reminders", q as Record<string, unknown>),
  unread: () => apiGet<ReminderResponse[]>("/reminders/unread"),
  markRead: (id: number) => apiPut<ReminderResponse>(`/reminders/${id}/read`),
  dismiss: (id: number) => apiPut<ReminderResponse>(`/reminders/${id}/dismiss`),
  remove: (id: number) => apiDelete<{ id: number }>(`/reminders/${id}`),
  runGenerator: () => apiPost<{ generated: number }>("/reminders/run-generator"),
};
