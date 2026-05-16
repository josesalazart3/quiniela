using Microsoft.EntityFrameworkCore;
using Quiniela.Models;
using Quiniela.Enums;

namespace Quiniela.Data
{
    public static class DatosMundial
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Torneos.AnyAsync()) return;

            await DatosTorneoYEstadiosAsync(context);
            await DatosEquiposAsync(context);
            await DatosFasesYGruposAsync(context);
            await DatosGrupoEquiposAsync(context);
            await DatosPartidosFaseGruposAsync(context);
            //await DatosPartidosFaseEliminatoriaAsync(context);
        }

        private static async Task DatosTorneoYEstadiosAsync(AppDbContext context)
        {
            var torneo = new Torneo
            {
                Id = 1,
                Nombre = "Copa Mundial FIFA 2026",
                Año = 2026,
                PaisSede = "México, USA, Canadá",
                FechaInicio = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc),
                FechaFin = new DateTime(2026, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            context.Torneos.Add(torneo);
            await context.SaveChangesAsync();

            var estadios = new List<Estadio>
            {
                new() { Id = 1,  Nombre = "Estadio Azteca",          Ciudad = "Ciudad de México", Pais = "México",         Capacidad = 87000 },
                new() { Id = 2,  Nombre = "Estadio Akron",           Ciudad = "Guadalajara",      Pais = "México",         Capacidad = 49850 },
                new() { Id = 3,  Nombre = "Estadio BBVA",            Ciudad = "Monterrey",        Pais = "México",         Capacidad = 51000 },
                new() { Id = 4,  Nombre = "BMO Field",               Ciudad = "Toronto",          Pais = "Canadá",         Capacidad = 45000 },
                new() { Id = 5,  Nombre = "BC Place",                Ciudad = "Vancouver",        Pais = "Canadá",         Capacidad = 54500 },
                new() { Id = 6,  Nombre = "SoFi Stadium",            Ciudad = "Los Ángeles",      Pais = "Estados Unidos", Capacidad = 70240 },
                new() { Id = 7,  Nombre = "Levi's Stadium",          Ciudad = "Bahía de San Francisco", Pais = "Estados Unidos", Capacidad = 68500 },
                new() { Id = 8,  Nombre = "MetLife Stadium",         Ciudad = "Nueva York",       Pais = "Estados Unidos", Capacidad = 82500 },
                new() { Id = 9,  Nombre = "Gillette Stadium",        Ciudad = "Boston",           Pais = "Estados Unidos", Capacidad = 65878 },
                new() { Id = 10, Nombre = "NRG Stadium",             Ciudad = "Houston",          Pais = "Estados Unidos", Capacidad = 72000 },
                new() { Id = 11, Nombre = "AT&T Stadium",            Ciudad = "Dallas",           Pais = "Estados Unidos", Capacidad = 80000 },
                new() { Id = 12, Nombre = "Lincoln Financial Field", Ciudad = "Filadelfia",       Pais = "Estados Unidos", Capacidad = 67000 },
                new() { Id = 13, Nombre = "Mercedes-Benz Stadium",   Ciudad = "Atlanta",          Pais = "Estados Unidos", Capacidad = 71000 },
                new() { Id = 14, Nombre = "Lumen Field",             Ciudad = "Seattle",          Pais = "Estados Unidos", Capacidad = 68740 },
                new() { Id = 15, Nombre = "Hard Rock Stadium",       Ciudad = "Miami",            Pais = "Estados Unidos", Capacidad = 65000 },
                new() { Id = 16, Nombre = "Arrowhead Stadium",       Ciudad = "Kansas City",      Pais = "Estados Unidos", Capacidad = 76000 },
            };
            context.Estadios.AddRange(estadios);
            await context.SaveChangesAsync();
        }
        private static async Task DatosEquiposAsync(AppDbContext context)
        {
            var equipos = new List<Equipo>
            {
                // Grupo A
                new() { Id = 1,  Nombre = "México",               CodigoFifa = "MEX", BanderaUrl = "https://flagcdn.com/mx.svg", Entrenador = "Javier Aguirre",      Capitan = "Hirving Lozano" },
                new() { Id = 2,  Nombre = "Sudáfrica",            CodigoFifa = "RSA", BanderaUrl = "https://flagcdn.com/za.svg", Entrenador = "Hugo Broos",          Capitan = "Ronwen Williams" },
                new() { Id = 3,  Nombre = "República de Corea",   CodigoFifa = "KOR", BanderaUrl = "https://flagcdn.com/kr.svg", Entrenador = "Hong Myung-bo",       Capitan = "Son Heung-min" },
                new() { Id = 4,  Nombre = "República Checa",      CodigoFifa = "CZE", BanderaUrl = "https://flagcdn.com/cz.svg", Entrenador = "Ivan Hašek",          Capitan = "Tomáš Souček" },

                // Grupo B
                new() { Id = 5,  Nombre = "Canadá",               CodigoFifa = "CAN", BanderaUrl = "https://flagcdn.com/ca.svg", Entrenador = "Jesse Marsch",        Capitan = "Alphonso Davies" },
                new() { Id = 6,  Nombre = "Bosnia y Herzegovina", CodigoFifa = "BIH", BanderaUrl = "https://flagcdn.com/ba.svg", Entrenador = "Sergej Barbarez",     Capitan = "Edin Džeko" },
                new() { Id = 7,  Nombre = "Catar",                CodigoFifa = "QAT", BanderaUrl = "https://flagcdn.com/qa.svg", Entrenador = "Marquez López",       Capitan = "Hassan Al-Haydos" },
                new() { Id = 8,  Nombre = "Suiza",                CodigoFifa = "SUI", BanderaUrl = "https://flagcdn.com/ch.svg", Entrenador = "Murat Yakin",         Capitan = "Granit Xhaka" },

                // Grupo C
                new() { Id = 9,  Nombre = "Brasil",               CodigoFifa = "BRA", BanderaUrl = "https://flagcdn.com/br.svg", Entrenador = "Dorival Júnior",      Capitan = "Vinícius Jr." },
                new() { Id = 10, Nombre = "Marruecos",            CodigoFifa = "MAR", BanderaUrl = "https://flagcdn.com/ma.svg", Entrenador = "Walid Regragui",      Capitan = "Romain Saïss" },
                new() { Id = 11, Nombre = "Haití",                CodigoFifa = "HAI", BanderaUrl = "https://flagcdn.com/ht.svg", Entrenador = "Marc Collat",         Capitan = "Duckens Nazon" },
                new() { Id = 12, Nombre = "Escocia",              CodigoFifa = "SCO", BanderaUrl = "https://flagcdn.com/gb-sct.svg", Entrenador = "Steve Clarke",    Capitan = "Andy Robertson" },

                // Grupo D
                new() { Id = 13, Nombre = "Estados Unidos",       CodigoFifa = "USA", BanderaUrl = "https://flagcdn.com/us.svg", Entrenador = "Mauricio Pochettino", Capitan = "Christian Pulisic" },
                new() { Id = 14, Nombre = "Paraguay",             CodigoFifa = "PAR", BanderaUrl = "https://flagcdn.com/py.svg", Entrenador = "Gustavo Alfaro",      Capitan = "Miguel Almirón" },
                new() { Id = 15, Nombre = "Australia",            CodigoFifa = "AUS", BanderaUrl = "https://flagcdn.com/au.svg", Entrenador = "Tony Popovic",        Capitan = "Mathew Ryan" },
                new() { Id = 16, Nombre = "Turquía",              CodigoFifa = "TUR", BanderaUrl = "https://flagcdn.com/tr.svg", Entrenador = "Vincenzo Montella",   Capitan = "Hakan Çalhanoğlu" },

                // Grupo E
                new() { Id = 17, Nombre = "Alemania",             CodigoFifa = "GER", BanderaUrl = "https://flagcdn.com/de.svg", Entrenador = "Julian Nagelsmann",   Capitan = "Manuel Neuer" },
                new() { Id = 18, Nombre = "Curazao",              CodigoFifa = "CUW", BanderaUrl = "https://flagcdn.com/cw.svg", Entrenador = "Alexwander Denge",    Capitan = "Leandro Bacuna" },
                new() { Id = 19, Nombre = "Costa de Marfil",      CodigoFifa = "CIV", BanderaUrl = "https://flagcdn.com/ci.svg", Entrenador = "Emerse Faé",          Capitan = "Sébastien Haller" },
                new() { Id = 20, Nombre = "Ecuador",              CodigoFifa = "ECU", BanderaUrl = "https://flagcdn.com/ec.svg", Entrenador = "Sebastián Beccacece", Capitan = "Enner Valencia" },

                // Grupo F
                new() { Id = 21, Nombre = "Países Bajos",         CodigoFifa = "NED", BanderaUrl = "https://flagcdn.com/nl.svg", Entrenador = "Ronald Koeman",       Capitan = "Virgil van Dijk" },
                new() { Id = 22, Nombre = "Japón",                CodigoFifa = "JPN", BanderaUrl = "https://flagcdn.com/jp.svg", Entrenador = "Hajime Moriyasu",     Capitan = "Maya Yoshida" },
                new() { Id = 23, Nombre = "Suecia",               CodigoFifa = "SWE", BanderaUrl = "https://flagcdn.com/se.svg", Entrenador = "Jon Dahl Tomasson",   Capitan = "Victor Nilsson Lindelöf" },
                new() { Id = 24, Nombre = "Túnez",                CodigoFifa = "TUN", BanderaUrl = "https://flagcdn.com/tn.svg", Entrenador = "Faouzi Benzarti",     Capitan = "Yassine Meriah" },

                // Grupo G
                new() { Id = 25, Nombre = "Bélgica",              CodigoFifa = "BEL", BanderaUrl = "https://flagcdn.com/be.svg", Entrenador = "Domenico Tedesco",    Capitan = "Kevin De Bruyne" },
                new() { Id = 26, Nombre = "Egipto",               CodigoFifa = "EGY", BanderaUrl = "https://flagcdn.com/eg.svg", Entrenador = "Hossam Hassan",       Capitan = "Mohamed Salah" },
                new() { Id = 27, Nombre = "RI de Irán",           CodigoFifa = "IRN", BanderaUrl = "https://flagcdn.com/ir.svg", Entrenador = "Amir Ghalenoei",      Capitan = "Ehsan Hajsafi" },
                new() { Id = 28, Nombre = "Nueva Zelanda",        CodigoFifa = "NZL", BanderaUrl = "https://flagcdn.com/nz.svg", Entrenador = "Darren Bazeley",      Capitan = "Tommy Smith" },

                // Grupo H
                new() { Id = 29, Nombre = "España",               CodigoFifa = "ESP", BanderaUrl = "https://flagcdn.com/es.svg", Entrenador = "Luis de la Fuente",   Capitan = "Álvaro Morata" },
                new() { Id = 30, Nombre = "Cabo Verde",           CodigoFifa = "CPV", BanderaUrl = "https://flagcdn.com/cv.svg", Entrenador = "Bubista",             Capitan = "Stopira" },
                new() { Id = 31, Nombre = "Arabia Saudí",         CodigoFifa = "KSA", BanderaUrl = "https://flagcdn.com/sa.svg", Entrenador = "Hervé Renard",        Capitan = "Salem Al-Dawsari" },
                new() { Id = 32, Nombre = "Uruguay",              CodigoFifa = "URU", BanderaUrl = "https://flagcdn.com/uy.svg", Entrenador = "Marcelo Bielsa",      Capitan = "Federico Valverde" },

                // Grupo I
                new() { Id = 33, Nombre = "Francia",              CodigoFifa = "FRA", BanderaUrl = "https://flagcdn.com/fr.svg", Entrenador = "Didier Deschamps",    Capitan = "Kylian Mbappé" },
                new() { Id = 34, Nombre = "Senegal",              CodigoFifa = "SEN", BanderaUrl = "https://flagcdn.com/sn.svg", Entrenador = "Aliou Cissé",         Capitan = "Kalidou Koulibaly" },
                new() { Id = 35, Nombre = "Irak",                 CodigoFifa = "IRQ", BanderaUrl = "https://flagcdn.com/iq.svg", Entrenador = "Jesús Casas",         Capitan = "Ali Adnan" },
                new() { Id = 36, Nombre = "Noruega",              CodigoFifa = "NOR", BanderaUrl = "https://flagcdn.com/no.svg", Entrenador = "Ståle Solbakken",     Capitan = "Erling Haaland" },

                // Grupo J
                new() { Id = 37, Nombre = "Argentina",            CodigoFifa = "ARG", BanderaUrl = "https://flagcdn.com/ar.svg", Entrenador = "Lionel Scaloni",      Capitan = "Lionel Messi" },
                new() { Id = 38, Nombre = "Argelia",              CodigoFifa = "ALG", BanderaUrl = "https://flagcdn.com/dz.svg", Entrenador = "Vladimir Petković",   Capitan = "Riyad Mahrez" },
                new() { Id = 39, Nombre = "Austria",              CodigoFifa = "AUT", BanderaUrl = "https://flagcdn.com/at.svg", Entrenador = "Ralf Rangnick",       Capitan = "David Alaba" },
                new() { Id = 40, Nombre = "Jordania",             CodigoFifa = "JOR", BanderaUrl = "https://flagcdn.com/jo.svg", Entrenador = "Hussain Ammouta",     Capitan = "Baha' Faisal" },

                // Grupo K
                new() { Id = 41, Nombre = "Portugal",             CodigoFifa = "POR", BanderaUrl = "https://flagcdn.com/pt.svg", Entrenador = "Roberto Martínez",    Capitan = "Cristiano Ronaldo" },
                new() { Id = 42, Nombre = "RD Congo",             CodigoFifa = "COD", BanderaUrl = "https://flagcdn.com/cd.svg", Entrenador = "Sébastien Desabre",   Capitan = "Chancel Mbemba" },
                new() { Id = 43, Nombre = "Uzbekistán",           CodigoFifa = "UZB", BanderaUrl = "https://flagcdn.com/uz.svg", Entrenador = "Srecko Katanec",      Capitan = "Eldor Shomurodov" },
                new() { Id = 44, Nombre = "Colombia",             CodigoFifa = "COL", BanderaUrl = "https://flagcdn.com/co.svg", Entrenador = "Néstor Lorenzo",      Capitan = "James Rodríguez" },

                // Grupo L
                new() { Id = 45, Nombre = "Inglaterra",           CodigoFifa = "ENG", BanderaUrl = "https://flagcdn.com/gb-eng.svg", Entrenador = "Thomas Tuchel",   Capitan = "Harry Kane" },
                new() { Id = 46, Nombre = "Croacia",              CodigoFifa = "CRO", BanderaUrl = "https://flagcdn.com/hr.svg", Entrenador = "Zlatko Dalić",        Capitan = "Luka Modrić" },
                new() { Id = 47, Nombre = "Ghana",                CodigoFifa = "GHA", BanderaUrl = "https://flagcdn.com/gh.svg", Entrenador = "Otto Addo",           Capitan = "Thomas Partey" },
                new() { Id = 48, Nombre = "Panamá",               CodigoFifa = "PAN", BanderaUrl = "https://flagcdn.com/pa.svg", Entrenador = "Thomas Christiansen", Capitan = "Román Torres" },
            };
            context.Equipos.AddRange(equipos);
            await context.SaveChangesAsync();
        }

        private static async Task DatosFasesYGruposAsync(AppDbContext context)
        {
            var fases = new List<Fase>
            {
                new() { Id = 1, Nombre = "Fase de Grupos",          Orden = 1, TorneoId = 1 },
                new() { Id = 2, Nombre = "Dieciseisavos de Final",  Orden = 2, TorneoId = 1 },
                new() { Id = 3, Nombre = "Octavos de Final",        Orden = 3, TorneoId = 1 },
                new() { Id = 4, Nombre = "Cuartos de Final",        Orden = 4, TorneoId = 1 },
                new() { Id = 5, Nombre = "Semifinal",               Orden = 5, TorneoId = 1 },
                new() { Id = 6, Nombre = "Tercer Puesto",           Orden = 6, TorneoId = 1 },
                new() { Id = 7, Nombre = "Final",                   Orden = 7, TorneoId = 1 },
            };
            context.Fases.AddRange(fases);

            var grupos = new List<Grupo>
            {
                new() { Id = 1,  Nombre = "Grupo A", TorneoId = 1 },
                new() { Id = 2,  Nombre = "Grupo B", TorneoId = 1 },
                new() { Id = 3,  Nombre = "Grupo C", TorneoId = 1 },
                new() { Id = 4,  Nombre = "Grupo D", TorneoId = 1 },
                new() { Id = 5,  Nombre = "Grupo E", TorneoId = 1 },
                new() { Id = 6,  Nombre = "Grupo F", TorneoId = 1 },
                new() { Id = 7,  Nombre = "Grupo G", TorneoId = 1 },
                new() { Id = 8,  Nombre = "Grupo H", TorneoId = 1 },
                new() { Id = 9,  Nombre = "Grupo I", TorneoId = 1 },
                new() { Id = 10, Nombre = "Grupo J", TorneoId = 1 },
                new() { Id = 11, Nombre = "Grupo K", TorneoId = 1 },
                new() { Id = 12, Nombre = "Grupo L", TorneoId = 1 },
            };
            context.Grupos.AddRange(grupos);
            await context.SaveChangesAsync();
        }

        private static async Task DatosGrupoEquiposAsync(AppDbContext context)
        {
            var grupoEquipos = new List<GrupoEquipo>
            {
                // Grupo A: México(1), Sudáfrica(2), República de Corea(3), República Checa(4)
                new() { GrupoId = 1, EquipoId = 1 },
                new() { GrupoId = 1, EquipoId = 2 },
                new() { GrupoId = 1, EquipoId = 3 },
                new() { GrupoId = 1, EquipoId = 4 },

                // Grupo B: Canadá(5), Bosnia(6), Catar(7), Suiza(8)
                new() { GrupoId = 2, EquipoId = 5 },
                new() { GrupoId = 2, EquipoId = 6 },
                new() { GrupoId = 2, EquipoId = 7 },
                new() { GrupoId = 2, EquipoId = 8 },

                // Grupo C: Brasil(9), Marruecos(10), Haití(11), Escocia(12)
                new() { GrupoId = 3, EquipoId = 9  },
                new() { GrupoId = 3, EquipoId = 10 },
                new() { GrupoId = 3, EquipoId = 11 },
                new() { GrupoId = 3, EquipoId = 12 },

                // Grupo D: USA(13), Paraguay(14), Australia(15), Turquía(16)
                new() { GrupoId = 4, EquipoId = 13 },
                new() { GrupoId = 4, EquipoId = 14 },
                new() { GrupoId = 4, EquipoId = 15 },
                new() { GrupoId = 4, EquipoId = 16 },

                // Grupo E: Alemania(17), Curazao(18), Costa de Marfil(19), Ecuador(20)
                new() { GrupoId = 5, EquipoId = 17 },
                new() { GrupoId = 5, EquipoId = 18 },
                new() { GrupoId = 5, EquipoId = 19 },
                new() { GrupoId = 5, EquipoId = 20 },

                // Grupo F: Países Bajos(21), Japón(22), Suecia(23), Túnez(24)
                new() { GrupoId = 6, EquipoId = 21 },
                new() { GrupoId = 6, EquipoId = 22 },
                new() { GrupoId = 6, EquipoId = 23 },
                new() { GrupoId = 6, EquipoId = 24 },

                // Grupo G: Bélgica(25), Egipto(26), Irán(27), Nueva Zelanda(28)
                new() { GrupoId = 7, EquipoId = 25 },
                new() { GrupoId = 7, EquipoId = 26 },
                new() { GrupoId = 7, EquipoId = 27 },
                new() { GrupoId = 7, EquipoId = 28 },

                // Grupo H: España(29), Cabo Verde(30), Arabia Saudí(31), Uruguay(32)
                new() { GrupoId = 8, EquipoId = 29 },
                new() { GrupoId = 8, EquipoId = 30 },
                new() { GrupoId = 8, EquipoId = 31 },
                new() { GrupoId = 8, EquipoId = 32 },

                // Grupo I: Francia(33), Senegal(34), Irak(35), Noruega(36)
                new() { GrupoId = 9,  EquipoId = 33 },
                new() { GrupoId = 9,  EquipoId = 34 },
                new() { GrupoId = 9,  EquipoId = 35 },
                new() { GrupoId = 9,  EquipoId = 36 },

                // Grupo J: Argentina(37), Argelia(38), Austria(39), Jordania(40)
                new() { GrupoId = 10, EquipoId = 37 },
                new() { GrupoId = 10, EquipoId = 38 },
                new() { GrupoId = 10, EquipoId = 39 },
                new() { GrupoId = 10, EquipoId = 40 },

                // Grupo K: Portugal(41), RD Congo(42), Uzbekistán(43), Colombia(44)
                new() { GrupoId = 11, EquipoId = 41 },
                new() { GrupoId = 11, EquipoId = 42 },
                new() { GrupoId = 11, EquipoId = 43 },
                new() { GrupoId = 11, EquipoId = 44 },

                // Grupo L: Inglaterra(45), Croacia(46), Ghana(47), Panamá(48)
                new() { GrupoId = 12, EquipoId = 45 },
                new() { GrupoId = 12, EquipoId = 46 },
                new() { GrupoId = 12, EquipoId = 47 },
                new() { GrupoId = 12, EquipoId = 48 },
            };
            context.GrupoEquipos.AddRange(grupoEquipos);

            // ClasificacionGrupo uno por cada GrupoEquipo con todo en 0
            var clasificaciones = grupoEquipos.Select(ge => new ClasificacionGrupo
            {
                GrupoId = ge.GrupoId,
                EquipoId = ge.EquipoId
            }).ToList();
            context.ClasificacionGrupos.AddRange(clasificaciones);

            await context.SaveChangesAsync();
        }

        private static async Task DatosPartidosFaseGruposAsync(AppDbContext context)
        {

            var partidos = new List<Partido>
            {
                //Partidos fase de grupos
                new() { Id=1,  TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=1,  EquipoVisitanteId=2,  FechaHora=new DateTime(2026,6,11,19,0,0,DateTimeKind.Utc), EstadioId=1 },
                new() { Id=2,  TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=3,  EquipoVisitanteId=4,  FechaHora=new DateTime(2026,6,12,2,0,0,DateTimeKind.Utc),  EstadioId=2 },

                new() { Id=3,  TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=5,  EquipoVisitanteId=6,  FechaHora=new DateTime(2026,6,12,19,0,0,DateTimeKind.Utc), EstadioId=4 },
                new() { Id=4,  TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=13, EquipoVisitanteId=14, FechaHora=new DateTime(2026,6,13,1,0,0,DateTimeKind.Utc),  EstadioId=6 },

                new() { Id=5,  TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=7,  EquipoVisitanteId=8,  FechaHora=new DateTime(2026,6,13,19,0,0,DateTimeKind.Utc), EstadioId=7 },
                new() { Id=6,  TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=9,  EquipoVisitanteId=10, FechaHora=new DateTime(2026,6,13,22,0,0,DateTimeKind.Utc), EstadioId=8 },
                new() { Id=7,  TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=11, EquipoVisitanteId=12, FechaHora=new DateTime(2026,6,14,1,0,0,DateTimeKind.Utc),  EstadioId=9 },
                new() { Id=8,  TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=15, EquipoVisitanteId=16, FechaHora=new DateTime(2026,6,14,4,0,0,DateTimeKind.Utc),  EstadioId=5 },

                new() { Id=9,  TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=17, EquipoVisitanteId=18, FechaHora=new DateTime(2026,6,14,17,0,0,DateTimeKind.Utc), EstadioId=10 },
                new() { Id=10, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=21, EquipoVisitanteId=22, FechaHora=new DateTime(2026,6,14,20,0,0,DateTimeKind.Utc), EstadioId=11 },
                new() { Id=11, TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=19, EquipoVisitanteId=20, FechaHora=new DateTime(2026,6,14,23,0,0,DateTimeKind.Utc), EstadioId=12 },
                new() { Id=12, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=23, EquipoVisitanteId=24, FechaHora=new DateTime(2026,6,15,2,0,0,DateTimeKind.Utc),  EstadioId=3 },

                new() { Id=13, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=29, EquipoVisitanteId=30, FechaHora=new DateTime(2026,6,15,16,0,0,DateTimeKind.Utc), EstadioId=13 },
                new() { Id=14, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=25, EquipoVisitanteId=26, FechaHora=new DateTime(2026,6,15,19,0,0,DateTimeKind.Utc), EstadioId=14 },
                new() { Id=15, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=31, EquipoVisitanteId=32, FechaHora=new DateTime(2026,6,15,22,0,0,DateTimeKind.Utc), EstadioId=15 },
                new() { Id=16, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=27, EquipoVisitanteId=28, FechaHora=new DateTime(2026,6,16,1,0,0,DateTimeKind.Utc),  EstadioId=6 },

                new() { Id=17, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=33, EquipoVisitanteId=34, FechaHora=new DateTime(2026,6,16,19,0,0,DateTimeKind.Utc), EstadioId=8 },
                new() { Id=18, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=35, EquipoVisitanteId=36, FechaHora=new DateTime(2026,6,16,22,0,0,DateTimeKind.Utc), EstadioId=9 },
                new() { Id=19, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=37, EquipoVisitanteId=38, FechaHora=new DateTime(2026,6,17,1,0,0,DateTimeKind.Utc),  EstadioId=16 },
                new() { Id=20, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=39, EquipoVisitanteId=40, FechaHora=new DateTime(2026,6,17,4,0,0,DateTimeKind.Utc),  EstadioId=7 },

                new() { Id=21, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=41, EquipoVisitanteId=42, FechaHora=new DateTime(2026,6,17,17,0,0,DateTimeKind.Utc), EstadioId=10 },
                new() { Id=22, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=45, EquipoVisitanteId=46, FechaHora=new DateTime(2026,6,17,20,0,0,DateTimeKind.Utc), EstadioId=11 },
                new() { Id=23, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=47, EquipoVisitanteId=48, FechaHora=new DateTime(2026,6,17,23,0,0,DateTimeKind.Utc), EstadioId=4 },
                new() { Id=24, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=43, EquipoVisitanteId=44, FechaHora=new DateTime(2026,6,18,2,0,0,DateTimeKind.Utc),  EstadioId=1 },

                new() { Id=25, TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=4,  EquipoVisitanteId=2,  FechaHora=new DateTime(2026,6,18,16,0,0,DateTimeKind.Utc), EstadioId=13 },
                new() { Id=26, TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=8,  EquipoVisitanteId=6,  FechaHora=new DateTime(2026,6,18,19,0,0,DateTimeKind.Utc), EstadioId=6 },
                new() { Id=27, TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=5,  EquipoVisitanteId=7,  FechaHora=new DateTime(2026,6,18,22,0,0,DateTimeKind.Utc), EstadioId=5 },
                new() { Id=28, TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=1,  EquipoVisitanteId=3,  FechaHora=new DateTime(2026,6,19,1,0,0,DateTimeKind.Utc),  EstadioId=2 },

                new() { Id=29, TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=13, EquipoVisitanteId=15, FechaHora=new DateTime(2026,6,19,19,0,0,DateTimeKind.Utc), EstadioId=14 },
                new() { Id=30, TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=12, EquipoVisitanteId=10, FechaHora=new DateTime(2026,6,19,22,0,0,DateTimeKind.Utc), EstadioId=9 },
                new() { Id=31, TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=9,  EquipoVisitanteId=11, FechaHora=new DateTime(2026,6,20,1,0,0,DateTimeKind.Utc),  EstadioId=12 },
                new() { Id=32, TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=16, EquipoVisitanteId=14, FechaHora=new DateTime(2026,6,20,4,0,0,DateTimeKind.Utc),  EstadioId=7 },

                new() { Id=33, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=21, EquipoVisitanteId=23, FechaHora=new DateTime(2026,6,20,17,0,0,DateTimeKind.Utc), EstadioId=10 },
                new() { Id=34, TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=17, EquipoVisitanteId=19, FechaHora=new DateTime(2026,6,20,20,0,0,DateTimeKind.Utc), EstadioId=4 },
                new() { Id=35, TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=20, EquipoVisitanteId=18, FechaHora=new DateTime(2026,6,21,2,0,0,DateTimeKind.Utc),  EstadioId=16 },
                new() { Id=36, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=24, EquipoVisitanteId=22, FechaHora=new DateTime(2026,6,21,4,0,0,DateTimeKind.Utc),  EstadioId=3 },

                new() { Id=37, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=29, EquipoVisitanteId=31, FechaHora=new DateTime(2026,6,21,16,0,0,DateTimeKind.Utc), EstadioId=13 },
                new() { Id=38, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=25, EquipoVisitanteId=27, FechaHora=new DateTime(2026,6,21,19,0,0,DateTimeKind.Utc), EstadioId=6 },
                new() { Id=39, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=32, EquipoVisitanteId=30, FechaHora=new DateTime(2026,6,21,22,0,0,DateTimeKind.Utc), EstadioId=15 },
                new() { Id=40, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=28, EquipoVisitanteId=26, FechaHora=new DateTime(2026,6,22,1,0,0,DateTimeKind.Utc),  EstadioId=5 },

                new() { Id=41, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=37, EquipoVisitanteId=39, FechaHora=new DateTime(2026,6,22,17,0,0,DateTimeKind.Utc), EstadioId=11 },
                new() { Id=42, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=33, EquipoVisitanteId=35, FechaHora=new DateTime(2026,6,22,21,0,0,DateTimeKind.Utc), EstadioId=12 },
                new() { Id=43, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=36, EquipoVisitanteId=34, FechaHora=new DateTime(2026,6,23,0,0,0,DateTimeKind.Utc),  EstadioId=8 },
                new() { Id=44, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=40, EquipoVisitanteId=38, FechaHora=new DateTime(2026,6,23,3,0,0,DateTimeKind.Utc),  EstadioId=7 },

                new() { Id=45, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=41, EquipoVisitanteId=43, FechaHora=new DateTime(2026,6,23,17,0,0,DateTimeKind.Utc), EstadioId=10 },
                new() { Id=46, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=45, EquipoVisitanteId=47, FechaHora=new DateTime(2026,6,23,20,0,0,DateTimeKind.Utc), EstadioId=9 },
                new() { Id=47, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=48, EquipoVisitanteId=46, FechaHora=new DateTime(2026,6,23,23,0,0,DateTimeKind.Utc), EstadioId=4 },
                new() { Id=48, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=44, EquipoVisitanteId=42, FechaHora=new DateTime(2026,6,24,2,0,0,DateTimeKind.Utc),  EstadioId=2 },

                new() { Id=49, TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=8,  EquipoVisitanteId=5,  FechaHora=new DateTime(2026,6,24,19,0,0,DateTimeKind.Utc), EstadioId=5 },
                new() { Id=50, TorneoId=1, FaseId=1, GrupoId=2,  EquipoLocalId=6,  EquipoVisitanteId=7,  FechaHora=new DateTime(2026,6,24,19,0,0,DateTimeKind.Utc), EstadioId=14 },
                new() { Id=51, TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=12, EquipoVisitanteId=9,  FechaHora=new DateTime(2026,6,24,22,0,0,DateTimeKind.Utc), EstadioId=15 },
                new() { Id=52, TorneoId=1, FaseId=1, GrupoId=3,  EquipoLocalId=10, EquipoVisitanteId=11, FechaHora=new DateTime(2026,6,24,22,0,0,DateTimeKind.Utc), EstadioId=13 },
                new() { Id=53, TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=4,  EquipoVisitanteId=1,  FechaHora=new DateTime(2026,6,25,1,0,0,DateTimeKind.Utc),  EstadioId=1 },
                new() { Id=54, TorneoId=1, FaseId=1, GrupoId=1,  EquipoLocalId=2,  EquipoVisitanteId=3,  FechaHora=new DateTime(2026,6,25,1,0,0,DateTimeKind.Utc),  EstadioId=3 },

                new() { Id=55, TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=18, EquipoVisitanteId=19, FechaHora=new DateTime(2026,6,25,20,0,0,DateTimeKind.Utc), EstadioId=12 },
                new() { Id=56, TorneoId=1, FaseId=1, GrupoId=5,  EquipoLocalId=20, EquipoVisitanteId=17, FechaHora=new DateTime(2026,6,25,20,0,0,DateTimeKind.Utc), EstadioId=8 },
                new() { Id=57, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=22, EquipoVisitanteId=23, FechaHora=new DateTime(2026,6,25,23,0,0,DateTimeKind.Utc), EstadioId=11 },
                new() { Id=58, TorneoId=1, FaseId=1, GrupoId=6,  EquipoLocalId=24, EquipoVisitanteId=21, FechaHora=new DateTime(2026,6,25,23,0,0,DateTimeKind.Utc), EstadioId=16 },
                new() { Id=59, TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=16, EquipoVisitanteId=13, FechaHora=new DateTime(2026,6,26,2,0,0,DateTimeKind.Utc),  EstadioId=6 },
                new() { Id=60, TorneoId=1, FaseId=1, GrupoId=4,  EquipoLocalId=14, EquipoVisitanteId=15, FechaHora=new DateTime(2026,6,26,2,0,0,DateTimeKind.Utc),  EstadioId=7 },

                new() { Id=61, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=36, EquipoVisitanteId=33, FechaHora=new DateTime(2026,6,26,19,0,0,DateTimeKind.Utc), EstadioId=9 },
                new() { Id=62, TorneoId=1, FaseId=1, GrupoId=9,  EquipoLocalId=34, EquipoVisitanteId=35, FechaHora=new DateTime(2026,6,26,19,0,0,DateTimeKind.Utc), EstadioId=4 },
                new() { Id=63, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=30, EquipoVisitanteId=31, FechaHora=new DateTime(2026,6,27,0,0,0,DateTimeKind.Utc),  EstadioId=10 },
                new() { Id=64, TorneoId=1, FaseId=1, GrupoId=8,  EquipoLocalId=32, EquipoVisitanteId=29, FechaHora=new DateTime(2026,6,27,0,0,0,DateTimeKind.Utc),  EstadioId=2 },
                new() { Id=65, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=26, EquipoVisitanteId=27, FechaHora=new DateTime(2026,6,27,3,0,0,DateTimeKind.Utc),  EstadioId=14 },
                new() { Id=66, TorneoId=1, FaseId=1, GrupoId=7,  EquipoLocalId=28, EquipoVisitanteId=25, FechaHora=new DateTime(2026,6,27,3,0,0,DateTimeKind.Utc),  EstadioId=5 },

                new() { Id=67, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=48, EquipoVisitanteId=45, FechaHora=new DateTime(2026,6,27,21,0,0,DateTimeKind.Utc), EstadioId=8 },
                new() { Id=68, TorneoId=1, FaseId=1, GrupoId=12, EquipoLocalId=46, EquipoVisitanteId=47, FechaHora=new DateTime(2026,6,27,21,0,0,DateTimeKind.Utc), EstadioId=12 },
                new() { Id=69, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=44, EquipoVisitanteId=41, FechaHora=new DateTime(2026,6,27,23,30,0,DateTimeKind.Utc),EstadioId=15 },
                new() { Id=70, TorneoId=1, FaseId=1, GrupoId=11, EquipoLocalId=42, EquipoVisitanteId=43, FechaHora=new DateTime(2026,6,27,23,30,0,DateTimeKind.Utc),EstadioId=13 },
                new() { Id=71, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=38, EquipoVisitanteId=39, FechaHora=new DateTime(2026,6,28,2,0,0,DateTimeKind.Utc),  EstadioId=16 },
                new() { Id=72, TorneoId=1, FaseId=1, GrupoId=10, EquipoLocalId=40, EquipoVisitanteId=37, FechaHora=new DateTime(2026,6,28,2,0,0,DateTimeKind.Utc),  EstadioId=11 },
            };

            context.Partidos.AddRange(partidos);
            await context.SaveChangesAsync();
        }

        /*private static async Task DatosPartidosFaseEliminatoriaAsync(AppDbContext context)
        {
            var partidos = new List<Partido>
            {
                //16avos
                new() { Id=73,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,28,20,0,0,DateTimeKind.Utc), EstadioId=6,  DescripcionLocal="2º Grupo A",           DescripcionVisitante="2º Grupo B" },
                new() { Id=74,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,29,19,0,0,DateTimeKind.Utc), EstadioId=9,  DescripcionLocal="1º Grupo E",           DescripcionVisitante="3º Grupo A/B/C/D/F" },
                new() { Id=75,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,29,22,0,0,DateTimeKind.Utc), EstadioId=3,  DescripcionLocal="1º Grupo F",           DescripcionVisitante="2º Grupo C" },
                new() { Id=76,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,30,1,0,0,DateTimeKind.Utc),  EstadioId=10, DescripcionLocal="1º Grupo C",           DescripcionVisitante="2º Grupo F" },
                new() { Id=77,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,30,19,0,0,DateTimeKind.Utc), EstadioId=8,  DescripcionLocal="1º Grupo I",           DescripcionVisitante="3º Grupo C/D/F/G/H" },
                new() { Id=78,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,6,30,22,0,0,DateTimeKind.Utc), EstadioId=11, DescripcionLocal="2º Grupo E",           DescripcionVisitante="2º Grupo I" },
                new() { Id=79,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,1,1,0,0,DateTimeKind.Utc),   EstadioId=1,  DescripcionLocal="1º Grupo A",           DescripcionVisitante="3º Grupo C/E/F/H/I" },
                new() { Id=80,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,1,16,0,0,DateTimeKind.Utc),  EstadioId=13, DescripcionLocal="1º Grupo L",           DescripcionVisitante="3º Grupo E/H/I/J/K" },
                new() { Id=81,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,1,19,0,0,DateTimeKind.Utc),  EstadioId=7,  DescripcionLocal="1º Grupo D",           DescripcionVisitante="3º Grupo B/E/F/I/J" },
                new() { Id=82,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,1,22,0,0,DateTimeKind.Utc),  EstadioId=14, DescripcionLocal="1º Grupo G",           DescripcionVisitante="3º Grupo A/E/H/I/J" },
                new() { Id=83,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,2,17,0,0,DateTimeKind.Utc),  EstadioId=4,  DescripcionLocal="2º Grupo K",           DescripcionVisitante="2º Grupo L" },
                new() { Id=84,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,2,20,0,0,DateTimeKind.Utc),  EstadioId=6,  DescripcionLocal="1º Grupo H",           DescripcionVisitante="2º Grupo J" },
                new() { Id=85,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,2,23,0,0,DateTimeKind.Utc),  EstadioId=5,  DescripcionLocal="1º Grupo B",           DescripcionVisitante="3º Grupo E/F/G/I/J" },
                new() { Id=86,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,3,17,0,0,DateTimeKind.Utc),  EstadioId=15, DescripcionLocal="1º Grupo J",           DescripcionVisitante="2º Grupo H" },
                new() { Id=87,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,3,20,0,0,DateTimeKind.Utc),  EstadioId=16, DescripcionLocal="1º Grupo K",           DescripcionVisitante="3º Grupo D/E/I/J/L" },
                new() { Id=88,  TorneoId=1, FaseId=2, GrupoId=null, FechaHora=new DateTime(2026,7,3,23,0,0,DateTimeKind.Utc),  EstadioId=11, DescripcionLocal="2º Grupo D",           DescripcionVisitante="2º Grupo G" },

                //OCTAVOS
                new() { Id=89,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,4,19,0,0,DateTimeKind.Utc),  EstadioId=12, DescripcionLocal="Ganador Partido 74",  DescripcionVisitante="Ganador Partido 77" },
                new() { Id=90,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,4,22,0,0,DateTimeKind.Utc),  EstadioId=10, DescripcionLocal="Ganador Partido 73",  DescripcionVisitante="Ganador Partido 75" },
                new() { Id=91,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,5,19,0,0,DateTimeKind.Utc),  EstadioId=8,  DescripcionLocal="Ganador Partido 76",  DescripcionVisitante="Ganador Partido 78" },
                new() { Id=92,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,5,22,0,0,DateTimeKind.Utc),  EstadioId=1,  DescripcionLocal="Ganador Partido 79",  DescripcionVisitante="Ganador Partido 80" },
                new() { Id=93,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,6,19,0,0,DateTimeKind.Utc),  EstadioId=11, DescripcionLocal="Ganador Partido 83",  DescripcionVisitante="Ganador Partido 84" },
                new() { Id=94,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,6,22,0,0,DateTimeKind.Utc),  EstadioId=14, DescripcionLocal="Ganador Partido 81",  DescripcionVisitante="Ganador Partido 82" },
                new() { Id=95,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,7,19,0,0,DateTimeKind.Utc),  EstadioId=13, DescripcionLocal="Ganador Partido 86",  DescripcionVisitante="Ganador Partido 88" },
                new() { Id=96,  TorneoId=1, FaseId=3, GrupoId=null, FechaHora=new DateTime(2026,7,7,22,0,0,DateTimeKind.Utc),  EstadioId=5,  DescripcionLocal="Ganador Partido 85",  DescripcionVisitante="Ganador Partido 87" },

                // CUARTOS
                new() { Id=97,  TorneoId=1, FaseId=4, GrupoId=null, FechaHora=new DateTime(2026,7,9,19,0,0,DateTimeKind.Utc),  EstadioId=9,  DescripcionLocal="Ganador Partido 89",  DescripcionVisitante="Ganador Partido 90" },
                new() { Id=98,  TorneoId=1, FaseId=4, GrupoId=null, FechaHora=new DateTime(2026,7,10,19,0,0,DateTimeKind.Utc), EstadioId=6,  DescripcionLocal="Ganador Partido 93",  DescripcionVisitante="Ganador Partido 94" },
                new() { Id=99,  TorneoId=1, FaseId=4, GrupoId=null, FechaHora=new DateTime(2026,7,11,19,0,0,DateTimeKind.Utc), EstadioId=15, DescripcionLocal="Ganador Partido 91",  DescripcionVisitante="Ganador Partido 92" },
                new() { Id=100, TorneoId=1, FaseId=4, GrupoId=null, FechaHora=new DateTime(2026,7,11,22,0,0,DateTimeKind.Utc), EstadioId=16, DescripcionLocal="Ganador Partido 95",  DescripcionVisitante="Ganador Partido 96" },

                // SEMIFINALES
                new() { Id=101, TorneoId=1, FaseId=5, GrupoId=null, FechaHora=new DateTime(2026,7,14,19,0,0,DateTimeKind.Utc), EstadioId=11, DescripcionLocal="Ganador Partido 97",  DescripcionVisitante="Ganador Partido 98" },
                new() { Id=102, TorneoId=1, FaseId=5, GrupoId=null, FechaHora=new DateTime(2026,7,15,19,0,0,DateTimeKind.Utc), EstadioId=13, DescripcionLocal="Ganador Partido 99",  DescripcionVisitante="Ganador Partido 100" },

                // TERCER PUESTO
                new() { Id=103, TorneoId=1, FaseId=6, GrupoId=null, FechaHora=new DateTime(2026,7,18,19,0,0,DateTimeKind.Utc), EstadioId=15, DescripcionLocal="Perdedor Partido 101", DescripcionVisitante="Perdedor Partido 102" },


                // FINAL fase id 7
                new() { Id=104, TorneoId=1, FaseId=7, GrupoId=null, FechaHora=new DateTime(2026,7,19,19,0,0,DateTimeKind.Utc), EstadioId=8,  DescripcionLocal="Ganador Partido 101", DescripcionVisitante="Ganador Partido 102" },
            };

            context.Partidos.AddRange(partidos);
            await context.SaveChangesAsync();
        }*/
    }
}