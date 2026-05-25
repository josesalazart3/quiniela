# Quiniela Mundial 2026 — Documentación de API

**Base URL:** `http://localhost:8080`  
**Formato:** JSON  
**Autenticación:** JWT Bearer Token  

Para endpoints protegidos incluir en el header:
```
Authorization: Bearer {token}
```

---

## Roles del sistema

| Rol | Descripción |
|-----|-------------|
| Administrador | Administrador del sistema |
| User | Usuario registrado |

---

## AUTH

### Registrarse
```
POST /api/v1/auth/register
```
Acceso: Publico

Body:
```json
{
  "email": "usuario@example.com",
  "password": "MiPassword123",
  "firstName": "Juan",
  "lastName": "Perez"
}
```

Respuesta:
```json
{
  "message": "Usuario registrado correctamente"
}
```

---

### Iniciar sesion
```
POST /api/v1/auth/login
```
Acceso: Publico

Registra automaticamente la sesion activa con IP y UserAgent.

Body:
```json
{
  "email": "usuario@example.com",
  "password": "MiPassword123"
}
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "usuario@example.com",
  "fullName": "Juan Perez",
  "role": {
    "name": "User"
  }
}
```

---

### Cerrar sesion
```
POST /api/v1/auth/logout
```
Acceso: JWT requerido

Marca la sesion activa como cerrada.

Respuesta:
```json
{
  "message": "Sesion cerrada correctamente"
}
```

---

### Perfil del usuario autenticado
```
GET /api/v1/auth/me
```
Acceso: JWT requerido

Respuesta:
```json
{
  "id": 1,
  "email": "usuario@example.com",
  "firstName": "Juan",
  "lastName": "Perez",
  "fullName": "Juan Perez",
  "role": "User"
}
```

---

### Olvide mi contrasena
```
POST /api/v1/auth/contrasena-olvidada
```
Acceso: Publico

Envia un email con link para recuperar la contrasena. Token expira en 1 hora.
Siempre retorna 200 aunque el email no exista, por seguridad.

Body:
```json
{
  "email": "usuario@example.com"
}
```

Respuesta:
```json
{
  "message": "Si el email existe recibiras un correo con instrucciones"
}
```

---

### Recuperar contrasena
```
POST /api/v1/auth/recuperar-contrasena
```
Acceso: Publico

Body:
```json
{
  "token": "uuid-del-email",
  "newPassword": "NuevoPassword123"
}
```

Respuesta:
```json
{
  "message": "Contrasena actualizada correctamente"
}
```

---

### Resetear contrasena (admin o propio usuario)
```
PUT /api/v1/auth/reset/{id}
```
Acceso: JWT requerido

Body:
```json
{
  "newPassword": "NuevoPassword123"
}
```

---

## USUARIO (gestion global — solo Administrador)

### Listar usuarios
```
GET /api/v1/usuario?page=1&pageSize=10
```
Acceso: JWT + Rol Administrador

Respuesta:
```json
[
  {
    "id": 1,
    "email": "admin@quiniela.com",
    "firstName": "System",
    "lastName": "Admin",
    "fullName": "System Admin",
    "role": "Administrador"
  }
]
```

---

### Listar roles disponibles
```
GET /api/v1/usuario/roles
```
Acceso: JWT + Rol Administrador

Respuesta:
```json
[
  { "id": 1, "name": "Administrador" },
  { "id": 2, "name": "User" }
]
```

---

### Usuario por ID
```
GET /api/v1/usuario/{id}
```
Acceso: JWT + Rol Administrador

---

### Crear usuario
```
POST /api/v1/usuario
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "email": "nuevo@example.com",
  "password": "Test123",
  "firstName": "Nuevo",
  "lastName": "Usuario",
  "roleId": 2
}
```

---

### Editar datos basicos
```
PUT /api/v1/usuario/{id}
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "email": "actualizado@example.com",
  "firstName": "Nombre Actualizado",
  "lastName": "Apellido Actualizado"
}
```

---

### Cambiar rol
```
PUT /api/v1/usuario/{id}/rol
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "roleId": 1
}
```

---

