import apiClient from './http'
import type { CreateCustomerRequest, CustomerDto } from '@/models/dtos'

export const customersApi = {
  async create(request: CreateCustomerRequest): Promise<CustomerDto> {
    const response = await apiClient.post<CustomerDto>('/api/customers', request)
    return response.data
  },

  async getById(id: string): Promise<CustomerDto> {
    const response = await apiClient.get<CustomerDto>(`/api/customers/${id}`)
    return response.data
  }
}
