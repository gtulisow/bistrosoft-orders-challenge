# 📜 Scripts de Automatización

Esta carpeta contiene scripts para automatizar tareas comunes del proyecto.

## 📁 Estructura

```
scripts/
├── test/           # Testing automatizado
├── dev/            # Desarrollo diario
└── README.md       # Este archivo
```

> **Nota:** Los paquetes NuGet (como Serilog) se instalan automáticamente con `dotnet restore` o `dotnet build`.
> No se necesitan scripts de instalación separados.

---

## 🧪 TEST - Testing Automatizado

### `test-api.sh` / `test-api.ps1`

Ejecuta una suite completa de tests contra la API:
- ✅ Login con JWT
- ✅ Obtener productos (endpoint protegido)
- ✅ Verificar 401 sin token
- ✅ Crear y obtener customers

**Mac/Linux:**
```bash
# 1. Asegúrate de que la API esté corriendo
SEED_ADMIN_PASSWORD="Admin123!" dotnet run --project src/Bistrosoft.Orders.Api

# 2. En otra terminal
cd scripts/test
chmod +x test-api.sh
./test-api.sh
```

**Windows (PowerShell):**
```powershell
# 1. Asegúrate de que la API esté corriendo
$env:SEED_ADMIN_PASSWORD="Admin123!"
dotnet run --project src/Bistrosoft.Orders.Api

# 2. En otra terminal
cd scripts\test
.\test-api.ps1
```

---

## 👨‍💻 DEV - Desarrollo Diario

### `start-full-stack.sh` / `start-full-stack.ps1` ⭐ RECOMENDADO

**Levanta TODO el stack completo en Docker** (API + SQL Server) con un solo comando:
- ✅ Verifica que Docker esté corriendo
- ✅ Detiene instancias anteriores
- ✅ Build de imagen Docker de la API
- ✅ Levanta SQL Server + API juntos
- ✅ Espera a que estén healthy
- ✅ Muestra URLs y credenciales

**Mac/Linux:**
```bash
cd scripts/dev
./start-full-stack.sh
```

**Windows (PowerShell):**
```powershell
cd scripts\dev
.\start-full-stack.ps1
```

**Resultado:**
- 🌐 API en: `http://localhost:8080/swagger`
- 🗄️ SQL Server en: `localhost:1433`
- ✅ Todo configurado y listo para usar

---

### `start-api.sh` / `start-api.ps1`

Inicia la API **localmente** (sin Docker, con InMemory):
- ✅ Configura ASPNETCORE_ENVIRONMENT=Development
- ✅ Configura SEED_ADMIN_PASSWORD
- ✅ Detiene procesos anteriores en puerto 8080
- ✅ Inicia la API con `dotnet run`

**Mac/Linux:**
```bash
cd scripts/dev
./start-api.sh
```

**Windows (PowerShell):**
```powershell
cd scripts\dev
.\start-api.ps1
```

**Uso:** Desarrollo rápido sin Docker

---

### `start-docker.sh` / `start-docker.ps1`

Inicia **solo SQL Server** con Docker (para desarrollo local de la API):
- ✅ Verifica que Docker esté corriendo
- ✅ Ejecuta docker-compose up -d
- ✅ Muestra información de conexión

**Mac/Linux:**
```bash
cd scripts/dev
./start-docker.sh
```

**Windows (PowerShell):**
```powershell
cd scripts\dev
.\start-docker.ps1
```

**Uso:** Cuando quieres BD real pero ejecutar la API localmente con `dotnet run`

---

## 🎯 WORKFLOWS COMUNES

### 🚀 Workflow 1: Stack Completo en Docker (RECOMENDADO para Testing Real)

**Todo en contenedores - Producción-like**

```bash
# Mac/Linux
./scripts/dev/start-full-stack.sh

# Windows (PowerShell)
.\scripts\dev\start-full-stack.ps1

# Resultado:
# ✅ API en Docker (http://localhost:8080)
# ✅ SQL Server en Docker
# ✅ Networking configurado
# ✅ Migraciones aplicadas automáticamente
```

**Ventajas:**
- ✅ Réplica exacta de producción
- ✅ Un solo comando
- ✅ Aislado del sistema host
- ✅ Fácil de limpiar (`docker-compose down -v`)

---

### ⚡ Workflow 2: Desarrollo Rápido (InMemory, Sin Docker)

**API local, BD en memoria - Desarrollo ágil**

```bash
# Mac/Linux
./scripts/dev/start-api.sh

# Windows (PowerShell)
.\scripts\dev\start-api.ps1
```

**Ventajas:**
- ✅ Inicio instantáneo
- ✅ No necesita Docker
- ✅ Reinicio rápido
- ✅ Perfecto para TDD

---

### 🗄️ Workflow 3: SQL Server Real, API Local

**BD real en Docker, API con dotnet run**

```bash
# Mac/Linux
./scripts/dev/start-docker.sh
# Cambiar UseInMemory=false en appsettings.Development.json
./scripts/dev/start-api.sh

# Windows (PowerShell)
.\scripts\dev\start-docker.ps1
# Cambiar UseInMemory=false en appsettings.Development.json
.\scripts\dev\start-api.ps1
```

**Ventajas:**
- ✅ BD real para tests de migraciones
- ✅ Hot reload con `dotnet watch`
- ✅ Mejor para debugging

---

### 🧪 Testing Completo de API

```bash
# Mac/Linux
./scripts/test/test-api.sh

# Windows (PowerShell)
.\scripts\test\test-api.ps1
```

---

### 🗑️ Resetear Base de Datos (Eliminar Todos los Datos)

```bash
# Mac/Linux
./scripts/dev/reset-database.sh

# Windows (PowerShell)
.\scripts\dev\reset-database.ps1
```

---

## 📦 Instalación de Dependencias

**Los paquetes NuGet se instalan automáticamente** con cualquiera de estos comandos:

```bash
dotnet restore   # Solo restaura paquetes
dotnet build     # Restaura + compila
dotnet run       # Restaura + compila + ejecuta
```

No necesitas scripts separados para instalar paquetes. ✅

---

## 📝 Notas Importantes

### Para Mac/Linux (Git Bash también)
- **Siempre dar permisos:** `chmod +x script.sh` antes de ejecutar (ya dados)
- **Shebang importante:** `#!/bin/bash` en la primera línea
- **Ejecutar con:** `./script.sh` o `bash script.sh`

### Para Windows
- **PowerShell:** Puede requerir ejecutar primero:
  ```powershell
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```
- **Ejecutar con:** `.\script.ps1`
- **Git Bash:** Puede ejecutar scripts `.sh` en Windows

---

## 🔗 Links Útiles

- [Swagger UI](http://localhost:8080/swagger)
- [Documentación de Testing](../docs/TESTING.md)
- [Documentación de Serilog](../docs/SERILOG_LOGGING.md)

---

## 🤝 Contribuyendo

Al agregar nuevos scripts:
1. Crear AMBAS versiones (`.sh` y `.ps1`)
2. Agregar comentarios explicativos
3. Documentar en este README
4. Usar colores para output legible
5. Incluir manejo de errores
