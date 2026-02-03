<template>
  <div class="container">
    <div class="home-header">
      <h1>Bienvenido al Sistema de Gestión de Pedidos</h1>
      <p class="subtitle">Gestiona clientes y pedidos de manera eficiente</p>
    </div>

    <div class="grid grid-cols-2">
      <div class="card feature-card">
        <div class="feature-icon">👥</div>
        <h3>Gestión de Clientes</h3>
        <p>Crea y busca clientes por ID. Visualiza su información completa y su historial de pedidos.</p>
        <router-link to="/customers" class="btn btn-primary">
          Ir a Clientes
        </router-link>
      </div>

      <div class="card feature-card">
        <div class="feature-icon">📦</div>
        <h3>Gestión de Pedidos</h3>
        <p>Crea nuevos pedidos, visualiza detalles de productos y actualiza el estado de los pedidos.</p>
        <router-link to="/orders" class="btn btn-primary">
          Ir a Pedidos
        </router-link>
      </div>
    </div>

    <div class="card info-section">
      <h3>Características del Sistema</h3>
      <ul class="features-list">
        <li>✅ Crear clientes con nombre, email y teléfono</li>
        <li>✅ Buscar clientes por ID único</li>
        <li>✅ Crear pedidos con múltiples productos</li>
        <li>✅ Visualizar pedidos con detalle de items</li>
        <li>✅ Actualizar estado del pedido (Pending → Paid → Shipped → Delivered)</li>
        <li>✅ Cancelar pedidos desde estado Pending</li>
        <li>✅ Validación de transiciones de estado en tiempo real</li>
      </ul>
    </div>

    <div class="card api-info">
      <h3>Información de la API</h3>
      <p class="text-sm text-gray-500">
        <strong>Base URL:</strong> {{ apiBaseUrl }}
      </p>
      <p class="text-sm text-gray-500 mt-4">
        <strong>Endpoints disponibles:</strong>
      </p>
      <ul class="endpoints-list">
        <li><code>POST /api/customers</code> - Crear cliente</li>
        <li><code>GET /api/customers/:id</code> - Obtener cliente</li>
        <li><code>POST /api/orders</code> - Crear pedido</li>
        <li><code>PUT /api/orders/:id/status</code> - Actualizar estado</li>
        <li><code>GET /api/customers/:id/orders</code> - Obtener pedidos del cliente</li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const apiBaseUrl = computed(() => import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000')
</script>

<style scoped>
.home-header {
  text-align: center;
  margin-bottom: 3rem;
  padding-top: 2rem;
}

.subtitle {
  font-size: 1.125rem;
  color: var(--gray-600);
  margin-top: 0.5rem;
}

.feature-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  transition: transform 0.2s, box-shadow 0.2s;
}

.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.feature-card h3 {
  margin-bottom: 1rem;
}

.feature-card p {
  color: var(--gray-600);
  margin-bottom: 1.5rem;
  flex-grow: 1;
}

.info-section {
  margin-top: 2rem;
}

.features-list {
  list-style: none;
  padding-left: 0;
}

.features-list li {
  padding: 0.5rem 0;
  color: var(--gray-700);
}

.api-info {
  background-color: var(--gray-50);
  border: 1px solid var(--gray-200);
}

.endpoints-list {
  list-style: none;
  padding-left: 0;
  margin-top: 0.5rem;
}

.endpoints-list li {
  padding: 0.375rem 0;
  font-size: 0.875rem;
}

.endpoints-list code {
  background-color: var(--gray-800);
  color: #10b981;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-family: 'Courier New', monospace;
  font-size: 0.8125rem;
}

@media (max-width: 768px) {
  .home-header h1 {
    font-size: 1.5rem;
  }
  
  .feature-icon {
    font-size: 2rem;
  }
}
</style>
