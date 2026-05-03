# 🏆 Quiniela Mundial 2026 — Documentación de API

**Base URL:** `http://localhost:8080`  
**Formato:** JSON  
**Autenticación:** JWT Bearer Token  

---

##  Autenticación

Para endpoints protegidos, incluir en el header:
```
Authorization: Bearer {token}
```

### Roles
| Rol | Descripción |
|-----|-------------|
| `Administrador` | Administrador del sistema |
| `User` | Usuario registrado |

---

##  Auth

### Registrarse
```
POST /api/v1/auth/register
```
**Acceso:** Público

**Body:**
```json
{
  "email": "usuario@example.com",
  "password": "MiPassword123",
  "firstName": "Juan",
  "lastName": "Pérez"
}
```

**Respuesta:**
```json
{
  "message": "Usuario registrado correctamente"
}
```

---

### Iniciar sesión
```
POST /api/v1/auth/login
```
**Acceso:** Público

**Body:**
```json
{
  "email": "usuario@example.com",
  "password": "MiPassword123"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "usuario@example.com",
  "fullName": "Juan Pérez",
  "role": {
    "name": "User"
  }
}
```

---

### Cerrar sesión
```
POST /api/v1/auth/logout
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
{
  "message": "Sesión cerrada correctamente"
}
```

---

### Perfil del usuario autenticado
```
GET /api/v1/auth/me
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
{
  "id": 1,
  "email": "usuario@example.com",
  "fullName": "Juan Pérez",
  "role": "User"
}
```

---

### Olvidé mi contraseña
```
POST /api/v1/auth/contraseña-olvidada
```
**Acceso:** Público

**Body:**
```json
{
  "email": "usuario@example.com"
}
```

**Respuesta:**
```json
{
  "message": "Si el email existe recibirás un correo con instrucciones"
}
```

---

### Recuperar contraseña
```
POST /api/v1/auth/recuperar-contraseña
```
**Acceso:** Público

**Body:**
```json
{
  "token": "uuid-del-email",
  "newPassword": "NuevoPassword123"
}
```

**Respuesta:**
```json
{
  "message": "Contraseña actualizada correctamente"
}
```

---

### Resetear contraseña (admin o propio usuario)
```
PUT /api/v1/auth/reset/{id}
```
**Acceso:**  JWT requerido

**Body:**
```json
{
  "newPassword": "NuevoPassword123"
}
```

**Respuesta:**
```json
{
  "message": "Contraseña actualizada correctamente"
}
```

---

##  Torneo

### Listar torneos
```
GET /api/v1/torneo?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Copa Mundial FIFA 2026",
    "año": 2026,
    "paisSede": "México, USA, Canadá",
    "fechaInicio": "2026-06-11T00:00:00Z",
    "fechaFin": "2026-07-19T00:00:00Z",
    "createdAt": "2025-01-01T00:00:00Z"
  }
]
```

---

### Torneo por ID
```
GET /api/v1/torneo/{id}
```
**Acceso:**  JWT requerido

---

### Torneos para dropdown
```
GET /api/v1/torneo/select
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  { "id": 1, "nombre": "Copa Mundial FIFA 2026" }
]
```

---

### Crear torneo
```
POST /api/v1/torneo
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Copa Mundial FIFA 2026",
  "año": 2026,
  "paisSede": "México, USA, Canadá",
  "fechaInicio": "2026-06-11T00:00:00Z",
  "fechaFin": "2026-07-19T00:00:00Z"
}
```

---

### Actualizar torneo
```
PUT /api/v1/torneo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Nuevo nombre",
  "paisSede": "México, USA, Canadá",
  "fechaInicio": "2026-06-11T00:00:00Z",
  "fechaFin": "2026-07-19T00:00:00Z"
}
```

---

