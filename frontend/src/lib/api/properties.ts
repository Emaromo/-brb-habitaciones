import { apiClient } from './index'
import type { ApiResponse, PagedResult, PropertySummaryDto, PropertyDto } from '@/types'

export interface CreatePropertyPayload {
  name: string
  description: string
  province: string
  city: string
  address: string
  propertyType: string
}

export interface UpdatePropertyPayload {
  name?: string
  description?: string
  province?: string
  city?: string
  address?: string
  propertyType?: string
  isActive?: boolean
}

export const propertiesApi = {
  getMyProperties: () =>
    apiClient
      .get<ApiResponse<PropertySummaryDto[]>>('/api/v1/properties/my')
      .then(r => r.data.data ?? []),

  getById: (id: string) =>
    apiClient
      .get<ApiResponse<PropertyDto>>(`/api/v1/properties/${id}`)
      .then(r => r.data.data!),

  create: (payload: CreatePropertyPayload) =>
    apiClient
      .post<ApiResponse<PropertyDto>>('/api/v1/properties', payload)
      .then(r => r.data.data!),

  update: (id: string, payload: UpdatePropertyPayload) =>
    apiClient
      .put<ApiResponse<PropertyDto>>(`/api/v1/properties/${id}`, payload)
      .then(r => r.data.data!),

  delete: (id: string) =>
    apiClient.delete(`/api/v1/properties/${id}`),
}