### Resetear contrasena de usuario
```
PUT /api/v1/usuario/{id}/password
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "newPassword": "NuevoPassword123"
}
```

---

### Eliminar usuario (soft delete)
```
DELETE /api/v1/usuario/{id}
```
Acceso: JWT + Rol Administrador

---

## TORNEO

### Listar torneos
```
GET /api/v1/torneo?page=1&pageSize=10
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "id": 1,
    "nombre": "Copa Mundial FIFA 2026",
    "año": 2026,
    "paisSede": "Mexico, USA, Canada",
    "fechaInicio": "2026-06-11T00:00:00Z",
    "fechaFin": "2026-07-19T00:00:00Z",
    "finalizado": false,
    "createdAt": "2025-01-01T00:00:00Z"
  }
]
```

---

### Torneo por ID
```
GET /api/v1/torneo/{id}
```
Acceso: JWT requerido

---

### Torneos para dropdown
```
GET /api/v1/torneo/select
```
Acceso: JWT requerido

Respuesta:
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
Acceso: JWT + Rol Administrador

Body:
```json
{
  "nombre": "Copa Mundial FIFA 2026",
  "año": 2026,
  "paisSede": "Mexico, USA, Canada",
  "fechaInicio": "2026-06-11T00:00:00Z",
  "fechaFin": "2026-07-19T00:00:00Z"
}
```

---

### Actualizar torneo
```
PUT /api/v1/torneo/{id}
```
Acceso: JWT + Rol Administrador

---

### Eliminar torneo
```
DELETE /api/v1/torneo/{id}
```
Acceso: JWT + Rol Administrador

---

## ESTADIO

### Listar estadios
```
GET /api/v1/estadio?page=1&pageSize=20
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "id": 1,
    "nombre": "Estadio Azteca",
    "ciudad": "Ciudad de Mexico",
    "pais": "Mexico",
    "capacidad": 87000
  }
]
```

---

### Estadios para dropdown
```
GET /api/v1/estadio/select
```
Acceso: JWT requerido

Respuesta:
```json
[
  { "id": 1, "nombre": "Estadio Azteca", "ciudad": "Ciudad de Mexico" }
]
```

---

### Estadio por ID
```
GET /api/v1/estadio/{id}
```
Acceso: JWT requerido

---

### Crear estadio
```
POST /api/v1/estadio
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "nombre": "Estadio Azteca",
  "ciudad": "Ciudad de Mexico",
  "pais": "Mexico",
  "capacidad": 87000
}
```

---

### Actualizar estadio
```
PUT /api/v1/estadio/{id}
```
Acceso: JWT + Rol Administrador

---

### Eliminar estadio
```
DELETE /api/v1/estadio/{id}
```
Acceso: JWT + Rol Administrador

---

## EQUIPO

### Listar equipos
```
GET /api/v1/equipo?page=1&pageSize=50
```
Acceso: JWT requerido

