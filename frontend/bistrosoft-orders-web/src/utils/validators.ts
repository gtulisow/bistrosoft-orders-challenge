export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export function isValidPhone(phone: string): boolean {
  // Acepta formatos comunes de teléfono (10-15 dígitos con separadores opcionales)
  const phoneRegex = /^[\d\s\-\+\(\)]{10,15}$/
  return phoneRegex.test(phone)
}

export function isNotEmpty(value: string): boolean {
  return value.trim().length > 0
}

export function isPositiveNumber(value: number): boolean {
  return !isNaN(value) && value > 0
}