### Eliminar torneo
```
DELETE /api/v1/torneo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

##  Estadio

### Listar estadios
```
GET /api/v1/estadio?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Estadio Azteca",
    "ciudad": "Ciudad de México",
    "pais": "México",
    "capacidad": 87000
  }
]
```

---

### Estadios para dropdown
```
GET /api/v1/estadio/select
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  { "id": 1, "nombre": "Estadio Azteca", "ciudad": "Ciudad de México" }
]
```

---

### Estadio por ID
```
GET /api/v1/estadio/{id}
```
**Acceso:**  JWT requerido

---

### Crear estadio
```
POST /api/v1/estadio
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Estadio Azteca",
  "ciudad": "Ciudad de México",
  "pais": "México",
  "capacidad": 87000
}
```

---

### Actualizar estadio
```
PUT /api/v1/estadio/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

### Eliminar estadio
```
DELETE /api/v1/estadio/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

##  Equipo

### Listar equipos
```
GET /api/v1/equipo?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Argentina",
    "codigoFifa": "ARG",
    "banderaUrl": "https://flagcdn.com/ar.svg",
    "entrenador": "Lionel Scaloni",
    "capitan": "Lionel Messi"
  }
]
```

---

### Equipos para dropdown
```
GET /api/v1/equipo/select
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Argentina",
    "banderaUrl": "https://flagcdn.com/ar.svg",
    "codigoFifa": "ARG"
  }
]
```

---

### Equipo por ID
```
GET /api/v1/equipo/{id}
```
**Acceso:**  JWT requerido

---

### Crear equipo
```
POST /api/v1/equipo
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Argentina",
  "codigoFifa": "ARG",
  "banderaUrl": "https://flagcdn.com/ar.svg",
  "entrenador": "Lionel Scaloni",
  "capitan": "Lionel Messi"
}
```

---

### Actualizar equipo
```
PUT /api/v1/equipo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

### Eliminar equipo
```
DELETE /api/v1/equipo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

##  Fase

### Listar fases
```
GET /api/v1/fase?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Fase de Grupos",
    "orden": 1,
    "torneoId": 1,
    "torneoNombre": "Copa Mundial FIFA 2026"
  }
]
```

---

### Fases por torneo
```
GET /api/v1/fase/torneo/{torneoId}
```
**Acceso:**  JWT requerido

---

### Fases para dropdown por torneo
```
GET /api/v1/fase/select/{torneoId}
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  { "id": 1, "nombre": "Fase de Grupos", "orden": 1 }
]
```

---

### Fase por ID
```
GET /api/v1/fase/{id}
```
**Acceso:**  JWT requerido

---

### Crear fase
```
POST /api/v1/fase
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Fase de Grupos",
  "orden": 1,
  "torneoId": 1
}
```

---

### Actualizar fase
```
PUT /api/v1/fase/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

### Eliminar fase
```
DELETE /api/v1/fase/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

##  Grupo

### Listar grupos
```
GET /api/v1/grupo?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Grupo A",
    "torneoId": 1,
    "torneoNombre": "Copa Mundial FIFA 2026",
    "equipos": [
      {
        "id": 1,
        "nombre": "México",
        "codigoFifa": "MEX",
        "banderaUrl": "https://flagcdn.com/mx.svg",
        "entrenador": "Javier Aguirre",
        "capitan": "Hirving Lozano"
      }
    ]
  }
]
```

---

### Grupos por torneo
```
GET /api/v1/grupo/torneo/{torneoId}
```
**Acceso:**  JWT requerido

---

### Grupos para dropdown por torneo
```
GET /api/v1/grupo/select/{torneoId}
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  { "id": 1, "nombre": "Grupo A" }
]
```

---

### Grupo por ID
```
GET /api/v1/grupo/{id}
```
**Acceso:**  JWT requerido

---

### Clasificación de un grupo
```
GET /api/v1/grupo/{grupoId}/clasificacion
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "equipo": {
      "id": 1,
      "nombre": "México",
      "codigoFifa": "MEX",
      "banderaUrl": "https://flagcdn.com/mx.svg"
    },
    "partidosJugados": 3,
    "ganados": 2,
    "empatados": 0,
    "perdidos": 1,
    "golesAFavor": 5,
    "golesEnContra": 3,
    "diferenciaGoles": 2,
    "puntos": 6
  }
]
```

---

### Crear grupo
```
POST /api/v1/grupo
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Grupo A",
  "torneoId": 1
}
```

---

### Asignar un equipo a un grupo
```
POST /api/v1/grupo/{grupoId}/equipos/{equipoId}
```
**Acceso:**  JWT + Rol `Administrador`  
> Crea automáticamente el registro en ClasificacionGrupo con valores en 0.

---

### Asignar varios equipos a un grupo
```
POST /api/v1/grupo/{grupoId}/equipos
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "equipoIds": [1, 2, 3, 4]
}
```

---

### Actualizar grupo
```
PUT /api/v1/grupo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "nombre": "Grupo A Actualizado"
}
```

---

### Eliminar grupo
```
DELETE /api/v1/grupo/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

