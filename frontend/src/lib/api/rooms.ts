import { apiClient } from './index'
import type { ApiResponse, RoomDto, PhotoDto } from '@/types'

export interface CreateRoomPayload {
  title: string
  description: string
  capacity: number
  pricePerNight: number
}

export interface UpdateRoomPayload {
  title?: string
  description?: string
  capacity?: number
  pricePerNight?: number
  isActive?: boolean
}

export const roomsApi = {
  getByProperty: (propertyId: string) =>
    apiClient
      .get<ApiResponse<RoomDto[]>>(`/api/v1/properties/${propertyId}/rooms`)
      .then(r => r.data.data ?? []),

  getById: (id: string) =>
    apiClient
      .get<ApiResponse<RoomDto>>(`/api/v1/rooms/${id}`)
      .then(r => r.data.data!),

  create: (propertyId: string, payload: CreateRoomPayload) =>
    apiClient
      .post<ApiResponse<RoomDto>>(`/api/v1/properties/${propertyId}/rooms`, payload)
      .then(r => r.data.data!),

  update: (id: string, payload: UpdateRoomPayload) =>
    apiClient
      .put<ApiResponse<RoomDto>>(`/api/v1/rooms/${id}`, payload)
      .then(r => r.data.data!),

  delete: (id: string) =>
    apiClient.delete(`/api/v1/rooms/${id}`),

  uploadPhoto: (roomId: string, file: File) => {
    const form = new FormData()
    form.append('file', file)
    return apiClient
      .post<ApiResponse<PhotoDto>>(`/api/v1/rooms/${roomId}/photos`, form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      .then(r => r.data.data!)
  },

  deletePhoto: (photoId: string) =>
    apiClient.delete(`/api/v1/photos/${photoId}`),

  addAmenity: (roomId: string, amenityId: string) =>
    apiClient
      .post<ApiResponse<RoomDto>>(`/api/v1/rooms/${roomId}/amenities/${amenityId}`)
      .then(r => r.data.data!),
}
