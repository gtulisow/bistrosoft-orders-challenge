<template>
  <div id="app">
    <nav v-if="authStore.isAuthenticated" class="navbar">
      <div class="container">
        <div class="nav-content">
          <router-link to="/" class="nav-brand">
            <h2>🍽️ Bistrosoft Orders</h2>
          </router-link>
          <div class="nav-links">
            <router-link to="/" class="nav-link">Inicio</router-link>
            <router-link to="/customers" class="nav-link">Clientes</router-link>
            <router-link to="/orders" class="nav-link">Pedidos</router-link>
          </div>
          <div class="nav-user">
            <span class="user-email">{{ authStore.email }}</span>
            <button @click="handleLogout" class="btn btn-secondary btn-sm">
              Cerrar sesión
            </button>
          </div>
        </div>
      </div>
    </nav>
    
    <main>
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const router = useRouter()
const authStore = useAuthStore()

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  background: white;
  box-shadow: var(--shadow-sm);
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
  gap: 2rem;
}

.nav-brand {
  text-decoration: none;
  color: var(--gray-900);
}

.nav-brand h2 {
  margin: 0;
  font-size: 1.5rem;
}

.nav-links {
  display: flex;
  gap: 2rem;
  flex: 1;
}

.nav-link {
  text-decoration: none;
  color: var(--gray-600);
  font-weight: 500;
  transition: color 0.2s;
  padding: 0.5rem 0;
}

.nav-link:hover {
  color: var(--primary-color);
}

.nav-link.router-link-active {
  color: var(--primary-color);
  border-bottom: 2px solid var(--primary-color);
}

.nav-user {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-email {
  font-size: 0.875rem;
  color: var(--gray-600);
  font-weight: 500;
}

main {
  min-height: calc(100vh - 80px);
}

@media (max-width: 768px) {
  .nav-content {
    flex-wrap: wrap;
  }
  
  .nav-links {
    flex: 100%;
    order: 3;
    justify-content: center;
  }
  
  .user-email {
    display: none;
  }
}
</style>