Respuesta:
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
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "id": 1,
    "nombre": "Argentina",
    "codigoFifa": "ARG",
    "banderaUrl": "https://flagcdn.com/ar.svg"
  }
]
```

---

### Equipo por ID
```
GET /api/v1/equipo/{id}
```
Acceso: JWT requerido

---

### Crear equipo
```
POST /api/v1/equipo
```
Acceso: JWT + Rol Administrador

Body:
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
Acceso: JWT + Rol Administrador

---

### Eliminar equipo
```
DELETE /api/v1/equipo/{id}
```
Acceso: JWT + Rol Administrador

---

## FASE

### Listar fases
```
GET /api/v1/fase?page=1&pageSize=10
```
Acceso: JWT requerido

Respuesta:
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
Acceso: JWT requerido

---

### Fases para dropdown por torneo
```
GET /api/v1/fase/select/{torneoId}
```
Acceso: JWT requerido

Respuesta:
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
Acceso: JWT requerido

---

### Crear fase
```
POST /api/v1/fase
```
Acceso: JWT + Rol Administrador

Body:
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
Acceso: JWT + Rol Administrador

---

### Eliminar fase
```
DELETE /api/v1/fase/{id}
```
Acceso: JWT + Rol Administrador

---

## GRUPO

### Listar grupos
```
GET /api/v1/grupo?page=1&pageSize=20
```
Acceso: JWT requerido

Respuesta:
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
        "nombre": "Mexico",
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
Acceso: JWT requerido

---

### Grupos para dropdown por torneo
```
GET /api/v1/grupo/select/{torneoId}
```
Acceso: JWT requerido

---

### Grupo por ID
```
GET /api/v1/grupo/{id}
```
Acceso: JWT requerido

---

### Clasificacion de un grupo
```
GET /api/v1/grupo/{grupoId}/clasificacion
```
Acceso: JWT requerido

Ya viene ordenado por posicion (1ro, 2do, 3ro, 4to).

Respuesta:
```json
[
  {
    "equipo": {
      "id": 3,
      "nombre": "Republica de Corea",
      "codigoFifa": "KOR",
      "banderaUrl": "https://flagcdn.com/kr.svg"
    },
    "partidosJugados": 3,
    "ganados": 2,
    "empatados": 1,
    "perdidos": 0,
    "golesAFavor": 4,
    "golesEnContra": 1,
    "diferenciaGoles": 3,
    "puntos": 7
  }
]
```

---

### Mejores terceros lugares
```
GET /api/v1/grupo/torneo/{torneoId}/mejores-terceros
```
Acceso: JWT requerido

Retorna los 12 terceros ordenados de mejor a peor. Los primeros 8 son los que clasifican a dieciseisavos.

Respuesta:
```json
[
  {
    "grupoId": 2,
    "grupoNombre": "Grupo B",
    "equipoId": 8,
    "equipoNombre": "Suiza",
    "banderaUrl": "https://flagcdn.com/ch.svg",
    "puntos": 6,
    "diferenciaGoles": 3,
    "golesAFavor": 5,
    "partidosJugados": 3,
    "posicion": 3
  }
]
```

---

### Crear grupo
```
POST /api/v1/grupo
```
Acceso: JWT + Rol Administrador

Body:
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
Acceso: JWT + Rol Administrador

Crea automaticamente el registro en ClasificacionGrupo con valores en 0.

---

### Asignar varios equipos a un grupo
```
POST /api/v1/grupo/{grupoId}/equipos
```
Acceso: JWT + Rol Administrador

Body:
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
Acceso: JWT + Rol Administrador

---

### Eliminar grupo
```
DELETE /api/v1/grupo/{id}
```
Acceso: JWT + Rol Administrador

---

### Remover equipo de un grupo
```
DELETE /api/v1/grupo/{grupoId}/equipos/{equipoId}
```
Acceso: JWT + Rol Administrador

---

## PARTIDO

### Listar partidos
```
GET /api/v1/partido?page=1&pageSize=20
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "id": 1,
    "torneoId": 1,
    "fase": {
      "id": 1,
      "nombre": "Fase de Grupos",
      "orden": 1,
      "torneoId": 1
    },
    "grupoId": 1,
    "grupoNombre": "Grupo A",
    "equipoLocal": {
      "id": 1,
      "nombre": "Mexico",
      "codigoFifa": "MEX",
      "banderaUrl": "https://flagcdn.com/mx.svg"
    },
    "equipoVisitante": {
      "id": 2,
      "nombre": "Sudafrica",
      "codigoFifa": "RSA",
      "banderaUrl": "https://flagcdn.com/za.svg"
    },
    "descripcionLocal": null,
    "descripcionVisitante": null,
    "fechaHora": "2026-06-11T19:00:00Z",
    "estadio": {
      "id": 1,
      "nombre": "Estadio Azteca",
      "ciudad": "Ciudad de Mexico",
      "pais": "Mexico",
      "capacidad": 87000
    },
    "golesLocal": null,
    "golesVisitante": null,
    "golesLocalPenales": null,
    "golesVisitantePenales": null,
    "finalizado": false
  }
]
```

Nota: En partidos eliminatorios equipoLocal o equipoVisitante pueden ser null.
En ese caso usar descripcionLocal y descripcionVisitante como placeholder.

---

### Partidos por torneo
```
GET /api/v1/partido/torneo/{torneoId}
```
Acceso: JWT requerido

Filtrar por fase.id para organizar el bracket:
- faseId 1 = Fase de Grupos (partidos 1-72)
- faseId 2 = Dieciseisavos de Final (partidos 73-88)
- faseId 3 = Octavos de Final (partidos 89-96)
- faseId 4 = Cuartos de Final (partidos 97-100)
- faseId 5 = Semifinal (partidos 101-102)
- faseId 6 = Tercer Puesto (partido 103)
- faseId 7 = Final (partido 104)

---

### Partidos por fase
```
GET /api/v1/partido/fase/{faseId}
```
Acceso: JWT requerido

---

### Partidos por grupo
```
GET /api/v1/partido/grupo/{grupoId}
```
Acceso: JWT requerido

---

### Partidos pendientes por torneo
```
GET /api/v1/partido/pendientes/{torneoId}
```
Acceso: JWT requerido

---

### Partido por ID
```
GET /api/v1/partido/{id}
```
Acceso: JWT requerido

---

### Crear partido
```
POST /api/v1/partido
```
Acceso: JWT + Rol Administrador

Body (fase de grupos):
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

Body (fase eliminatoria sin equipos definidos):
```json
{
  "torneoId": 1,
  "faseId": 2,
  "grupoId": null,
  "equipoLocalId": null,
  "equipoVisitanteId": null,
  "descripcionLocal": "1 Grupo A",
  "descripcionVisitante": "2 Grupo B",
  "fechaHora": "2026-07-04T19:00:00Z",
  "estadioId": 6
}
```

---

### Actualizar partido y asignar equipos clasificados
```
PUT /api/v1/partido/{id}
```
Acceso: JWT + Rol Administrador

Body:
```json
{
  "equipoLocalId": 3,
  "equipoVisitanteId": null,
  "fechaHora": "2026-07-04T19:00:00Z",
  "estadioId": 6
}
```

---

### Actualizar marcador en vivo (sin finalizar)
```
PUT /api/v1/partido/{id}/marcador
```
Acceso: JWT + Rol Administrador

Solo actualiza los goles y notifica por SignalR. No calcula puntos ni finaliza el partido.
Se puede llamar multiples veces durante el partido.

Body:
```json
{
  "golesLocal": 1,
  "golesVisitante": 0
}
```

---

### Ingresar resultado final
```
PUT /api/v1/partido/{id}/resultado
```
Acceso: JWT + Rol Administrador

Al finalizar se ejecuta automaticamente:
- Actualizacion de ClasificacionGrupo (si es fase de grupos)
- Calculo de puntos en todas las predicciones
- Actualizacion de puntos en LigaMiembro
- Asignacion automatica del ganador al siguiente partido del bracket
- Notificacion SignalR a todos los clientes conectados

Reglas de penales:
- Fase de grupos: nunca hay penales, siempre null
- Fases eliminatorias con empate: penales obligatorios
- Fases eliminatorias sin empate: penales deben ser null

Body sin penales:
```json
{
  "golesLocal": 2,
  "golesVisitante": 1,
  "golesLocalPenales": null,
  "golesVisitantePenales": null
}
```

Body con penales (solo eliminatorias con empate):
```json
{
  "golesLocal": 1,
  "golesVisitante": 1,
  "golesLocalPenales": 5,
  "golesVisitantePenales": 3
}
```

---

### Eliminar partido (soft delete)
```
DELETE /api/v1/partido/{id}
```
Acceso: JWT + Rol Administrador

---

## LIGA

### Listar ligas
```
GET /api/v1/liga?page=1&pageSize=10
```
Acceso: JWT requerido

---

### Mis ligas
```
GET /api/v1/liga/mis-ligas?page=1&pageSize=10
```
Acceso: JWT requerido

---

### Buscar liga por nombre
```
GET /api/v1/liga/buscar?nombre=amigos&page=1&pageSize=10
```
Acceso: JWT requerido

Minimo 2 caracteres.

---

### Liga por ID
```
GET /api/v1/liga/{id}
```
Acceso: JWT requerido

---

### Miembros aprobados de una liga
```
GET /api/v1/liga/{ligaId}/miembros
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "userId": 2,
    "fullName": "Juan Perez",
    "email": "juan@example.com",
    "nombreEquipo": "Los Cracks",
    "esAdmin": true,
    "puntos": 25,
    "estado": "Aprobado",
    "fechaUnion": "2026-01-01T00:00:00Z"
  }
]
```

---

### Miembros pendientes de aprobacion
```
GET /api/v1/liga/{ligaId}/miembros/pendientes
```
Acceso: JWT requerido (solo admin de la liga)

---

### Ranking de la liga con premios estimados
```
GET /api/v1/liga/{ligaId}/ranking
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Perez",
    "nombreEquipo": "Los Cracks",
    "puntos": 25,
    "premioAsignado": 47.50
  }
]
```

---

### Crear liga
```
POST /api/v1/liga
```
Acceso: JWT requerido

Al crear la liga el usuario queda automaticamente como admin y miembro aprobado.

Body (liga de apuestas):
```json
{
  "nombre": "Liga de los Amigos",
  "esDeApuestas": true,
  "precioPorUnirse": 100.00,
  "torneoId": 1,
  "nombreEquipo": "Los Cracks"
}
```

Body (liga de diversion):
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
Acceso: JWT requerido

Queda en estado Pendiente hasta que el admin apruebe.

Body:
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
Acceso: JWT requerido (solo admin de la liga)

Body:
```json
{
  "userId": 3,
  "aprobar": true
}
```

---

### Actualizar liga
```
PUT /api/v1/liga/{id}
```
Acceso: JWT requerido (solo admin de la liga)

---

### Eliminar liga (soft delete)
```
DELETE /api/v1/liga/{id}
```
Acceso: JWT requerido (solo admin de la liga)

---

### Salir de una liga
```
DELETE /api/v1/liga/{ligaId}/salir
```
Acceso: JWT requerido

El admin no puede salir de su propia liga.

---

## INVITACIONES DE LIGA

### Enviar invitacion por email
```
POST /api/v1/invitacionliga/liga/{ligaId}
```
Acceso: JWT requerido (solo admin de la liga)

Envia un email con link que incluye el token. Expira en 7 dias.

Body:
```json
{
  "emailInvitado": "amigo@example.com"
}
```

Respuesta:
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
Acceso: JWT requerido (solo admin de la liga)

---

### Responder invitacion (aceptar o rechazar)
```
POST /api/v1/invitacionliga/responder
```
Acceso: Publico

Si el invitado no esta registrado debe registrarse primero y luego responder con su token.
Al aceptar queda en estado Pendiente hasta que el admin lo apruebe.

Body (aceptar):
```json
{
  "token": "uuid-del-link",
  "aceptar": true,
  "nombreEquipo": "Mi Equipo"
}
```

Body (rechazar):
```json
{
  "token": "uuid-del-link",
  "aceptar": false,
  "nombreEquipo": ""
}
```

---

### Cancelar invitacion
```
DELETE /api/v1/invitacionliga/{invitacionId}
```
Acceso: JWT requerido (solo admin de la liga)

---

## PREDICCIONES

### Predicciones de una liga
```
GET /api/v1/prediccion/liga/{ligaId}?page=1&pageSize=20
```
Acceso: JWT requerido

---

### Mis predicciones en una liga
```
GET /api/v1/prediccion/mis-predicciones/{ligaId}?page=1&pageSize=20
```
Acceso: JWT requerido

Respuesta:
```json
[
  {
    "id": 1,
    "partidoId": 1,
    "ligaId": 1,
    "equipoLocal": "Mexico",
    "equipoVisitante": "Sudafrica",
    "golesLocal": 2,
    "golesVisitante": 1,
    "puntosGanados": 3,
    "createdAt": "2026-06-10T00:00:00Z",
    "updatedAt": null
  }
]
```

---

### Verificar si ya predijo un partido
```
GET /api/v1/prediccion/verificar?partidoId=1&ligaId=1
```
Acceso: JWT requerido

Respuesta:
```json
{
  "existe": true,
  "prediccion": { }
}
```

---

### Prediccion por ID
```
GET /api/v1/prediccion/{id}
```
Acceso: JWT requerido

---

### Crear prediccion
```
POST /api/v1/prediccion
```
Acceso: JWT requerido (miembro aprobado de la liga)

Solo valido si faltan mas de 15 minutos para el inicio del partido.

Body:
```json
{
  "partidoId": 1,
  "ligaId": 1,
  "golesLocal": 2,
  "golesVisitante": 1
}
```

---

### Actualizar prediccion
```
PUT /api/v1/prediccion/{id}
```
Acceso: JWT requerido (solo el dueno)

Solo valido si faltan mas de 15 minutos para el inicio del partido.

Body:
```json
{
  "golesLocal": 3,
  "golesVisitante": 0
}
```

---

## RANKINGS Y PREMIOS

### Ranking global individual
```
GET /api/v1/ranking/global/usuarios
```
Acceso: JWT requerido (cualquier rol)

Usa el puntaje mas alto del usuario entre todas las ligas de apuesta donde participa.

Respuesta:
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Perez",
    "totalPuntos": 25,
    "ligaId": 1,
    "nombreLiga": "Liga de los Amigos",
    "premioAsignado": null
  }
]
```

