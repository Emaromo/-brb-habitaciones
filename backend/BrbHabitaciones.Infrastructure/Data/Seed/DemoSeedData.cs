using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Data.Seed;

public static class DemoSeedData
{
    private const string DemoOwnerEmail  = "dueno@brbhabitaciones.com";
    private const string DemoOwnerPassword = "Demo1234!";
    private const string AdminEmail      = "admin@brbhabitaciones.com";
    private const string Admin2Email     = "admin2@brbhabitaciones.com";
    private const string SuperEmail      = "ema_romo97@hotmail.com";
    private const string AdminPassword   = "Admin1234!";

    public static async Task SeedAsync(AppDbContext db)
    {
        Console.WriteLine("[SEED] DemoSeedData.SeedAsync started");

        // ── ema_romo97 — primero, independiente de todo lo demás ────────
        Console.WriteLine($"[SEED] Looking up {SuperEmail}...");
        var superUser = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == SuperEmail);
        if (superUser is not null)
        {
            Console.WriteLine($"[SEED] Found {SuperEmail} — current role={superUser.Role}, updating to Administrador");
            superUser.Role = UserRole.Administrador;
            await db.SaveChangesAsync();
            Console.WriteLine($"[SEED] {SuperEmail} role updated OK");
        }
        else
        {
            Console.WriteLine($"[SEED] {SuperEmail} NOT FOUND in DB — skipping role update");
        }