### Remover equipo de un grupo
```
DELETE /api/v1/grupo/{grupoId}/equipos/{equipoId}
```
**Acceso:**  JWT + Rol `Administrador`  
> Elimina automáticamente el registro de ClasificacionGrupo.

---

##  Partido

### Listar partidos
```
GET /api/v1/partido?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "torneoId": 1,
    "fase": { "id": 1, "nombre": "Fase de Grupos", "orden": 1, "torneoId": 1, "torneoNombre": "" },
    "grupoId": 1,
    "grupoNombre": "Grupo A",
    "equipoLocal": { "id": 1, "nombre": "México", "codigoFifa": "MEX", "banderaUrl": "https://flagcdn.com/mx.svg" },
    "equipoVisitante": { "id": 2, "nombre": "Sudáfrica", "codigoFifa": "RSA", "banderaUrl": "https://flagcdn.com/za.svg" },
    "descripcionLocal": null,
    "descripcionVisitante": null,
    "fechaHora": "2026-06-11T19:00:00Z",
    "estadio": { "id": 1, "nombre": "Estadio Azteca", "ciudad": "Ciudad de México", "pais": "México", "capacidad": 87000 },
    "golesLocal": null,
    "golesVisitante": null,
    "finalizado": false
  }
]
```

---

### Partidos por torneo
```
GET /api/v1/partido/torneo/{torneoId}
```
**Acceso:**  JWT requerido

---

### Partidos por fase
```
GET /api/v1/partido/fase/{faseId}
```
**Acceso:**  JWT requerido

---

### Partidos por grupo
```
GET /api/v1/partido/grupo/{grupoId}
```
**Acceso:**  JWT requerido

---

### Partidos pendientes por torneo
```
GET /api/v1/partido/pendientes/{torneoId}
```
**Acceso:**  JWT requerido

---

### Partido por ID
```
GET /api/v1/partido/{id}
```
**Acceso:**  JWT requerido

---

### Crear partido
```
POST /api/v1/partido
```
**Acceso:**  JWT + Rol `Administrador`

**Body (fase de grupos):**
```json
{
  "torneoId": 1,
  "faseId": 1,
  "grupoId": 1,
  "equipoLocalId": 1,
  "equipoVisitanteId": 2,
  "descripcionLocal": null,
  "descripcionVisitante": null,
  "fechaHora": "2026-06-11T19:00:00Z",
  "estadioId": 1
}
```

**Body (fase eliminatoria — equipos aún no definidos):**
```json
{
  "torneoId": 1,
  "faseId": 2,
  "grupoId": null,
  "equipoLocalId": null,
  "equipoVisitanteId": null,
  "descripcionLocal": "Ganador Grupo A",
  "descripcionVisitante": "2do Grupo B",
  "fechaHora": "2026-07-04T19:00:00Z",
  "estadioId": 6
}
```

---

