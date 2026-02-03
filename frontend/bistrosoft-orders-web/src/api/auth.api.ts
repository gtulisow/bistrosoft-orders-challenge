import apiClient from './http'
import type { LoginRequestDto, LoginResponseDto } from '@/models/dtos'

export const authApi = {
  async login(email: string, password: string): Promise<LoginResponseDto> {
    const request: LoginRequestDto = { email, password }
    const response = await apiClient.post<LoginResponseDto>('/api/auth/login', request)
    return response.data
  }
} 
