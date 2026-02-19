API REST profesional para gestión de tareas con .NET 9, ASP.NET Core Identity y arquitectura en capas.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoftsqlserver)

## 🚀 Características

### Arquitectura
- ✅ **Arquitectura en Capas** - Separación profesional de responsabilidades
- ✅ **Repository Pattern** - Abstracción de acceso a datos
- ✅ **Dependency Injection** - ICurrentUserService para gestión de contexto
- ✅ **Service Layer Pattern** - Lógica de negocio encapsulada
- ✅ **SOLID Principles** - Código mantenible y escalable

### Seguridad
- ✅ **ASP.NET Core Identity** - Gestión profesional de usuarios
- ✅ **JWT Authentication** - Tokens seguros con expiración de 7 días
- ✅ **Políticas de contraseña** - 6+ caracteres, mayúsculas, minúsculas, números
- ✅ **Account Lockout** - Bloqueo automático después de 5 intentos fallidos
- ✅ **Aislamiento de datos** - Cada usuario ve solo sus recursos

### Calidad de Código
- ✅ **Global Exception Handler** - Manejo centralizado de errores
- ✅ **Structured Logging** - ILogger con contexto para debugging
- ✅ **Validaciones** - Data Annotations en todos los DTOs
- ✅ **ErrorMessages centralizados** - Mensajes consistentes
- ✅ **Try-Catch estratégico** - En operaciones críticas

### Funcionalidades
- ✅ **CRUD completo** - Tareas y Categorías
- ✅ **Filtros dinámicos** - Por estado, prioridad, categoría y búsqueda
- ✅ **Estados de tareas** - Pendiente, En Proceso, Terminado
- ✅ **Prioridades** - Baja, Alta
- ✅ **Categorías personalizadas** - Organización flexible

## 🛠️ Stack Tecnológico

- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 9** - ORM
- **ASP.NET Core Identity** - Autenticación y autorización
- **JWT (JSON Web Tokens)** - Tokens de autenticación
- **AutoMapper 12** - Mapeo objeto a objeto
- **SQL Server** - Base de datos relacional
- **Swagger/OpenAPI** - Documentación interactiva

## 📦 Instalación

### Requisitos Previos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) o SQL Server LocalDB
- Visual Studio 2022 / VS Code / Rider (opcional)

### Pasos de Instalación

**1. Clonar el repositorio**
```bash
git clone https://github.com/LucianoNieva/GestionDeTareas.git
cd GestionDeTareas
```

**2. Configurar la base de datos**

Editar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server = DESKTOP-14QRTTP;Database=GestionTareasDB;Integrated Security=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "jnksdjkahdsjkagdjhagdgjkahsdgjhksagdsjghkajhgdjashkdsaghjkghjksdagjhkasghjkjghkdsghjkaghjkads"
  }
}
```

**3. Aplicar migraciones**
```bash
dotnet ef database update
```

**4. Ejecutar el proyecto**
```bash
dotnet run
```

**5. Acceder a Swagger**
```
https://localhost:5001/swagger
```

## 📚 Endpoints de la API

### 🔐 Autenticación

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/registrar` | Registrar nuevo usuario | No |
| POST | `/api/auth/login` | Iniciar sesión | No |

**Ejemplo de registro:**
```json
POST /api/auth/registrar
{
  "nombre": "Juan",
  "email": "juan@example.com",
  "password": "Password123"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "juan@example.com",
  "nombre": "Juan",
  "expiracion": "2026-02-25T10:00:00Z"
}
```

### 📁 Categorías

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/categorias` | Listar mis categorías | Sí |
| GET | `/api/categorias/{id}` | Obtener categoría específica | Sí |
| POST | `/api/categorias` | Crear nueva categoría | Sí |
| PUT | `/api/categorias/{id}` | Actualizar categoría | Sí |
| DELETE | `/api/categorias/{id}` | Eliminar categoría | Sí |
| GET | `/api/categorias/{id}/tarea` | Obtener tareas de una categoría | Sí |

### ✅ Tareas

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/tarea` | Listar mis tareas | Sí |
| GET | `/api/tarea/{id}` | Obtener tarea específica | Sí |
| POST | `/api/tarea` | Crear nueva tarea | Sí |
| PUT | `/api/tarea/{id}` | Actualizar tarea | Sí |
| DELETE | `/api/tarea/{id}` | Eliminar tarea | Sí |
| PATCH | `/api/tarea/{id}/completar` | Marcar tarea como completada | Sí |
| GET | `/api/tarea/filtrar` | Filtrar tareas con múltiples criterios | Sí |