---

### Ranking global de ligas
```
GET /api/v1/ranking/global/ligas
```
Acceso: JWT requerido (cualquier rol)

Solo aparecen ligas de apuesta, ordenadas por promedio de puntos.

Respuesta:
```json
[
  {
    "posicion": 1,
    "ligaId": 1,
    "nombreLiga": "Liga de los Amigos",
    "promedioPuntos": 18.50,
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
Acceso: JWT requerido (cualquier rol)

Respuesta:
```json
[
  {
    "posicion": 1,
    "userId": 2,
    "fullName": "Juan Perez",
    "nombreEquipo": "Los Cracks",
    "puntos": 25,
    "premioAsignado": 47.50
  }
]
```

---

### Premios globales estimados
```
GET /api/v1/ranking/premios/globales
```
Acceso: JWT + Rol Administrador

Respuesta:
```json
{
  "totalRecaudadoGlobal": 500.00,
  "montoGlobalIndividual": 2.50,
  "montoGlobalLiga": 2.50,
  "topIndividuales": [
    {
      "posicion": 1,
      "userId": 2,
      "fullName": "Juan Perez",
      "totalPuntos": 25,
      "ligaId": 1,
      "nombreLiga": "Liga de los Amigos",
      "premioAsignado": 1.25
    }
  ],
  "mejorLiga": {
    "posicion": 1,
    "ligaId": 1,
    "nombreLiga": "Liga de los Amigos",
    "promedioPuntos": 18.50,
    "totalMiembros": 4,
    "premioTotal": 2.50,
    "premioPerCapita": 0.625
  }
}
```

---

### Cerrar torneo y distribuir premios
```
POST /api/v1/ranking/premios/globales/cerrar/{torneoId}
```
Acceso: JWT + Rol Administrador

Solo se puede ejecutar una vez. Marca el torneo como finalizado y registra todos los premios definitivamente.

---

### Historial de premios distribuidos
```
GET /api/v1/ranking/premios/globales/historial/{torneoId}
```
Acceso: JWT + Rol Administrador

Respuesta:
```json
[
  {
    "id": 1,
    "torneoId": 1,
    "userId": 2,
    "fullName": "Juan Perez",
    "ligaId": 1,
    "nombreLiga": "Liga de los Amigos",
    "concepto": "1 lugar — Liga de los Amigos",
    "monto": 47.50,
    "posicion": 1,
    "fechaDistribucion": "2026-07-19T20:00:00Z"
  }
]
```

---

## REPORTES (solo Administrador)

### Resumen del dashboard
```
GET /api/v1/reporte/resumen
```
Acceso: JWT + Rol Administrador

Respuesta:
```json
{
  "totalUsuarios": 45,
  "totalLigas": 12,
  "totalLigasApuesta": 8,
  "totalLigasDiversion": 4,
  "totalPredicciones": 320,
  "partidosJugados": 24,
  "partidosPendientes": 48,
  "invitacionesPendientes": 5,
  "totalRecaudado": 1200.00,
  "totalSesionesActivas": 8
}
```

---

### Descargar reporte de usuarios (CSV)
```
GET /api/v1/reporte/usuarios/descargar
```
Acceso: JWT + Rol Administrador

Devuelve un archivo .csv descargable con todos los usuarios y su actividad.

---

### Descargar reporte de ligas (CSV)
```
GET /api/v1/reporte/ligas/descargar
```
Acceso: JWT + Rol Administrador

---

### Descargar predicciones de una liga (CSV)
```
GET /api/v1/reporte/predicciones/descargar/{ligaId}
```
Acceso: JWT + Rol Administrador

---

### Descargar premios distribuidos de un torneo (CSV)
```
GET /api/v1/reporte/premios/descargar/{torneoId}
```
Acceso: JWT + Rol Administrador

---

## SIGNALR — TIEMPO REAL

URL de conexion: `ws://localhost:8080/hubs/quiniela`