        // ── admin@ ───────────────────────────────────────────────────────
        Console.WriteLine($"[SEED] Processing {AdminEmail}...");
        var adminUser = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == AdminEmail);
        if (adminUser is null)
        {
            db.Users.Add(new User
            {
                Email        = AdminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword),
                FirstName    = "Admin",
                LastName     = "BRB",
                Role         = UserRole.Administrador,
            });
        }
        else
        {
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword);
            adminUser.Role         = UserRole.Administrador;
        }
        await db.SaveChangesAsync();
        Console.WriteLine($"[SEED] {AdminEmail} OK");

        // ── admin2@ ──────────────────────────────────────────────────────
        var admin2User = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == Admin2Email);
        if (admin2User is null)
        {
            db.Users.Add(new User
            {
                Email        = Admin2Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword),
                FirstName    = "Admin2",
                LastName     = "BRB",
                Role         = UserRole.Administrador,
            });
        }
        else
        {
            admin2User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword);
            admin2User.Role         = UserRole.Administrador;
        }
        await db.SaveChangesAsync();

        // ── dueno@ ───────────────────────────────────────────────────────
        var ownerUser = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == DemoOwnerEmail);
        if (ownerUser is null)
        {
            db.Users.Add(new User
            {
                Email        = DemoOwnerEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoOwnerPassword),
                FirstName    = "Carlos",
                LastName     = "Rodríguez",
                Phone        = "+54 11 4567-8901",
                Role         = UserRole.DuenoAlojamiento,
            });
        }
        else
        {
            ownerUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoOwnerPassword);
            ownerUser.Role         = UserRole.DuenoAlojamiento;
        }
        await db.SaveChangesAsync();

        // ── Propiedades demo ─────────────────────────────────────────────
        var propCount = await db.Properties.CountAsync();
        if (propCount >= 30)
        {
            Console.WriteLine($"[SEED] {propCount} propiedades ya existen — saltando seed de propiedades");
            return;
        }

        if (propCount > 0)
        {
            Console.WriteLine($"[SEED] Eliminando {propCount} propiedades antiguas para resembrar con 30...");
            db.Properties.RemoveRange(await db.Properties.ToListAsync());
            await db.SaveChangesAsync();
        }

        var owner     = await db.Users.IgnoreQueryFilters().FirstAsync(u => u.Email == DemoOwnerEmail);
        var amenities = await db.Amenities.ToDictionaryAsync(a => a.Name);
        Amenity? Get(string name) => amenities.GetValueOrDefault(name);

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 1: BUENOS AIRES (Costa Atlántica)
        // ══════════════════════════════════════════════════════════════════

        var p1 = new Property
        {
            OwnerId = owner.Id, Province = "Buenos Aires", City = "Mar del Plata",
            Name = "Departamento Frente al Mar — Mar del Plata",
            Description = "Amplio departamento a metros de la Playa Grande en Mar del Plata. Piso 8 con vista panorámica al Atlántico, completamente equipado con cocina, living con sofá cama, dormitorio principal con cama queen y balcón corrido frente al mar. Ideal para parejas o familia de hasta 4 personas.",
            Address = "Av. Patricio Peralta Ramos 4230, Mar del Plata",
            PropertyType = PropertyType.Departamento, IsActive = true, IsApproved = true,
        };
        p1.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mdp-p1-a/900/600", PublicId = "demo/mdp-p1-a", AltText = "Vista al mar desde el balcón",     IsCover = true,  DisplayOrder = 0 });
        p1.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mdp-p1-b/900/600", PublicId = "demo/mdp-p1-b", AltText = "Living comedor con vista al Atlántico", IsCover = false, DisplayOrder = 1 });
        AddRoom(p1, "Dormitorio Principal con Vista al Mar",
            "Cama queen, placard amplio, aire acondicionado y acceso directo al balcón con vista panorámica al mar. Baño privado completo.",
            2, 12500m, "mdp-r1a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"));
        AddRoom(p1, "Habitación Doble con Balcón Lateral",
            "Dos camas individuales, balcón lateral y baño compartido. Ropa de cama y toallas incluidas.",
            2, 9200m, "mdp-r1b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p1);

        var p2 = new Property
        {
            OwnerId = owner.Id, Province = "Buenos Aires", City = "Villa Gesell",
            Name = "Cabaña en los Médanos — Villa Gesell",
            Description = "Cabaña de madera en el bosque de médanos de Villa Gesell, a 800 metros de la playa. Entorno natural único con pinos y arena. Deck privado con parrilla, hamacas y fogón exterior. Perfecta para desconectarse en familia o con amigos en la costa atlántica bonaerense.",
            Address = "Paseo 104 entre Av. 3 y 4, Villa Gesell",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p2.Photos.Add(new Photo { Url = "https://picsum.photos/seed/vg-p2-a/900/600", PublicId = "demo/vg-p2-a", AltText = "Cabaña entre los pinos", IsCover = true,  DisplayOrder = 0 });
        p2.Photos.Add(new Photo { Url = "https://picsum.photos/seed/vg-p2-b/900/600", PublicId = "demo/vg-p2-b", AltText = "Deck con parrilla",     IsCover = false, DisplayOrder = 1 });
        AddRoom(p2, "Cabaña Completa — Hasta 5 Personas",
            "Cabaña entera con dormitorio principal (cama queen), cama marinera en el altillo, sofá cama en el living, cocina equipada, baño completo y deck con parrilla y fogón.",
            5, 18500m, "vg-r2a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Parrilla / Asador"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p2, "Habitación en Cabaña Secundaria",
            "Dormitorio independiente en planta baja con cama matrimonial, baño compartido y acceso al deck.",
            2, 10500m, "vg-r2b", 2,
            Get("WiFi"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Estacionamiento"));
        db.Properties.Add(p2);

        var p3 = new Property
        {
            OwnerId = owner.Id, Province = "Buenos Aires", City = "Pinamar",
            Name = "Casa en el Bosque — Pinamar",
            Description = "Hermosa casa de madera y ladrillo a la vista en Pinamar Norte, rodeada de bosque de pinos, a tres cuadras del mar. Jardín privado con piscina, parrilla y hamacas. Un oasis de tranquilidad para disfrutar la costa atlántica con toda la comodidad.",
            Address = "Calle de las Artes 2345, Pinamar",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p3.Photos.Add(new Photo { Url = "https://picsum.photos/seed/pin-p3-a/900/600", PublicId = "demo/pin-p3-a", AltText = "Casa en el bosque de pinos", IsCover = true,  DisplayOrder = 0 });
        p3.Photos.Add(new Photo { Url = "https://picsum.photos/seed/pin-p3-b/900/600", PublicId = "demo/pin-p3-b", AltText = "Piscina en el jardín",      IsCover = false, DisplayOrder = 1 });
        AddRoom(p3, "Suite Principal con Baño en Suite",
            "Dormitorio principal con cama king, vestidor, baño privado y acceso al jardín. La habitación más amplia y confortable de la casa.",
            2, 15500m, "pin-r3a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("TV"), Get("Caja fuerte"));
        AddRoom(p3, "Habitación Doble con Vista al Jardín",
            "Dos camas individuales, baño compartido y ventanas orientadas al jardín y la piscina.",
            2, 8800m, "pin-r3b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        AddRoom(p3, "Habitación Simple",
            "Habitación cómoda con cama de una plaza, armario amplio y ventana al jardín. Ideal para niño o viajero solo.",
            1, 5500m, "pin-r3c", 2,
            Get("WiFi"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p3);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Buenos Aires — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 2: CÓRDOBA (Sierras)
        // ══════════════════════════════════════════════════════════════════

        var p4 = new Property
        {
            OwnerId = owner.Id, Province = "Córdoba", City = "Villa General Belgrano",
            Name = "Posada del Valle — Villa General Belgrano",
            Description = "Encantadora posada de estilo bávaro en el corazón de Villa General Belgrano. La localidad más alemana de Argentina, a 90 km de la ciudad de Córdoba, rodeada de sierras y el río Los Reartes. Desayuno serrano incluido con pan casero y mermeladas artesanales.",
            Address = "Av. Julio Roca 854, Villa General Belgrano",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p4.Photos.Add(new Photo { Url = "https://picsum.photos/seed/vgb-p4-a/900/600", PublicId = "demo/vgb-p4-a", AltText = "Fachada bávara de la posada", IsCover = true,  DisplayOrder = 0 });
        p4.Photos.Add(new Photo { Url = "https://picsum.photos/seed/vgb-p4-b/900/600", PublicId = "demo/vgb-p4-b", AltText = "Jardín serrano",             IsCover = false, DisplayOrder = 1 });
        AddRoom(p4, "Suite Bávara con Jacuzzi",
            "Suite de lujo con cama king, jacuzzi privado, balcón con vista a las sierras y desayuno buffet incluido.",
            2, 16500m, "vgb-r4a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Balcón / Terraza"));
        AddRoom(p4, "Habitación Estándar Doble",
            "Habitación doble con cama matrimonial, baño privado, calefacción central y vista al jardín. Desayuno continental incluido.",
            2, 9500m, "vgb-r4b", 2,
            Get("WiFi"), Get("Calefacción"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p4);

        var p5 = new Property
        {
            OwnerId = owner.Id, Province = "Córdoba", City = "Mina Clavero",
            Name = "Cabaña a Orillas del Río — Mina Clavero",
            Description = "Cabaña privada a 50 metros del cristalino río Mina Clavero, en plenas Sierras de Córdoba. Bajá directamente al agua desde el jardín. Deck con reposeras, parrilla y vista serrana. El destino ideal para el verano cordobés: sol, río y naturaleza.",
            Address = "Av. Costanera del Río 1560, Mina Clavero",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p5.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mc-p5-a/900/600", PublicId = "demo/mc-p5-a", AltText = "Cabaña junto al río serrano", IsCover = true,  DisplayOrder = 0 });
        p5.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mc-p5-b/900/600", PublicId = "demo/mc-p5-b", AltText = "Deck frente al río",          IsCover = false, DisplayOrder = 1 });
        AddRoom(p5, "Cabaña Completa Frente al Río — 4 Personas",
            "Cabaña entera con dormitorio principal (cama queen), altillo con cama marinera, living, cocina equipada, baño completo y deck con parrilla frente al río.",
            4, 19500m, "mc-r5a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Parrilla / Asador"), Get("Estacionamiento"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p5, "Cabaña Doble con Acceso al Río",
            "Cabaña secundaria más pequeña con cama matrimonial, cocina integrada, baño privado y mismo acceso al río y jardín.",
            2, 11500m, "mc-r5b", 2,
            Get("WiFi"), Get("Cocina equipada"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p5);

        var p6 = new Property
        {
            OwnerId = owner.Id, Province = "Córdoba", City = "Alta Gracia",
            Name = "Casa Colonial — Alta Gracia",
            Description = "Encantadora casa colonial restaurada a dos cuadras del centro de Alta Gracia y a metros del Tajamar. A 35 km de la ciudad de Córdoba. Patio interno con fuente, galería y jardín. La ciudad del Virrey Liniers y la juventud del Che Guevara.",
            Address = "Calle del Virrey 678, Alta Gracia",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p6.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ag-p6-a/900/600", PublicId = "demo/ag-p6-a", AltText = "Fachada colonial", IsCover = true,  DisplayOrder = 0 });
        p6.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ag-p6-b/900/600", PublicId = "demo/ag-p6-b", AltText = "Patio interno",   IsCover = false, DisplayOrder = 1 });
        AddRoom(p6, "Dormitorio Principal con Patio",
            "Habitación con cama king, piso de madera, techo alto y acceso directo al patio interno colonial. Baño en suite con ducha de lluvia.",
            2, 9800m, "ag-r6a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Escritorio de trabajo"));
        AddRoom(p6, "Habitación con Dos Camas Individuales",
            "Habitación con dos camas individuales, ventana al jardín y baño compartido. Ideal para hermanos o amigos.",
            2, 7200m, "ag-r6b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p6);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Córdoba — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 3: MENDOZA (Vinos y Cordillera)
        // ══════════════════════════════════════════════════════════════════

        var p7 = new Property
        {
            OwnerId = owner.Id, Province = "Mendoza", City = "Chacras de Coria",
            Name = "Cabaña Viñas del Sur — Chacras de Coria",
            Description = "Cabaña privada en Chacras de Coria, a minutos de las bodegas más importantes de Mendoza. Rodeada de viñedos con deck privado, parrilla y vista a la Cordillera de los Andes. Perfecta para parejas o grupos pequeños que buscan desconectarse entre vinos y montañas.",
            Address = "Carril Rodríguez Peña 3210, Chacras de Coria",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p7.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cdc-p7-a/900/600", PublicId = "demo/cdc-p7-a", AltText = "Cabaña con vista a los viñedos", IsCover = true,  DisplayOrder = 0 });
        p7.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cdc-p7-b/900/600", PublicId = "demo/cdc-p7-b", AltText = "Deck y Cordillera",             IsCover = false, DisplayOrder = 1 });
        AddRoom(p7, "Cabaña Completa — 4 Personas",
            "Cabaña con cama king, altillo con cama marinera, living, cocina equipada, baño completo y deck privado con parrilla y vista a los Andes.",
            4, 21500m, "cdc-r7a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Parrilla / Asador"), Get("Estacionamiento"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"));
        AddRoom(p7, "Loft Superior con Vista a los Andes",
            "Loft moderno con cama queen, baño en suite, minifridge y balcón con vista a la Cordillera.",
            2, 13500m, "cdc-r7b", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"));
        db.Properties.Add(p7);

        var p8 = new Property
        {
            OwnerId = owner.Id, Province = "Mendoza", City = "Luján de Cuyo",
            Name = "Posada Los Álamos — Luján de Cuyo",
            Description = "Posada boutique en el corazón de la región vitivinícola de Luján de Cuyo, rodeada de álamos y viñedos. A 25 km de la ciudad de Mendoza y a metros de las principales bodegas de la zona. Desayuno mendocino con pan casero, dulces artesanales y quesos de cabra.",
            Address = "Ruta Provincial 15 km 4.5, Luján de Cuyo",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p8.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ldc-p8-a/900/600", PublicId = "demo/ldc-p8-a", AltText = "Posada entre álamos y viñedos", IsCover = true,  DisplayOrder = 0 });
        p8.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ldc-p8-b/900/600", PublicId = "demo/ldc-p8-b", AltText = "Jardín de la posada",          IsCover = false, DisplayOrder = 1 });
        AddRoom(p8, "Habitación Superior con Vista al Viñedo",
            "Habitación elegante con cama king, baño en suite con bañadera, calefacción y ventana panorámica al viñedo. Desayuno incluido.",
            2, 14500m, "ldc-r8a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Estacionamiento"));
        AddRoom(p8, "Habitación Estándar Doble",
            "Habitación cómoda con cama matrimonial, baño privado, vista al jardín y desayuno continental incluido.",
            2, 10200m, "ldc-r8b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Estacionamiento"));
        db.Properties.Add(p8);

        var p9 = new Property
        {
            OwnerId = owner.Id, Province = "Mendoza", City = "Maipú",
            Name = "Casa de Bodega — Maipú",
            Description = "Antigua casa de bodega reconvertida en alojamiento boutique en Maipú, a 15 km de la ciudad de Mendoza, en el centro de la ruta del vino. Arquitectura de adobe y ladrillo, techos de vigas de madera, patio central con parras. Bodegas a pie y bicis para recorrerlas.",
            Address = "Calle Ozamis 2876, Maipú",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p9.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mai-p9-a/900/600", PublicId = "demo/mai-p9-a", AltText = "Casa de bodega con parras", IsCover = true,  DisplayOrder = 0 });
        p9.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mai-p9-b/900/600", PublicId = "demo/mai-p9-b", AltText = "Patio central",            IsCover = false, DisplayOrder = 1 });
        AddRoom(p9, "Suite de la Barrica",
            "Suite en el antiguo depósito de barricas, con vigas originales a la vista, cama king, baño en suite con ducha de piedra y vista al patio de parras.",
            2, 18000m, "mai-r9a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Caja fuerte"));
        AddRoom(p9, "Habitación del Mayordomo",
            "Habitación doble con cama matrimonial, baño privado y acceso al patio de parras. Nombrada en honor al mayordomo de la antigua bodega.",
            2, 12000m, "mai-r9b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p9);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Mendoza — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 4: SALTA (Noroeste)
        // ══════════════════════════════════════════════════════════════════

        var p10 = new Property
        {
            OwnerId = owner.Id, Province = "Salta", City = "Salta",
            Name = "Posada Bellavista — Salta Capital",
            Description = "Encantador posada boutique en el barrio histórico de Salta, a dos cuadras de la Plaza 9 de Julio. Casa colonial del siglo XIX restaurada, combinando arquitectura original con comodidades modernas. Patio andaluz con naranjos y terraza con vista a los cerros colorados.",
            Address = "Balcarce 1023, Salta",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p10.Photos.Add(new Photo { Url = "https://picsum.photos/seed/slt-p10-a/900/600", PublicId = "demo/slt-p10-a", AltText = "Patio colonial con naranjos",   IsCover = true,  DisplayOrder = 0 });
        p10.Photos.Add(new Photo { Url = "https://picsum.photos/seed/slt-p10-b/900/600", PublicId = "demo/slt-p10-b", AltText = "Vista a los cerros colorados", IsCover = false, DisplayOrder = 1 });
        AddRoom(p10, "Suite Colonial con Patio",
            "Suite en planta alta con cama king, balcón al patio andaluz, baño en suite con bañadera de pie y desayuno regional incluido.",
            2, 19500m, "slt-r10a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Caja fuerte"), Get("Balcón / Terraza"));
        AddRoom(p10, "Habitación Superior Doble",
            "Habitación con cama matrimonial, baño privado, piso de mosaico veneciano y ventanas al patio. Desayuno incluido.",
            2, 13500m, "slt-r10b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p10);

        var p11 = new Property
        {
            OwnerId = owner.Id, Province = "Salta", City = "Cafayate",
            Name = "Cabaña entre Viñedos — Cafayate",
            Description = "Cabaña rústica entre los viñedos de altura de Cafayate, en el Valle Calchaquí. A 1.700 metros sobre el nivel del mar, con cielos estrellados únicos. Rodeada de los viñedos más altos del mundo y las imponentes Quebradas de Cafayate, candidatas a la lista de la UNESCO.",
            Address = "Ruta Nacional 40 km 4280, Cafayate",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p11.Photos.Add(new Photo { Url = "https://picsum.photos/seed/caf-p11-a/900/600", PublicId = "demo/caf-p11-a", AltText = "Cabaña con viñedos de altura",    IsCover = true,  DisplayOrder = 0 });
        p11.Photos.Add(new Photo { Url = "https://picsum.photos/seed/caf-p11-b/900/600", PublicId = "demo/caf-p11-b", AltText = "Atardecer sobre las Quebradas", IsCover = false, DisplayOrder = 1 });
        AddRoom(p11, "Cabaña Completa — 3 Personas",
            "Cabaña de adobe y piedra con cama matrimonial, sofá cama, cocina equipada y deck con vista 360° a los viñedos y las sierras calchaquíes.",
            3, 16500m, "caf-r11a", 4,
            Get("Cocina equipada"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p11, "Habitación con Vista a las Quebradas",
            "Habitación doble con cama matrimonial, baño privado y ventana con vista directa a las Quebradas de Cafayate.",
            2, 10500m, "caf-r11b", 2,
            Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p11);

        var p12 = new Property
        {
            OwnerId = owner.Id, Province = "Salta", City = "Cachi",
            Name = "Casa de Adobe — Cachi",
            Description = "Casa de adobe en el corazón de Cachi, el pueblo más hermoso de los Valles Calchaquíes salteños. A 2.280 metros de altura, con cielos de 300 días de sol al año. Arquitectura andina colonial con patio de tierra, vigas de cactus y vista a los nevados de Cachi de 6.380 metros.",
            Address = "Ruiz de los Llanos 456, Cachi",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p12.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cac-p12-a/900/600", PublicId = "demo/cac-p12-a", AltText = "Casa de adobe frente a los nevados", IsCover = true,  DisplayOrder = 0 });
        p12.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cac-p12-b/900/600", PublicId = "demo/cac-p12-b", AltText = "Patio andino colonial",             IsCover = false, DisplayOrder = 1 });
        AddRoom(p12, "Habitación con Vista a los Nevados",
            "Habitación de adobe con cama matrimonial, ventana con vista directa a los nevados de Cachi, calefacción a leña y baño privado.",
            2, 11500m, "cac-r12a", 3,
            Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        AddRoom(p12, "Habitación Triple — Familia",
            "Habitación con cama matrimonial y una individual, baño compartido y acceso al patio de cardones.",
            3, 14000m, "cac-r12b", 2,
            Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p12);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Salta — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 5: MISIONES (Selva y Cataratas)
        // ══════════════════════════════════════════════════════════════════

        var p13 = new Property
        {
            OwnerId = owner.Id, Province = "Misiones", City = "Puerto Iguazú",
            Name = "Posada Selva Verde — Puerto Iguazú",
            Description = "Posada boutique en la selva misionera, a 3 km de las Cataratas del Iguazú, Patrimonio Natural de la Humanidad. Inmersa en la naturaleza subtropical con tucanes, monos capuchinos y mariposas morpho azules. Piscina natural rodeada de helechos gigantes y desayuno regional.",
            Address = "Ruta 12 km 1055, Puerto Iguazú",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p13.Photos.Add(new Photo { Url = "https://picsum.photos/seed/igu-p13-a/900/600", PublicId = "demo/igu-p13-a", AltText = "Posada en la selva misionera", IsCover = true,  DisplayOrder = 0 });
        p13.Photos.Add(new Photo { Url = "https://picsum.photos/seed/igu-p13-b/900/600", PublicId = "demo/igu-p13-b", AltText = "Piscina en la selva",          IsCover = false, DisplayOrder = 1 });
        AddRoom(p13, "Cabaña Selvática Suite",
            "Cabaña independiente en la selva con cama king, baño con ducha exterior de piedra y galería privada con hamaca paraguaya. Desayuno con frutas tropicales incluido.",
            2, 22500m, "igu-r13a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Balcón / Terraza"), Get("Piscina"));
        AddRoom(p13, "Habitación Doble Superior",
            "Habitación doble con cama matrimonial, baño privado, aire acondicionado y ventanas a la selva. Desayuno tropical incluido.",
            2, 15500m, "igu-r13b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Piscina"));
        db.Properties.Add(p13);

        var p14 = new Property
        {
            OwnerId = owner.Id, Province = "Misiones", City = "San Ignacio",
            Name = "Casa de Campo — San Ignacio",
            Description = "Casa de campo a 500 metros de las ruinas jesuíticas de San Ignacio Miní, declaradas Patrimonio de la Humanidad por UNESCO. Construcción de ladrillo artesanal misionero con techo de tejas, galería envolvente y jardín con araucarias y palmeras. A 60 km de Posadas.",
            Address = "Ruta Nacional 12 km 1230, San Ignacio",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p14.Photos.Add(new Photo { Url = "https://picsum.photos/seed/sgi-p14-a/900/600", PublicId = "demo/sgi-p14-a", AltText = "Casa colonial en San Ignacio", IsCover = true,  DisplayOrder = 0 });
        p14.Photos.Add(new Photo { Url = "https://picsum.photos/seed/sgi-p14-b/900/600", PublicId = "demo/sgi-p14-b", AltText = "Jardín con palmeras",           IsCover = false, DisplayOrder = 1 });
        AddRoom(p14, "Habitación Principal con Galería",
            "Habitación con cama king, baño privado, techo de vigas y acceso directo a la galería exterior con vista al jardín y las araucarias.",
            2, 9500m, "sgi-r14a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Estacionamiento"), Get("Mascotas permitidas"));
        AddRoom(p14, "Habitación Doble Estándar",
            "Habitación con dos camas individuales, baño compartido y ventilador de techo. Ideal para dos viajeros.",
            2, 6800m, "sgi-r14b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p14);

        var p15 = new Property
        {
            OwnerId = owner.Id, Province = "Misiones", City = "Oberá",
            Name = "Cabaña en la Selva — Oberá",
            Description = "Cabaña privada en el municipio de Oberá, corazón de la colonización alemana, ucraniana y polaca de Misiones. Sede de la Fiesta Nacional del Inmigrante (septiembre). Entorno de selva subtropical con orquídeas, helechos gigantes y aves exóticas. A 100 km de Posadas.",
            Address = "Colonia Guaraní km 8, Oberá",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p15.Photos.Add(new Photo { Url = "https://picsum.photos/seed/obe-p15-a/900/600", PublicId = "demo/obe-p15-a", AltText = "Cabaña rodeada de selva", IsCover = true, DisplayOrder = 0 });
        AddRoom(p15, "Cabaña Familiar — Hasta 4 Personas",
            "Cabaña con dos dormitorios (cama matrimonial y dos individuales), baño completo, cocina equipada y galería con vista a la selva.",
            4, 14500m, "obe-r15a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p15, "Loft Doble en la Selva",
            "Cabaña pequeña con cama matrimonial, cocina integrada y baño privado. Para parejas que buscan privacidad en la naturaleza.",
            2, 9500m, "obe-r15b", 2,
            Get("WiFi"), Get("Cocina equipada"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p15);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Misiones — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 6: RÍO NEGRO (Patagonia Andina)
        // ══════════════════════════════════════════════════════════════════

        var p16 = new Property
        {
            OwnerId = owner.Id, Province = "Río Negro", City = "San Carlos de Bariloche",
            Name = "Cabaña Frente al Lago Nahuel Huapi — Bariloche",
            Description = "Cabaña de troncos con vista directa al Lago Nahuel Huapi y los picos nevados del Cerro López. A 8 km del centro de Bariloche por la Av. Bustillo. Playa privada de piedras para kayak, muelle privado y terraza con la mejor vista de la Patagonia andina.",
            Address = "Av. Bustillo km 8.5, Bariloche",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p16.Photos.Add(new Photo { Url = "https://picsum.photos/seed/bar-p16-a/900/600", PublicId = "demo/bar-p16-a", AltText = "Cabaña frente al Nahuel Huapi", IsCover = true,  DisplayOrder = 0 });
        p16.Photos.Add(new Photo { Url = "https://picsum.photos/seed/bar-p16-b/900/600", PublicId = "demo/bar-p16-b", AltText = "Muelle privado y lago",          IsCover = false, DisplayOrder = 1 });
        AddRoom(p16, "Cabaña Lake View — 4 Personas",
            "Cabaña de troncos con vista 180° al lago. Cama king en el dormitorio principal, altillo con camas marineras, chimenea a leña, cocina equipada y deck frente al agua.",
            4, 32500m, "bar-r16a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"), Get("Mascotas permitidas"));
        AddRoom(p16, "Loft para Parejas — Vista al Lago",
            "Loft con cama queen, calefacción a leña, kitchenette y balcón privado con vista al lago y la cordillera nevada.",
            2, 21500m, "bar-r16b", 3,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"));
        db.Properties.Add(p16);

        var p17 = new Property
        {
            OwnerId = owner.Id, Province = "Río Negro", City = "Villa La Angostura",
            Name = "Posada de Montaña — Villa La Angostura",
            Description = "Acogedora posada en Villa La Angostura, la perla del Nahuel Huapi. A 80 km de Bariloche, en el acceso al Parque Nacional Los Arrayanes, con los únicos bosques de arrayanes del mundo. Desayuno patagónico con mermeladas artesanales de frutos del bosque.",
            Address = "Av. Arrayanes 2340, Villa La Angostura",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p17.Photos.Add(new Photo { Url = "https://picsum.photos/seed/vla-p17-a/900/600", PublicId = "demo/vla-p17-a", AltText = "Posada entre arrayanes", IsCover = true, DisplayOrder = 0 });
        AddRoom(p17, "Habitación Superior Patagónica",
            "Habitación con cama king, chimenea a leña, baño en suite y ventana al bosque de arrayanes. Desayuno patagónico incluido.",
            2, 24500m, "vla-r17a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Estacionamiento"));
        AddRoom(p17, "Habitación Doble Estándar",
            "Habitación con cama matrimonial, baño privado, calefacción central y vista al jardín con nieve en invierno. Desayuno incluido.",
            2, 17500m, "vla-r17b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Estacionamiento"));
        db.Properties.Add(p17);

        var p18 = new Property
        {
            OwnerId = owner.Id, Province = "Río Negro", City = "El Bolsón",
            Name = "Casa en el Bosque — El Bolsón",
            Description = "Casa de campo en El Bolsón, el pueblo hippie y artesanal de la Patagonia. Rodeada de bosque de pinos y sauces, a 130 km de Bariloche. Famosa por su Feria Artesanal, la cerveza artesanal y el lúpulo. Río Quemquemtreu a 200 metros para nadar y pescar.",
            Address = "Calle de los Artesanos 890, El Bolsón",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p18.Photos.Add(new Photo { Url = "https://picsum.photos/seed/elb-p18-a/900/600", PublicId = "demo/elb-p18-a", AltText = "Casa entre pinos en El Bolsón", IsCover = true, DisplayOrder = 0 });
        AddRoom(p18, "Habitación Matrimonial con Estufa a Leña",
            "Habitación con cama matrimonial, estufa a leña, baño privado y ventanas al bosque.",
            2, 12500m, "elb-r18a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p18, "Habitación Triple — Familia",
            "Habitación con cama matrimonial y una individual, baño compartido y acceso al jardín con río.",
            3, 16000m, "elb-r18b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        db.Properties.Add(p18);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Río Negro — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 7: NEUQUÉN (Lagos y Montañas)
        // ══════════════════════════════════════════════════════════════════

        var p19 = new Property
        {
            OwnerId = owner.Id, Province = "Neuquén", City = "San Martín de los Andes",
            Name = "Cabaña Lago Lácar — San Martín de los Andes",
            Description = "Cabaña de madera sobre la costa del Lago Lácar, en San Martín de los Andes, la joya del Neuquén. A 5 km del centro. Ciudad modelo, con arquitectura alpina y calles de tierra. Acceso privado al lago, kayaks incluidos, y montañas nevadas que rodean el agua.",
            Address = "Av. Costanera Lácar 1890, San Martín de los Andes",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p19.Photos.Add(new Photo { Url = "https://picsum.photos/seed/sma-p19-a/900/600", PublicId = "demo/sma-p19-a", AltText = "Cabaña frente al Lago Lácar",   IsCover = true,  DisplayOrder = 0 });
        p19.Photos.Add(new Photo { Url = "https://picsum.photos/seed/sma-p19-b/900/600", PublicId = "demo/sma-p19-b", AltText = "Acceso privado y kayaks al lago", IsCover = false, DisplayOrder = 1 });
        AddRoom(p19, "Cabaña Premium Lago Lácar — 4 Personas",
            "Cabaña de troncos con cama king, altillo con camas marineras, chimenea, cocina equipada y deck sobre el lago con vista a la cordillera.",
            4, 35500m, "sma-r19a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"), Get("Mascotas permitidas"));
        AddRoom(p19, "Suite Lago — Para Parejas",
            "Suite independiente con cama queen, chimenea, jacuzzi privado y terraza con vista directa al lago y la cordillera nevada.",
            2, 28500m, "sma-r19b", 3,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"), Get("Desayuno incluido"));
        db.Properties.Add(p19);

        var p20 = new Property
        {
            OwnerId = owner.Id, Province = "Neuquén", City = "Junín de los Andes",
            Name = "Posada del Pescador — Junín de los Andes",
            Description = "Posada familiar en Junín de los Andes, la capital nacional de la trucha. A orillas del Río Chimehuin, el río de pesca con mosca más famoso de Sudamérica. Guías de pesca disponibles, secadero de moscas y freezer para conservar capturas. A 40 km de San Martín de los Andes.",
            Address = "Coronel Suárez 560, Junín de los Andes",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p20.Photos.Add(new Photo { Url = "https://picsum.photos/seed/jun-p20-a/900/600", PublicId = "demo/jun-p20-a", AltText = "Posada a orillas del Chimehuin", IsCover = true, DisplayOrder = 0 });
        AddRoom(p20, "Habitación del Pescador con Vista al Río",
            "Habitación con cama matrimonial, baño privado y ventana con vista al Río Chimehuin. Desayuno tempranero incluido para salir a pescar al amanecer.",
            2, 13500m, "jun-r20a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        AddRoom(p20, "Habitación Doble Estándar",
            "Habitación doble con cama matrimonial, baño privado y calefacción central.",
            2, 10500m, "jun-r20b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p20);

        var p21 = new Property
        {
            OwnerId = owner.Id, Province = "Neuquén", City = "Las Lajas",
            Name = "Casa de Montaña — Las Lajas",
            Description = "Casa de montaña en Las Lajas, Neuquén, al pie de las sierras del Pehuén y a 80 km del volcán Copahue con sus famosas termas y la laguna turquesa de aguas ácidas volcánicas. Pueblo tranquilo de 10.000 habitantes con valles, cerros y el Río Loncopué.",
            Address = "San Martín 1245, Las Lajas",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p21.Photos.Add(new Photo { Url = "https://picsum.photos/seed/llj-p21-a/900/600", PublicId = "demo/llj-p21-a", AltText = "Casa de montaña en Las Lajas", IsCover = true, DisplayOrder = 0 });
        AddRoom(p21, "Habitación con Chimenea y Vista a los Cerros",
            "Habitación principal con cama matrimonial, chimenea a leña, baño en suite y ventanales con vista a las sierras del Pehuén.",
            2, 10500m, "llj-r21a", 3,
            Get("WiFi"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        AddRoom(p21, "Habitación Doble con Jardín",
            "Habitación con dos camas individuales, baño compartido y vista al jardín con pehuenes (araucarias).",
            2, 8500m, "llj-r21b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));
        db.Properties.Add(p21);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Neuquén — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 8: ENTRE RÍOS (Mesopotamia)
        // ══════════════════════════════════════════════════════════════════

        var p22 = new Property
        {
            OwnerId = owner.Id, Province = "Entre Ríos", City = "Colón",
            Name = "Casa Termal — Colón",
            Description = "Casa con piscina termal privada en Colón, la ciudad de las termas del Palmar, a orillas del Río Uruguay. Jardín con piscina de agua termal natural a 38°C. A metros de los complejos termales más importantes de Argentina y el Parque Nacional El Palmar con las palmeras yatay más antiguas del mundo.",
            Address = "Pellegrini 1567, Colón",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p22.Photos.Add(new Photo { Url = "https://picsum.photos/seed/col-p22-a/900/600", PublicId = "demo/col-p22-a", AltText = "Casa con piscina termal privada", IsCover = true,  DisplayOrder = 0 });
        p22.Photos.Add(new Photo { Url = "https://picsum.photos/seed/col-p22-b/900/600", PublicId = "demo/col-p22-b", AltText = "Piscina termal al atardecer",   IsCover = false, DisplayOrder = 1 });
        AddRoom(p22, "Habitación con Piscina Termal Privada",
            "Habitación con cama king, baño privado y acceso exclusivo a la piscina termal del jardín. Toallas de piscina incluidas.",
            2, 16500m, "col-r22a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Estacionamiento"), Get("Piscina"));
        AddRoom(p22, "Habitación Doble con Acceso al Jardín",
            "Habitación doble con cama matrimonial, baño compartido y acceso al jardín y la piscina termal.",
            2, 11500m, "col-r22b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Estacionamiento"), Get("Piscina"));
        db.Properties.Add(p22);

        var p23 = new Property
        {
            OwnerId = owner.Id, Province = "Entre Ríos", City = "Gualeguaychú",
            Name = "Posada del Carnaval — Gualeguaychú",
            Description = "Posada a metros del Corsódromo de Gualeguaychú, sede del carnaval más largo del mundo (enero a marzo). Fuera de temporada de carnaval, Gualeguaychú ofrece playas de río, termas naturales y la reserva natural del Parque Unzué. A 230 km de Buenos Aires.",
            Address = "Costanera 25 de Mayo 890, Gualeguaychú",
            PropertyType = PropertyType.Posada, IsActive = true, IsApproved = true,
        };
        p23.Photos.Add(new Photo { Url = "https://picsum.photos/seed/gch-p23-a/900/600", PublicId = "demo/gch-p23-a", AltText = "Posada a orillas del río", IsCover = true, DisplayOrder = 0 });
        AddRoom(p23, "Habitación Doble con Vista al Río",
            "Habitación con cama matrimonial, baño privado y balcón con vista al Río Gualeguaychú. Desayuno incluido.",
            2, 9500m, "gch-r23a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Balcón / Terraza"), Get("Estacionamiento"));
        AddRoom(p23, "Habitación Estándar con Jardín",
            "Habitación con cama matrimonial, baño privado, aire acondicionado y acceso al jardín con hamacas. Desayuno incluido.",
            2, 7500m, "gch-r23b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p23);

        var p24 = new Property
        {
            OwnerId = owner.Id, Province = "Entre Ríos", City = "Concordia",
            Name = "Cabaña Lago Salto Grande — Concordia",
            Description = "Cabaña con vista al Lago de Salto Grande en Concordia, generado por la represa binacional argentino-uruguaya. Paraíso para la pesca deportiva de dorado y surubí, kayak y windsurf. Muelle privado. A 360 km de Buenos Aires por la Ruta 14 (Autovía de los Inmigrantes).",
            Address = "Costa del Lago Salto Grande km 15, Concordia",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p24.Photos.Add(new Photo { Url = "https://picsum.photos/seed/con-p24-a/900/600", PublicId = "demo/con-p24-a", AltText = "Cabaña frente al Lago Salto Grande", IsCover = true, DisplayOrder = 0 });
        AddRoom(p24, "Cabaña con Muelle Privado — 4 Personas",
            "Cabaña completa con cama matrimonial, altillo con camas, cocina equipada, baño completo, muelle privado para pesca y parrilla con vista al lago.",
            4, 17500m, "con-r24a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"), Get("Parrilla / Asador"));
        AddRoom(p24, "Habitación Ribereña Doble",
            "Habitación con cama matrimonial, baño privado y galería con hamacas frente al lago.",
            2, 11500m, "con-r24b", 2,
            Get("WiFi"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        db.Properties.Add(p24);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Entre Ríos — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 9: TUCUMÁN (Noroeste y Sierras)
        // ══════════════════════════════════════════════════════════════════

        var p25 = new Property
        {
            OwnerId = owner.Id, Province = "Tucumán", City = "Tafí del Valle",
            Name = "Cabaña en los Valles Calchaquíes — Tafí del Valle",
            Description = "Cabaña de montaña en Tafí del Valle, a 2.000 metros sobre el nivel del mar en la precordillera tucumana. Clima fresco en verano (18°C promedio) y paisajes andinos únicos. A 100 km de la ciudad de Tucumán por la espectacular Ruta del Infiernillo. Valle sagrado de los Menhires diaguitas.",
            Address = "Av. Los Faroles 345, Tafí del Valle",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p25.Photos.Add(new Photo { Url = "https://picsum.photos/seed/tafi-p25-a/900/600", PublicId = "demo/tafi-p25-a", AltText = "Cabaña en los valles calchaquíes", IsCover = true, DisplayOrder = 0 });
        AddRoom(p25, "Cabaña de Montaña — 3 Personas",
            "Cabaña con cama matrimonial, sofá cama, cocina equipada, baño completo, calefacción a leña y galería con vista al valle.",
            3, 14500m, "tafi-r25a", 4,
            Get("WiFi"), Get("Cocina equipada"), Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"), Get("Parrilla / Asador"));
        AddRoom(p25, "Habitación Doble con Vista al Valle",
            "Habitación con cama matrimonial, calefacción, baño privado y terraza con vista al valle calchaquí.",
            2, 10500m, "tafi-r25b", 2,
            Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"));
        db.Properties.Add(p25);

        var p26 = new Property
        {
            OwnerId = owner.Id, Province = "Tucumán", City = "San Miguel de Tucumán",
            Name = "Departamento Céntrico — San Miguel de Tucumán",
            Description = "Moderno departamento en el centro de San Miguel de Tucumán, a media cuadra de la Plaza Independencia, donde el 9 de julio de 1816 se declaró la Independencia Argentina. La capital del azúcar, el limón y el dulce de leche. Acceso a toda la gastronomía y vida cultural tucumana.",
            Address = "25 de Mayo 456, San Miguel de Tucumán",
            PropertyType = PropertyType.Departamento, IsActive = true, IsApproved = true,
        };
        p26.Photos.Add(new Photo { Url = "https://picsum.photos/seed/tuc-p26-a/900/600", PublicId = "demo/tuc-p26-a", AltText = "Departamento moderno en Tucumán", IsCover = true, DisplayOrder = 0 });
        AddRoom(p26, "Departamento Completo — 2 Personas",
            "Departamento de 1 ambiente con cama matrimonial, living, kitchenette equipada, baño completo y balcón con vista a la Plaza Independencia.",
            2, 9500m, "tuc-r26a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Cocina equipada"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Escritorio de trabajo"));
        db.Properties.Add(p26);

        var p27 = new Property
        {
            OwnerId = owner.Id, Province = "Tucumán", City = "Amaicha del Valle",
            Name = "Casa de Adobe — Amaicha del Valle",
            Description = "Casa de adobe con arquitectura precolombina en Amaicha del Valle, cuna de la cultura diaguita calchaquí. A 1.900 metros de altura, con 300 días de sol al año. Sede del famoso Carnaval de Amaicha (febrero). Rodeada de cactus cardones y paisajes áridos de gran belleza ancestral.",
            Address = "Av. Cacique Zapata 678, Amaicha del Valle",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p27.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ama-p27-a/900/600", PublicId = "demo/ama-p27-a", AltText = "Casa de adobe en Amaicha", IsCover = true, DisplayOrder = 0 });
        AddRoom(p27, "Habitación con Patio de Cardones",
            "Habitación de adobe con cama matrimonial, baño privado y patio privado rodeado de cactus cardones y vista a los cerros multicolores.",
            2, 8500m, "ama-r27a", 3,
            Get("Calefacción"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Mascotas permitidas"));
        AddRoom(p27, "Habitación Triple — Familia",
            "Habitación con cama matrimonial y una individual, baño compartido y acceso al patio de cardones y la galería de adobe.",
            3, 11000m, "ama-r27b", 2,
            Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p27);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Tucumán — 3 propiedades guardadas");

        // ══════════════════════════════════════════════════════════════════
        // PROVINCIA 10: SANTA FE (Litoral y Pampa)
        // ══════════════════════════════════════════════════════════════════

        var p28 = new Property
        {
            OwnerId = owner.Id, Province = "Santa Fe", City = "Rosario",
            Name = "Departamento Frente al Paraná — Rosario",
            Description = "Moderno departamento en el barrio de Pichincha, Rosario, a 200 metros del Río Paraná. En la ciudad de Ernesto 'Che' Guevara y Lionel Messi. Vistas al río desde el living y el dormitorio. A metros de la Costanera, el Monumento a la Bandera y la mejor gastronomía rosarina.",
            Address = "Av. Belgrano 1234, Rosario",
            PropertyType = PropertyType.Departamento, IsActive = true, IsApproved = true,
        };
        p28.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ros-p28-a/900/600", PublicId = "demo/ros-p28-a", AltText = "Departamento con vista al Paraná", IsCover = true,  DisplayOrder = 0 });
        p28.Photos.Add(new Photo { Url = "https://picsum.photos/seed/ros-p28-b/900/600", PublicId = "demo/ros-p28-b", AltText = "Living con vista al río",           IsCover = false, DisplayOrder = 1 });
        AddRoom(p28, "Suite Frente al Paraná",
            "Dormitorio principal con cama king, ventanal con vista panorámica al Río Paraná, baño en suite con ducha de lluvia y vestidor.",
            2, 13500m, "ros-r28a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Escritorio de trabajo"), Get("Balcón / Terraza"), Get("Caja fuerte"));
        AddRoom(p28, "Habitación Doble Vista al Río",
            "Habitación con cama matrimonial, baño privado y vista al río desde el piso 12. Ideal para trabajo o turismo en Rosario.",
            2, 10500m, "ros-r28b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Escritorio de trabajo"));
        db.Properties.Add(p28);

        var p29 = new Property
        {
            OwnerId = owner.Id, Province = "Santa Fe", City = "Santa Fe",
            Name = "Casa Histórica — Santa Fe Capital",
            Description = "Casa colonial restaurada en el casco histórico de Santa Fe, a metros de la catedral y la Casa de Gobierno. La ciudad donde se firmó la Constitución Nacional de 1853. Patio interno con fuente de la época, galería de ladrillo y jardín histórico. A 170 km de Rosario.",
            Address = "San Jerónimo 1890, Santa Fe",
            PropertyType = PropertyType.Casa, IsActive = true, IsApproved = true,
        };
        p29.Photos.Add(new Photo { Url = "https://picsum.photos/seed/stf-p29-a/900/600", PublicId = "demo/stf-p29-a", AltText = "Casa histórica colonial", IsCover = true, DisplayOrder = 0 });
        AddRoom(p29, "Habitación Principal con Patio Colonial",
            "Habitación con cama king, piso de madera, techos altos y acceso al patio interno colonial con fuente de mosaico del siglo XIX.",
            2, 9500m, "stf-r29a", 3,
            Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Escritorio de trabajo"));
        AddRoom(p29, "Habitación Doble con Vista al Jardín",
            "Habitación doble con dos camas individuales, baño compartido y ventana al jardín histórico con naranjos.",
            2, 7000m, "stf-r29b", 2,
            Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));
        db.Properties.Add(p29);

        var p30 = new Property
        {
            OwnerId = owner.Id, Province = "Santa Fe", City = "Cayastá",
            Name = "Cabaña Litoral — Cayastá",
            Description = "Cabaña a orillas del Río San Javier en Cayastá, sitio donde se fundó la primera Santa Fe en 1573. Rodeada de islas, selvas en galería y playas de arena del litoral santafesino. Pesca artesanal de dorado y surubí, avistaje de aves carpinteras y garzas reales.",
            Address = "Acceso Costanero km 3, Cayastá",
            PropertyType = PropertyType.Cabana, IsActive = true, IsApproved = true,
        };
        p30.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cay-p30-a/900/600", PublicId = "demo/cay-p30-a", AltText = "Cabaña a orillas del río San Javier", IsCover = true, DisplayOrder = 0 });
        AddRoom(p30, "Cabaña Completa con Muelle — 4 Personas",
            "Cabaña con cama matrimonial, dos individuales, cocina equipada, baño completo, galería con hamacas y muelle propio para pesca y kayak.",
            4, 15500m, "cay-r30a", 4,
            Get("Cocina equipada"), Get("Estacionamiento"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"), Get("Parrilla / Asador"));
        AddRoom(p30, "Habitación Doble Ribereña",
            "Habitación independiente con cama matrimonial, baño privado y galería con vista al río y las islas.",
            2, 9500m, "cay-r30b", 2,
            Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Mascotas permitidas"));
        db.Properties.Add(p30);

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Santa Fe — 3 propiedades guardadas. Total: 30 propiedades.");
    }

    private static void AddRoom(Property prop, string title, string description,
        int capacity, decimal price, string photoSeed, int photoCount,
        params Amenity?[] amenities)
    {
        var room = new Room
        {
            Title         = title,
            Description   = description,
            Capacity      = capacity,
            PricePerNight = price,
            IsActive      = true,
        };
        for (var i = 0; i < photoCount; i++)
            room.Photos.Add(new Photo
            {
                Url          = $"https://picsum.photos/seed/{photoSeed}-{i}/800/600",
                PublicId     = $"demo/{photoSeed}-{i}",
                AltText      = title,
                IsCover      = i == 0,
                DisplayOrder = i,
            });
        foreach (var a in amenities)
            if (a is not null)
                room.RoomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = a.Id });
        prop.Rooms.Add(room);
    }
}