**Ejemplo de filtrado:**
```
GET /api/tareas/filtrar?estado=1&prioridad=2&categoriaId=5&buscar=examen
```

## 🏗️ Arquitectura del Proyecto
```
┌─────────────────────────────────────────────┐
│           Client (React/Mobile)             │
└────────────────┬────────────────────────────┘
                 ↓ HTTP/JSON
┌─────────────────────────────────────────────┐
│              Controllers                     │
│  • AuthController                            │
│  • TareasController                          │
│  • CategoriasController                      │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│              Services                        │
│  • TareaService                              │
│  • CategoryService                           │
│  • ICurrentUserService                       │
│  • TokenService                              │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│            Repositories                      │
│  • ITareaRepository                          │
│  • ICategoryRepository                       │
│  • Implementaciones concretas                │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│          DbContext (EF Core)                 │
│          ApplicationDbContext                │
└────────────────┬────────────────────────────┘
                 ↓
┌─────────────────────────────────────────────┐
│              SQL Server                      │
└─────────────────────────────────────────────┘
```

### Capas del Proyecto

- **Controllers**: Manejo de requests HTTP y responses
- **Services**: Lógica de negocio y reglas de aplicación
- **Repositories**: Abstracción de acceso a datos
- **DTOs**: Data Transfer Objects para comunicación
- **Entities**: Modelos de dominio (Usuario, Tarea, Categoria)
- **Middlewares**: Cross-cutting concerns (logging, errores)

## 🔒 Seguridad Implementada

### Autenticación JWT
- Tokens con expiración de 7 días
- Claims: UserId, Email
- Validación automática en cada request

### Políticas de Contraseña
- Mínimo 6 caracteres
- Al menos una mayúscula
- Al menos una minúscula
- Al menos un número

### Protección de Cuenta
- Lockout automático después de 5 intentos fallidos
- Bloqueo temporal de 5 minutos
- Reset de contador en login exitoso

### Aislamiento de Datos
- Cada usuario accede solo a sus recursos
- Validación de pertenencia en cada operación
- ICurrentUserService para contexto seguro

## 📊 Patrones de Diseño Utilizados

- **Repository Pattern** - Abstracción de persistencia
- **Dependency Injection** - Inversión de control
- **DTO Pattern** - Separación de modelos
- **Service Layer Pattern** - Encapsulación de lógica
- **Middleware Pattern** - Pipeline de requests
- **Factory Pattern** - Creación de tokens

## 🧪 Ejemplos de Uso

### 1. Registrar y autenticar usuario
```bash
# Registrar
curl -X POST https://localhost:5001/api/auth/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Juan",
    "email": "juan@test.com",
    "password": "Test123"
  }'

# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "juan@test.com",
    "password": "Test123"
  }'
```

### 2. Crear categoría
```bash
curl -X POST https://localhost:5001/api/categorias \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {tu-token}" \
  -d '{
    "nombre": "Universidad"
  }'
```

### 3. Crear tarea
```bash
curl -X POST https://localhost:5001/api/tareas \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {tu-token}" \
  -d '{
    "titulo": "Estudiar para examen de matemáticas",
    "prioridad": 2,
    "idCategoria": 1
  }'
```

## 🚧 Roadmap

- [x] **v1.0** - API base con autenticación ✅
  - CRUD completo de tareas y categorías
  - Autenticación JWT
  - Arquitectura en capas
  - Global Exception Handler
  - Logging estructurado

- [ ] **v2.0** - Clean Architecture + Roles
  - Refactorización a Clean Architecture
  - Sistema de roles (Admin/Usuario)
  - Use Cases separados
  - Dominio independiente

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 👨‍💻 Autor

**[Luciano Fabrizio Nieva]**

- GitHub: https://github.com/LucianoNieva
- LinkedIn: https://www.linkedin.com/in/luciano-fabrizio-nieva-ab421623b/
- Email: lucianonieva49@gmail.com

---

⭐ **Si este proyecto te fue útil, dale una estrella en GitHub!** ⭐

---

**Versión:** 1.0.0  
**Última actualización:** Febrero 2026