Instalacion:
```bash
npm install @microsoft/signalr
```

Conexion:
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:8080/hubs/quiniela")
  .withAutomaticReconnect()
  .build();

await connection.start();
```

Suscribirse a un torneo:
```javascript
await connection.invoke("UnirseATorneo", 1);
```

Suscribirse a una liga:
```javascript
await connection.invoke("UnirseALiga", 1);
```

Salir de una liga (llamar en ngOnDestroy):
```javascript
await connection.invoke("SalirDeLiga", 1);
```

Eventos disponibles:

ResultadoActualizado — cuando el admin actualiza el marcador o finaliza un partido:
```javascript
connection.on("ResultadoActualizado", (data) => {
  // data.partidoId
  // data.golesLocal
  // data.golesVisitante
  // data.finalizado
  // data.equipoLocalId
  // data.equipoVisitanteId
});
```

ClasificacionActualizada — cuando termina un partido de grupos:
```javascript
connection.on("ClasificacionActualizada", (data) => {
  // data.grupoId
  // data.clasificacion[] con equipoId, puntos, partidosJugados, ganados, etc.
});
```

RankingActualizado — cuando cambian los puntos de una liga:
```javascript
connection.on("RankingActualizado", (data) => {
  // data.ligaId
  // data.ranking[] con posicion, userId, nombreEquipo, puntos
});
```

---

## CODIGOS DE ERROR

| Codigo | Descripcion |
|--------|-------------|
| 400 | Bad Request — datos invalidos o regla de negocio violada |
| 401 | Unauthorized — token no enviado o invalido |
| 403 | Forbidden — sin permisos para esta accion |
| 404 | Not Found — recurso no encontrado |
| 500 | Internal Server Error — error inesperado del servidor |

Formato de error:
```json
{
  "error": "Mensaje descriptivo del error"
}
```

---

## NOTAS IMPORTANTES

- Las fechas se manejan en formato UTC. El frontend debe convertir a la zona horaria local del usuario. Guatemala es UTC-6.
- El soft delete esta implementado — los registros eliminados no aparecen en las consultas pero se preservan en la base de datos.
- Las predicciones solo pueden crearse o editarse si faltan mas de 15 minutos para el inicio del partido.
- Al ingresar un resultado toda la logica de puntos, clasificacion y bracket se ejecuta automaticamente.
- El cierre de torneo distribuye los premios de forma definitiva y solo puede ejecutarse una vez.
- Los CSV descargables vienen con BOM UTF-8 para que Excel detecte tildes y caracteres especiales correctamente.
- La bitacora de auditoria se llena automaticamente mediante triggers de PostgreSQL en las tablas Users, Ligas, Predicciones y Partidos.
- Para sincronizar las secuencias de la BD despues de cargar seed data ejecutar en DBeaver:
```sql
SELECT setval(pg_get_serial_sequence('"Equipos"', 'Id'), MAX("Id")) FROM "Equipos";
SELECT setval(pg_get_serial_sequence('"Estadios"', 'Id'), MAX("Id")) FROM "Estadios";
SELECT setval(pg_get_serial_sequence('"Partidos"', 'Id'), MAX("Id")) FROM "Partidos";
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), MAX("Id")) FROM "Users";
```
