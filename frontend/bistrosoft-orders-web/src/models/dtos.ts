// Customer DTOs
export interface CreateCustomerRequest {
  name: string
  email: string
  phoneNumber?: string
}

export interface CustomerDto {
  id: string
  name: string
  email: string
  phoneNumber: string | null
  orders: OrderSummaryDto[]
}

export interface CustomerListDto {
  id: string
  name: string
  email: string
  phoneNumber: string | null
}

// Order DTOs
export interface CreateOrderRequest {
  customerId: string
  items: OrderItemInput[]
}

export interface OrderItemInput {
  productId: string
  quantity: number
}

export interface OrderDto {
  id: string
  customerId: string
  totalAmount: number
  createdAt: string
  status: OrderStatusDto
  items: OrderItemDto[]
}

export interface OrderItemDto {
  productId: string
  productName: string
  quantity: number
  unitPrice: number
  lineTotal: number
}

export interface OrderSummaryDto {
  id: string
  customerId: string
  totalAmount: number
  createdAt: string
  status: OrderStatusDto
  items: OrderItemDto[]
}

export interface UpdateOrderStatusRequest {
  orderId: string
  newStatusId: string
}

// Order Status (estructura con GUID del backend)
export interface OrderStatusDto {
  id: string
  name: string
  description: string
}

// GUIDs de los estados (constantes del backend)
export const OrderStatusIds = {
  Pending: '00000000-0000-0000-0000-000000000001',
  Paid: '00000000-0000-0000-0000-000000000002',
  Shipped: '00000000-0000-0000-0000-000000000003',
  Delivered: '00000000-0000-0000-0000-000000000004',
  Cancelled: '00000000-0000-0000-0000-000000000005'
} as const

// Type para los IDs de estados
export type OrderStatusId = typeof OrderStatusIds[keyof typeof OrderStatusIds]

// Product DTO (del backend)
export interface ProductDto {
  id: string
  name: string
  price: number
  stockQuantity: number
}

// Auth DTOs
export interface LoginRequestDto {
  email: string
  password: string
}

export interface LoginResponseDto {
  token: string
  expiresAtUtc: string
  userId: string
  email: string
}

// Error del backend (ProblemDetails)
export interface ProblemDetails {
  type?: string
  title: string
  status: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}