### Actualizar partido
```
PUT /api/v1/partido/{id}
```
**Acceso:**  JWT + Rol `Administrador`

**Body:**
```json
{
  "equipoLocalId": 1,
  "equipoVisitanteId": 2,
  "fechaHora": "2026-06-11T19:00:00Z",
  "estadioId": 1
}
```

---

### Ingresar resultado ⚡ Dispara actualización automática
```
PUT /api/v1/partido/{id}/resultado
```
**Acceso:**  JWT + Rol `Administrador`

> Al ingresar el resultado se ejecuta automáticamente:
> - Actualización de ClasificacionGrupo
> - Cálculo de PuntosGanados en todas las Predicciones
> - Actualización de Puntos en LigaMiembro
> - Actualización del bracket eliminatorio
> - Notificación en tiempo real via SignalR

**Body:**
```json
{
  "golesLocal": 2,
  "golesVisitante": 1
}
```

---

### Eliminar partido
```
DELETE /api/v1/partido/{id}
```
**Acceso:**  JWT + Rol `Administrador`

---

##  Liga

### Listar todas las ligas
```
GET /api/v1/liga?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Liga de los Amigos",
    "esDeApuestas": true,
    "precioPorUnirse": 100.00,
    "torneoId": 1,
    "creadaPor": "Juan Pérez",
    "totalMiembros": 5,
    "createdAt": "2026-01-01T00:00:00Z"
  }
]
```

---

### Mis ligas
```
GET /api/v1/liga/mis-ligas?page=1&pageSize=10
```
**Acceso:**  JWT requerido  
> Retorna las ligas donde el usuario autenticado es miembro.

---

### Buscar liga por nombre
```
GET /api/v1/liga/buscar?nombre=amigos&page=1&pageSize=10
```
**Acceso:**  JWT requerido  
> El parámetro `nombre` debe tener al menos 2 caracteres.

---

### Liga por ID
```
GET /api/v1/liga/{id}
```
**Acceso:**  JWT requerido

---

### Miembros de una liga
```
GET /api/v1/liga/{ligaId}/miembros
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "userId": 2,
    "fullName": "Juan Pérez",
    "email": "juan@example.com",
    "nombreEquipo": "Los Cracks",
    "esAdmin": true,
    "puntos": 15,
    "estado": "Aprobado",
    "fechaUnion": "2026-01-01T00:00:00Z"
  }
]
```

---

### Miembros pendientes de aprobación
```
GET /api/v1/liga/{ligaId}/miembros/pendientes
```
**Acceso:**  JWT requerido (solo admin de la liga)

---

### Ranking de una liga
```
GET /api/v1/liga/{ligaId}/ranking
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Pérez",
    "nombreEquipo": "Los Cracks",
    "puntos": 15,
    "premioAsignado": 375.00
  }
]
```

---

### Crear liga
```
POST /api/v1/liga
```
**Acceso:**  JWT requerido (cualquier usuario)

> Al crear la liga, el usuario queda automáticamente como admin y miembro aprobado.

**Body (liga de apuestas):**
```json
{
  "nombre": "Liga de los Amigos",
  "esDeApuestas": true,
  "precioPorUnirse": 100.00,
  "torneoId": 1,
  "nombreEquipo": "Los Cracks"
}
```

**Body (liga de diversión):**
```json
{
  "nombre": "Liga Casual",
  "esDeApuestas": false,
  "precioPorUnirse": null,
  "torneoId": 1,
  "nombreEquipo": "Mi Equipo"
}
```

---

### Unirse a una liga
```
POST /api/v1/liga/{ligaId}/unirse
```
**Acceso:**  JWT requerido

> El usuario queda en estado `Pendiente` hasta que el admin lo apruebe.

**Body:**
```json
{
  "nombreEquipo": "Los Rivales"
}
```

---

### Aprobar o rechazar miembro
```
POST /api/v1/liga/{ligaId}/aprobar
```
**Acceso:**  JWT requerido (solo admin de la liga)

