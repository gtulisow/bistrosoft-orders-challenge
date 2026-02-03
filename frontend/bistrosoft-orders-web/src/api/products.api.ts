import apiClient from './http'
import type { ProductDto } from '@/models/dtos'

export const productsApi = {
  async getAll(): Promise<ProductDto[]> {
    const response = await apiClient.get<ProductDto[]>('/api/products')
    return response.data
  }
}
