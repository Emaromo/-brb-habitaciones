export type UserRole = 'Cliente' | 'DuenoAlojamiento' | 'Administrador'

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  role: UserRole
  avatarUrl?: string
}

export interface ApiResponse<T> {
  success: boolean
  data: T | null
  message: string | null
  errors?: string[] | null
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface PhotoDto {
  id: string
  url: string
  publicId: string
  isCover: boolean
  displayOrder: number
}

export interface AmenityDto {
  id: string
  name: string
  icon: string
  category: string
}

export interface RoomSummaryDto {
  id: string
  title: string
  capacity: number
  pricePerNight: number
  coverPhotoUrl: string | null
}

export interface PropertySummaryDto {
  id: string
  name: string
  province: string
  city: string
  propertyType: string
  roomCount: number
  minPricePerNight: number | null
  coverPhotoUrl: string | null
  createdAt: string
}

export interface PropertyDto {
  id: string
  ownerId: string
  ownerName: string
  name: string
  description: string
  province: string
  city: string
  address: string
  propertyType: string
  isActive: boolean
  isApproved: boolean
  rooms: RoomSummaryDto[]
  photos: PhotoDto[]
  createdAt: string
}

export interface RoomDto {
  id: string
  propertyId: string
  propertyName: string
  title: string
  description: string
  capacity: number
  pricePerNight: number
  isActive: boolean
  photos: PhotoDto[]
  amenities: AmenityDto[]
  createdAt: string
}

export interface AdminStatsDto {
  totalUsers: number
  totalProperties: number
  totalRooms: number
  pendingProperties: number
  reservationsThisMonth: number
  revenueThisMonth: number
  activeReservations: number
}

export interface AdminUserDto {
  id: string
  email: string
  firstName: string
  lastName: string
  role: string
  isActive: boolean
  createdAt: string
}

export interface AdminPropertyDto {
  id: string
  name: string
  ownerName: string
  province: string
  city: string
  propertyType: string
  isActive: boolean
  isApproved: boolean
  roomCount: number
  createdAt: string
}

export type ReservationStatus = 'Confirmada' | 'Cancelada' | 'Completada'

export interface ReservationDto {
  id: string
  roomId: string
  roomTitle: string
  propertyId: string
  propertyName: string
  propertyCity: string
  clientId: string
  clientName: string
  checkInDate: string   // "YYYY-MM-DD"
  checkOutDate: string  // "YYYY-MM-DD"
  guestCount: number
  nights: number
  totalPrice: number
  status: ReservationStatus
  cancellationReason: string | null
  createdAt: string
}