**Body (aprobar):**
```json
{
  "userId": 3,
  "aprobar": true
}
```

**Body (rechazar):**
```json
{
  "userId": 3,
  "aprobar": false
}
```

---

### Actualizar liga
```
PUT /api/v1/liga/{id}
```
**Acceso:**  JWT requerido (solo admin de la liga)

**Body:**
```json
{
  "nombre": "Nuevo nombre",
  "esDeApuestas": true,
  "precioPorUnirse": 150.00
}
```

---

### Eliminar liga
```
DELETE /api/v1/liga/{id}
```
**Acceso:**  JWT requerido (solo admin de la liga)

---

### Salir de una liga
```
DELETE /api/v1/liga/{ligaId}/salir
```
**Acceso:**  JWT requerido  
> El admin no puede salir de su propia liga.

---

## 📨 Invitaciones de Liga

### Enviar invitación por email
```
POST /api/v1/invitacionliga/liga/{ligaId}
```
**Acceso:**  JWT requerido (solo admin de la liga)

> Envía un email al destinatario con un link que incluye el token de invitación. El link expira en 7 días.

**Body:**
```json
{
  "emailInvitado": "amigo@example.com"
}
```

**Respuesta:**
```json
{
  "id": 1,
  "emailInvitado": "amigo@example.com",
  "estado": "Pendiente",
  "fechaEnvio": "2026-01-01T00:00:00Z",
  "fechaExpiracion": "2026-01-08T00:00:00Z",
  "fechaRespuesta": null
}
```

---

### Ver invitaciones de una liga
```
GET /api/v1/invitacionliga/liga/{ligaId}
```
**Acceso:**  JWT requerido (solo admin de la liga)

---

### Responder invitación (aceptar o rechazar)
```
POST /api/v1/invitacionliga/responder
```
**Acceso:** Público (el invitado puede no estar logueado)

> Si el invitado no está registrado, debe registrarse primero y luego responder con su token.  
> Al aceptar, queda en estado `Pendiente` hasta que el admin lo apruebe.

**Body (aceptar):**
```json
{
  "token": "uuid-del-link-de-invitacion",
  "aceptar": true,
  "nombreEquipo": "Mi Equipo"
}
```

**Body (rechazar):**
```json
{
  "token": "uuid-del-link-de-invitacion",
  "aceptar": false,
  "nombreEquipo": ""
}
```

---

### Cancelar invitación
```
DELETE /api/v1/invitacionliga/{invitacionId}
```
**Acceso:**  JWT requerido (solo admin de la liga)

---

##  Predicciones

### Predicciones de una liga
```
GET /api/v1/prediccion/liga/{ligaId}?page=1&pageSize=10
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "id": 1,
    "partidoId": 1,
    "ligaId": 1,
    "equipoLocal": "México",
    "equipoVisitante": "Sudáfrica",
    "golesLocal": 2,
    "golesVisitante": 0,
    "puntosGanados": 3,
    "createdAt": "2026-06-10T00:00:00Z",
    "updatedAt": null
  }
]
```

---

### Mis predicciones en una liga
```
GET /api/v1/prediccion/mis-predicciones/{ligaId}?page=1&pageSize=10
```
**Acceso:**  JWT requerido  
> Retorna las predicciones del usuario autenticado en esa liga.

---

### Verificar si ya predijo un partido
```
GET /api/v1/prediccion/verificar?partidoId=1&ligaId=1
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
{
  "existe": true,
  "prediccion": { ... }
}
```

---

### Predicción por ID
```
GET /api/v1/prediccion/{id}
```
**Acceso:**  JWT requerido

---

### Crear predicción
```
POST /api/v1/prediccion
```
**Acceso:**  JWT requerido (miembro aprobado de la liga)

> Solo se puede crear si faltan más de 15 minutos para el inicio del partido.

