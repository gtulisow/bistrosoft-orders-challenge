import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/auth.api'

const STORAGE_KEYS = {
  token: 'auth.token',
  email: 'auth.email',
  userId: 'auth.userId',
  expiresAtUtc: 'auth.expiresAtUtc'
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const email = ref<string | null>(null)
  const userId = ref<string | null>(null)
  const expiresAtUtc = ref<string | null>(null)
  const isLoggingIn = ref(false)
  const loginError = ref<string | null>(null)

  const isAuthenticated = computed(() => {
    if (!token.value || !expiresAtUtc.value) return false
    
    // Verificar si el token no ha expirado
    const expiryDate = new Date(expiresAtUtc.value)
    const now = new Date()
    
    return expiryDate > now
  })

  async function login(emailInput: string, password: string) {
    isLoggingIn.value = true
    loginError.value = null

    try {
      const response = await authApi.login(emailInput, password)
      
      // Guardar en estado
      token.value = response.token
      email.value = response.email
      userId.value = response.userId
      expiresAtUtc.value = response.expiresAtUtc

      // Persistir en localStorage
      localStorage.setItem(STORAGE_KEYS.token, response.token)
      localStorage.setItem(STORAGE_KEYS.email, response.email)
      localStorage.setItem(STORAGE_KEYS.userId, response.userId)
      localStorage.setItem(STORAGE_KEYS.expiresAtUtc, response.expiresAtUtc)

    } catch (err: any) {
      loginError.value = err.message || 'Credenciales inválidas'
      throw err
    } finally {
      isLoggingIn.value = false
    }
  }

  function logout() {
    // Limpiar estado
    token.value = null
    email.value = null
    userId.value = null
    expiresAtUtc.value = null
    loginError.value = null

    // Limpiar localStorage
    localStorage.removeItem(STORAGE_KEYS.token)
    localStorage.removeItem(STORAGE_KEYS.email)
    localStorage.removeItem(STORAGE_KEYS.userId)
    localStorage.removeItem(STORAGE_KEYS.expiresAtUtc)
  }

  function restore() {
    // Restaurar desde localStorage
    const storedToken = localStorage.getItem(STORAGE_KEYS.token)
    const storedEmail = localStorage.getItem(STORAGE_KEYS.email)
    const storedUserId = localStorage.getItem(STORAGE_KEYS.userId)
    const storedExpiresAtUtc = localStorage.getItem(STORAGE_KEYS.expiresAtUtc)

    if (storedToken && storedEmail && storedUserId && storedExpiresAtUtc) {
      // Verificar si no ha expirado
      const expiryDate = new Date(storedExpiresAtUtc)
      const now = new Date()

      if (expiryDate > now) {
        token.value = storedToken
        email.value = storedEmail
        userId.value = storedUserId
        expiresAtUtc.value = storedExpiresAtUtc
      } else {
        // Token expirado, limpiar
        logout()
      }
    }
  }

  function clearError() {
    loginError.value = null
  }

  return {
    token,
    email,
    userId,
    expiresAtUtc,
    isLoggingIn,
    loginError,
    isAuthenticated,
    login,
    logout,
    restore,
    clearError
  }
})
