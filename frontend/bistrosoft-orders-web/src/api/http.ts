import axios, { AxiosError } from 'axios'
import type { ProblemDetails } from '@/models/dtos'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor: attach Bearer token
apiClient.interceptors.request.use(
  (config) => {
    // Leer token desde localStorage (evita dependencia circular con store)
    const token = localStorage.getItem('auth.token')
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor: handle errors and 401
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ProblemDetails>) => {
    // Handle 401 Unauthorized
    if (error.response?.status === 401) {
      // Evitar loop infinito: no redirigir si ya estamos en /auth/login
      const isLoginRequest = error.config?.url?.includes('/api/auth/login')
      
      if (!isLoginRequest) {
        // Limpiar localStorage
        localStorage.removeItem('auth.token')
        localStorage.removeItem('auth.email')
        localStorage.removeItem('auth.userId')
        localStorage.removeItem('auth.expiresAtUtc')
        
        // Redirigir a login
        // Importación dinámica para evitar dependencia circular
        const router = await import('@/router')
        router.default.push('/login')
      }
    }
    
    // Formatear mensaje de error
    if (error.response?.data) {
      const problemDetails = error.response.data
      
      if (problemDetails.errors) {
        const validationErrors = Object.entries(problemDetails.errors)
          .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
          .join('\n')
        
        error.message = `${problemDetails.title || 'Error de validación'}\n${validationErrors}`
      } else {
        error.message = problemDetails.detail || problemDetails.title || error.message
      }
    }
    
    return Promise.reject(error)
  }
)

export default apiClient
