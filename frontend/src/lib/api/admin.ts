import { apiClient } from './index'
import type { ApiResponse, AdminStatsDto, AdminUserDto, AdminPropertyDto, ReservationDto } from '@/types'

export const adminApi = {
  getStats: () =>
    apiClient.get<ApiResponse<AdminStatsDto>>('/api/v1/admin/stats').then(r => r.data.data!),

  getUsers: () =>
    apiClient.get<ApiResponse<AdminUserDto[]>>('/api/v1/admin/users').then(r => r.data.data ?? []),

  changeRole: (userId: string, role: string) =>
    apiClient
      .put<ApiResponse<AdminUserDto>>(`/api/v1/admin/users/${userId}/role`, { role })
      .then(r => r.data.data!),

  toggleActive: (userId: string) =>
    apiClient
      .put<ApiResponse<AdminUserDto>>(`/api/v1/admin/users/${userId}/deactivate`)
      .then(r => r.data.data!),

  getProperties: () =>
    apiClient
      .get<ApiResponse<AdminPropertyDto[]>>('/api/v1/admin/properties')
      .then(r => r.data.data ?? []),

  approveProperty: (id: string) =>
    apiClient
      .put<ApiResponse<AdminPropertyDto>>(`/api/v1/admin/properties/${id}/approve`)
      .then(r => r.data.data!),

  rejectProperty: (id: string) =>
    apiClient
      .put<ApiResponse<AdminPropertyDto>>(`/api/v1/admin/properties/${id}/reject`)
      .then(r => r.data.data!),

  getReservations: () =>
    apiClient
      .get<ApiResponse<ReservationDto[]>>('/api/v1/admin/reservations')
      .then(r => r.data.data ?? []),
}
