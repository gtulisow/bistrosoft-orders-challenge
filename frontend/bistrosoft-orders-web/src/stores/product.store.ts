import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ProductDto } from '@/models/dtos'
import { productsApi } from '@/api/products.api'

export const useProductStore = defineStore('product', () => {
  const products = ref<ProductDto[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function loadProducts() {
    isLoading.value = true
    error.value = null
    
    try {
      products.value = await productsApi.getAll()
    } catch (err: any) {
      error.value = err.message || 'Error al cargar los productos'
      products.value = []
      console.error('Error loading products:', err)
    } finally {
      isLoading.value = false
    }
  }

  function getProductById(id: string): ProductDto | undefined {
    return products.value.find(p => p.id === id)
  }

  function getProductPrice(id: string): number {
    return getProductById(id)?.price ?? 0
  }

  return {
    products,
    isLoading,
    error,
    loadProducts,
    getProductById,
    getProductPrice
  }
})