**Body:**
```json
{
  "partidoId": 1,
  "ligaId": 1,
  "golesLocal": 2,
  "golesVisitante": 1
}
```

---

### Actualizar predicción
```
PUT /api/v1/prediccion/{id}
```
**Acceso:**  JWT requerido (solo el dueño)

> Solo se puede actualizar si faltan más de 15 minutos para el inicio del partido.

**Body:**
```json
{
  "golesLocal": 3,
  "golesVisitante": 0
}
```

---

##  Rankings y Premios

### Ranking global individual
```
GET /api/v1/ranking/global/usuarios
```
**Acceso:**  JWT requerido

> Muestra el ranking de usuarios usando el puntaje más alto entre todas las ligas de apuesta donde participa.

**Respuesta:**
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Pérez",
    "totalPuntos": 25,
    "premioAsignado": null
  }
]
```

---

### Ranking global de ligas
```
GET /api/v1/ranking/global/ligas
```
**Acceso:**  JWT requerido

> Ligas de apuesta ordenadas por promedio de puntos de sus miembros.

**Respuesta:**
```json
[
  {
    "posicion": 1,
    "ligaId": 1,
    "nombreLiga": "Liga de los Amigos",
    "promedioPuntos": 18.5,
    "totalMiembros": 6,
    "premioTotal": null,
    "premioPerCapita": null
  }
]
```

---

### Premios estimados de una liga
```
GET /api/v1/ranking/premios/liga/{ligaId}
```
**Acceso:**  JWT requerido

**Respuesta:**
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Pérez",
    "nombreEquipo": "Los Cracks",
    "puntos": 25,
    "premioAsignado": 375.00
  }
]
```

---

### Premios globales estimados
```
GET /api/v1/ranking/premios/globales
```
**Acceso:**  JWT + Rol `Administrador`

**Respuesta:**
```json
{
  "totalRecaudadoGlobal": 1500.00,
  "montoGlobalIndividual": 7.50,
  "montoGlobalLiga": 7.50,
  "topIndividuales": [ ... ],
  "mejorLiga": { ... }
}
```

---

## ⚡ SignalR — Tiempo Real

**URL de conexión:** `ws://localhost:8080/hubs/quiniela`

### Instalación (npm)
```bash
npm install @microsoft/signalr
```

### Conexión
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:8080/hubs/quiniela")
  .withAutomaticReconnect()
  .build();

await connection.start();
```

### Suscribirse a un torneo
```javascript
await connection.invoke("UnirseATorneo", 1); // torneoId
```

### Suscribirse a una liga
```javascript
await connection.invoke("UnirseALiga", 1); // ligaId
```

### Escuchar eventos

**Resultado de partido actualizado:**
```javascript
connection.on("ResultadoActualizado", (data) => {
  // data.partidoId, data.golesLocal, data.golesVisitante, data.finalizado
  // data.equipoLocalId, data.equipoVisitanteId
});
```

**Clasificación de grupo actualizada:**
```javascript
connection.on("ClasificacionActualizada", (data) => {
  // data.grupoId
  // data.clasificacion[] → equipoId, puntos, partidosJugados, ganados, etc.
});
```

**Ranking de liga actualizado:**
```javascript
connection.on("RankingActualizado", (data) => {
  // data.ligaId
  // data.ranking[] → posicion, userId, nombreEquipo, puntos
});
```

---


---

##  Notas importantes

- Las **fechas** se manejan en formato **UTC** (`2026-06-11T19:00:00Z`). El frontend debe convertir a la zona horaria local del usuario.
- El **soft delete** está implementado — los registros eliminados no aparecen en las consultas pero se preservan en la base de datos.
- Las **predicciones** solo pueden crearse o editarse si faltan **más de 15 minutos** para el inicio del partido.
- Al **ingresar un resultado**, toda la lógica de puntos, clasificación y bracket se ejecuta automáticamente.
- La **distribución de premios** se calcula en tiempo real pero se distribuye al finalizar el torneo.

