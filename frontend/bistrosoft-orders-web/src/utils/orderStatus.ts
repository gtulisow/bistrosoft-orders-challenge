import type { OrderStatusDto } from '@/models/dtos'
import { OrderStatusIds } from '@/models/dtos'

// Mapeo de status a etiqueta en español
export function statusToLabel(status: OrderStatusDto): string {
  switch (status.id) {
    case OrderStatusIds.Pending: return 'Pendiente'
    case OrderStatusIds.Paid: return 'Pagado'
    case OrderStatusIds.Shipped: return 'Enviado'
    case OrderStatusIds.Delivered: return 'Entregado'
    case OrderStatusIds.Cancelled: return 'Cancelado'
    default: return status.name || 'Desconocido'
  }
}

// Mapeo de status a clase CSS para badge
export function statusBadgeClass(status: OrderStatusDto): string {
  switch (status.id) {
    case OrderStatusIds.Pending: return 'badge-pending'
    case OrderStatusIds.Paid: return 'badge-paid'
    case OrderStatusIds.Shipped: return 'badge-shipped'
    case OrderStatusIds.Delivered: return 'badge-delivered'
    case OrderStatusIds.Cancelled: return 'badge-cancelled'
    default: return 'badge-pending'
  }
}

// Opciones de siguiente estado disponibles
export interface StatusOption {
  label: string
  statusId: string
}

export function getNextStatusOptions(currentStatus: OrderStatusDto): StatusOption[] {
  switch (currentStatus.id) {
    case OrderStatusIds.Pending:
      return [{ label: 'Marcar como Pagado', statusId: OrderStatusIds.Paid }]
    case OrderStatusIds.Paid:
      return [{ label: 'Marcar como Enviado', statusId: OrderStatusIds.Shipped }]
    case OrderStatusIds.Shipped:
      return [{ label: 'Marcar como Entregado', statusId: OrderStatusIds.Delivered }]
    case OrderStatusIds.Delivered:
    case OrderStatusIds.Cancelled:
      return []
    default:
      return []
  }
}

// Verificar si se puede cancelar
export function canCancel(status: OrderStatusDto): boolean {
  return status.id === OrderStatusIds.Pending
}
