import apiClient from './http'
import type { CreateOrderRequest, OrderDto, OrderSummaryDto } from '@/models/dtos'

export const ordersApi = {
  async create(request: CreateOrderRequest): Promise<OrderDto> {
    const response = await apiClient.post<OrderDto>('/api/orders', request)
    return response.data
  },

  async getCustomerOrders(customerId: string): Promise<OrderSummaryDto[]> {
    const response = await apiClient.get<OrderSummaryDto[]>(`/api/customers/${customerId}/orders`)
    return response.data
  },

  async updateOrderStatus(orderId: string, newStatusId: string): Promise<void> {
    await apiClient.put(`/api/orders/${orderId}/status`, { 
      orderId,
      newStatusId 
    })
  }
}
