<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h1>🍽️ Bistrosoft Orders</h1>
        <p class="subtitle">Inicia sesión para continuar</p>
      </div>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="form.email"
            type="email"
            placeholder="tu@email.com"
            required
            :disabled="authStore.isLoggingIn"
            autocomplete="email"
          />
        </div>

        <div class="form-group">
          <label for="password">Contraseña</label>
          <input
            id="password"
            v-model="form.password"
            type="password"
            placeholder="••••••••"
            required
            :disabled="authStore.isLoggingIn"
            autocomplete="current-password"
          />
        </div>

        <div v-if="authStore.loginError" class="alert alert-error">
          ⚠️ {{ authStore.loginError }}
        </div>

        <button 
          type="submit" 
          class="btn btn-primary btn-block"
          :disabled="authStore.isLoggingIn"
        >
          {{ authStore.isLoggingIn ? 'Ingresando...' : 'Ingresar' }}
        </button>
      </form>

      <div class="login-footer">
        <p class="text-sm text-gray-500">
          Sistema de gestión de pedidos
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({
  email: '',
  password: ''
})

async function handleLogin() {
  authStore.clearError()
  
  try {
    await authStore.login(form.email, form.password)
    
    // Redirigir a home después del login exitoso
    router.push('/')
  } catch (err) {
    // Error ya manejado en el store
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

.login-card {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-lg);
  padding: 3rem;
  width: 100%;
  max-width: 420px;
}

.login-header {
  text-align: center;
  margin-bottom: 2rem;
}

.login-header h1 {
  margin-bottom: 0.5rem;
  font-size: 2rem;
  color: var(--gray-900);
}

.subtitle {
  color: var(--gray-600);
  font-size: 1rem;
  margin: 0;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.btn-block {
  width: 100%;
  margin-top: 0.5rem;
}

.login-footer {
  margin-top: 2rem;
  text-align: center;
}

@media (max-width: 480px) {
  .login-card {
    padding: 2rem;
  }
  
  .login-header h1 {
    font-size: 1.5rem;
  }
}
</style>
