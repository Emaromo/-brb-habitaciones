import { apiClient } from './index'
import type { ApiResponse, ReservationDto } from '@/types'

export interface CreateReservationPayload {
  roomId: string
  checkInDate: string  // "YYYY-MM-DD"
  checkOutDate: string // "YYYY-MM-DD"
  guestCount: number
}

export const reservationsApi = {
  getBookedDates: (roomId: string, from: string, to: string) =>
    apiClient
      .get<ApiResponse<string[]>>(`/api/v1/rooms/${roomId}/availability`, {
        params: { from, to },
      })
      .then(r => r.data.data ?? []),

  create: (payload: CreateReservationPayload) =>
    apiClient
      .post<ApiResponse<ReservationDto>>('/api/v1/reservations', payload)
      .then(r => r.data.data!),

  getMy: () =>
    apiClient
      .get<ApiResponse<ReservationDto[]>>('/api/v1/reservations/my')
      .then(r => r.data.data ?? []),

  cancel: (id: string, reason?: string) =>
    apiClient
      .put<ApiResponse<ReservationDto>>(`/api/v1/reservations/${id}/cancel`, { reason })
      .then(r => r.data.data!),
}
