# Quiniela Mundial 2026 — Backend API

Sistema de quinielas para la Copa Mundial FIFA 2026. Permite crear ligas de apuesta o diversion, realizar predicciones de partidos, ver rankings en tiempo real y distribuir premios automaticamente al cierre del torneo.

El frontend de este proyecto fue desarrollado en Angular y se encuentra en un repositorio separado:
[https://github.com/DanielRevolorio1107/Frontend-Quiniela.git](https://github.com/DanielRevolorio1107/Frontend-Quiniela.git)

---

## Stack tecnologico

| Tecnologia | Version | Uso |
|---|---|---|
| .NET | 8.0 | Framework principal |
| C# | 12 | Lenguaje de programacion |
| PostgreSQL | 16 | Base de datos |
| Entity Framework Core | 8 | ORM |
| SignalR | - | Comunicacion en tiempo real |
| JWT | - | Autenticacion |
| BCrypt | - | Hash de contrasenas |
| MailKit | - | Envio de emails |
| Docker | - | Contenedores |

---

## Arquitectura

El proyecto sigue el patron **Repository + Service + Controller** con las siguientes capas:

```
Controllers   → reciben la request HTTP y retornan la response
Services      → logica de negocio
Repositories  → acceso a datos con Entity Framework Core
Models        → entidades de la base de datos
DTOs          → datos que entran y salen de la API
Hubs          → comunicacion en tiempo real con SignalR
Middlewares   → manejo global de errores
Utils         → helpers (CryptoHelper, CsvHelper)
```

---

## Estructura del proyecto

```
Quiniela/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsuarioController.cs
│   ├── TorneoController.cs
│   ├── EstadioController.cs
│   ├── EquipoController.cs
│   ├── FaseController.cs
│   ├── GrupoController.cs
│   ├── PartidoController.cs
│   ├── LigaController.cs
│   ├── InvitacionLigaController.cs
│   ├── PrediccionController.cs
│   ├── RankingController.cs
│   ├── ReporteController.cs
│   └── AuditoriaController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DatosMundial.cs         ← seed data con 48 equipos, 16 estadios y 104 partidos
├── Hubs/
│   └── QuinielaHub.cs
├── Middlewares/
│   └── ExceptionMiddleware.cs
├── Migrations/
├── Models/
│   ├── DTOs/
│   └── *.cs                    ← entidades de la base de datos
├── Repositories/
│   ├── Interfaces/
│   └── *.cs
├── Services/
│   ├── Interfaces/
│   ├── BracketService.cs       ← asignacion automatica del bracket
│   ├── NotificacionService.cs  ← SignalR
│   └── *.cs
├── Utils/
│   ├── CryptoHelper.cs
│   └── CsvHelper.cs
├── .env.example                ← template de variables de entorno
├── appsettings.example.json    ← template de configuracion
├── docker-compose.yml
├── Dockerfile
└── Program.cs
```

---

## Requisitos previos

- [Docker](https://www.docker.com/) y Docker Compose
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) (solo para desarrollo local sin Docker)
- [DBeaver](https://dbeaver.io/) u otro cliente de PostgreSQL (opcional, para administrar la BD)

---

## Instalacion y ejecucion con Docker

### 1. Clonar el repositorio

```bash
git clone https://github.com/rramirezg18/quiniela.git
cd quiniela
```

### 2. Configurar variables de entorno

Copiar el archivo de ejemplo y completar los valores:

```bash
cp .env.example .env
```

Editar el archivo `.env` con los valores correspondientes:

```
DB_HOST=localhost
DB_PORT=5433
DB_NAME=quiniela
DB_USER=quiniela
DB_PASSWORD=tu_password

JWT_KEY=tu_jwt_key_minimo_32_caracteres
JWT_ISSUER=QuinielaAPI
JWT_AUDIENCE=QuinielaAPI
JWT_EXPIRES_IN_MINUTES=60

EMAIL_SMTP_HOST=smtp.gmail.com
EMAIL_SMTP_PORT=587
EMAIL_FROM=tu_correo@gmail.com
EMAIL_FROM_NAME=Quiniela Mundial 2026
EMAIL_PASSWORD=tu_app_password_gmail

CRYPTO_KEY=tu_key_32_caracteres_exactos____
CRYPTO_IV=tu_iv_16_chars___
```

### 3. Configurar appsettings.json

```bash
cp appsettings.example.json appsettings.json
```

Editar `appsettings.json` con los mismos valores del `.env`.

### 4. Levantar los contenedores

```bash
sudo docker compose up -d --build
```

Esto levanta dos contenedores:
- `postgres_quiniela` — base de datos PostgreSQL en el puerto 5433
- `api_quiniela` — API en el puerto 8080

La API aplica migraciones y carga el seed data automaticamente al iniciar.

### 5. Verificar que funciona

```bash
docker logs api_quiniela --tail=20
```

El API estara disponible en: `http://localhost:8080`

Swagger UI: `http://localhost:8080/swagger`

---

## Sincronizar secuencias de la base de datos

Despues de la primera ejecucion, ejecutar estas consultas en DBeaver para sincronizar las secuencias de autoincremento con el seed data:

```sql
SELECT setval(pg_get_serial_sequence('"Equipos"', 'Id'), MAX("Id")) FROM "Equipos";
SELECT setval(pg_get_serial_sequence('"Estadios"', 'Id'), MAX("Id")) FROM "Estadios";
SELECT setval(pg_get_serial_sequence('"Torneos"', 'Id'), MAX("Id")) FROM "Torneos";
SELECT setval(pg_get_serial_sequence('"Fases"', 'Id'), MAX("Id")) FROM "Fases";
SELECT setval(pg_get_serial_sequence('"Grupos"', 'Id'), MAX("Id")) FROM "Grupos";
SELECT setval(pg_get_serial_sequence('"Partidos"', 'Id'), MAX("Id")) FROM "Partidos";
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), MAX("Id")) FROM "Users";
```

---

## Credenciales del administrador por defecto

```
Email:    admin@quiniela.com
Password: quiniela
```

---

## Instalacion local sin Docker

### 1. Instalar dependencias

```bash
dotnet restore
```

### 2. Configurar la cadena de conexion

En `appsettings.json` asegurarse que `ConnectionStrings:DefaultConnection` apunte a una instancia local de PostgreSQL.

### 3. Aplicar migraciones

```bash
dotnet ef database update
```

### 4. Ejecutar el proyecto

```bash
dotnet run
```

---

## Modulos del sistema

| Modulo | Descripcion |
|---|---|
| M1 — Autenticacion | Registro, login con JWT, recuperacion de contrasena, sesiones activas |
| M2 — Gestion de Ligas | Crear ligas de apuesta o diversion, invitaciones por email, aprobacion de miembros |
| M3 — Predicciones | Vaticinar resultados de partidos con limite de 15 minutos antes del inicio |
| M4 — Tiempo Real | Rankings y marcadores actualizados en tiempo real con SignalR |
| M5 — Administracion del Mundial | CRUD de equipos, estadios, grupos, partidos, ingreso de resultados, bracket automatico |
| M6 — Premios | Calculo automatico de premios con reglas de empate, distribucion del 1% global |
| M7 — Panel Admin | Gestion de usuarios, reportes descargables en CSV, dashboard de metricas |

---

## Funcionalidades destacadas

- Bracket automatico — al ingresar el resultado de un partido, el ganador avanza automaticamente a la siguiente ronda
- Penales — soporte para desempate por penales en fases eliminatorias
- Soft delete — los registros eliminados se preservan en la base de datos
- Bitacora de auditoria — triggers en PostgreSQL registran todos los cambios en tablas criticas
- Reportes CSV — descarga de reportes de usuarios, ligas, predicciones y premios con BOM UTF-8 para Excel
- Emails automaticos — notificaciones de invitaciones y recuperacion de contrasena via Gmail SMTP

---

## Variables de entorno

| Variable | Descripcion |
|---|---|
| `DB_HOST` | Host de la base de datos |
| `DB_PORT` | Puerto de la base de datos |
| `DB_NAME` | Nombre de la base de datos |
| `DB_USER` | Usuario de la base de datos |
| `DB_PASSWORD` | Contrasena de la base de datos |
| `JWT_KEY` | Clave secreta para firmar los tokens JWT (minimo 32 caracteres) |
| `JWT_ISSUER` | Issuer del token JWT |
| `JWT_AUDIENCE` | Audience del token JWT |
| `JWT_EXPIRES_IN_MINUTES` | Tiempo de expiracion del token en minutos |
| `EMAIL_SMTP_HOST` | Host del servidor SMTP |
| `EMAIL_SMTP_PORT` | Puerto del servidor SMTP |
| `EMAIL_FROM` | Correo electronico del remitente |
| `EMAIL_FROM_NAME` | Nombre del remitente |
| `EMAIL_PASSWORD` | Contrasena de aplicacion del correo (Gmail App Password) |
| `CRYPTO_KEY` | Clave para encriptar IDs en JWT (32 caracteres exactos) |
| `CRYPTO_IV` | Vector de inicializacion para encriptacion (16 caracteres exactos) |

---

## Comandos utiles

```bash
# Ver logs del API
docker logs api_quiniela --tail=50

# Detener los contenedores
sudo docker compose down

# Reconstruir y levantar
sudo docker compose up -d --build

# Aplicar una nueva migracion
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

---

## Repositorios

| Repositorio | URL |
|---|---|
| Backend  | https://github.com/rramirezg18/quiniela.git |
| Frontend (Angular) | https://github.com/DanielRevolorio1107/Frontend-Quiniela.git |
