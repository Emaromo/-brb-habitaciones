using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Data.Seed;

public static class DemoSeedData
{
    private const string DemoOwnerEmail = "dueno@brbhabitaciones.com";
    private const string DemoOwnerPassword = "Demo1234!";

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Properties.AnyAsync()) return;

        // ── Usuario dueño ──────────────────────────────────────────────
        var owner = new User
        {
            Email    = DemoOwnerEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoOwnerPassword),
            FirstName = "Carlos",
            LastName  = "Rodríguez",
            Phone     = "+54 11 4567-8901",
            Role      = UserRole.DuenoAlojamiento,
        };
        db.Users.Add(owner);
        await db.SaveChangesAsync();

        // ── Amenities ya seedeados — tomamos los que necesitamos ───────
        var amenities = await db.Amenities.ToDictionaryAsync(a => a.Name);

        Amenity? Get(string name) => amenities.GetValueOrDefault(name);

        // ══════════════════════════════════════════════════════════════
        // PROPIEDAD 1 — Casa en Los Cardales, Buenos Aires
        // ══════════════════════════════════════════════════════════════
        var p1 = new Property
        {
            OwnerId      = owner.Id,
            Name         = "Casa de Campo Los Cardales",
            Description  = "Amplia casa de campo en los alrededores de Los Cardales, a 70 km de Capital Federal. Ideal para escapadas de fin de semana en familia. Cuenta con jardín, pileta y parrilla bajo techo. Entorno rural con vistas al campo.",
            Province     = "Buenos Aires",
            City         = "Los Cardales",
            Address      = "Calle Las Acacias 1240, Los Cardales",
            PropertyType = PropertyType.Casa,
            IsActive     = true,
            IsApproved   = true,
        };
        p1.Photos.Add(new Photo { Url = "https://picsum.photos/seed/bsas-prop1-a/900/600", PublicId = "demo/bsas-prop1-a", AltText = "Vista exterior de la casa",            IsCover = true,  DisplayOrder = 0 });
        p1.Photos.Add(new Photo { Url = "https://picsum.photos/seed/bsas-prop1-b/900/600", PublicId = "demo/bsas-prop1-b", AltText = "Jardín y parrilla",                    IsCover = false, DisplayOrder = 1 });
        p1.Photos.Add(new Photo { Url = "https://picsum.photos/seed/bsas-prop1-c/900/600", PublicId = "demo/bsas-prop1-c", AltText = "Living comedor",                        IsCover = false, DisplayOrder = 2 });

        var r1a = new Room
        {
            PropertyId   = p1.Id,
            Title        = "Habitación Principal con Baño en Suite",
            Description  = "Dormitorio principal con cama king, baño privado con bañadera, vestidor y acceso directo al jardín. Vista panorámica al campo.",
            Capacity     = 2,
            PricePerNight = 9500m,
            IsActive     = true,
        };
        AddPhotos(r1a, "bsas-r1a", 3);
        AddAmenities(r1a, Get("WiFi"), Get("Aire acondicionado"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("TV"));

        var r1b = new Room
        {
            PropertyId   = p1.Id,
            Title        = "Habitación Doble con Vista al Jardín",
            Description  = "Habitación doble con dos camas individuales o una cama matrimonial, baño compartido y vista directa al jardín y la pileta.",
            Capacity     = 2,
            PricePerNight = 6800m,
            IsActive     = true,
        };
        AddPhotos(r1b, "bsas-r1b", 2);
        AddAmenities(r1b, Get("WiFi"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"));

        var r1c = new Room
        {
            PropertyId   = p1.Id,
            Title        = "Habitación Simple — Cuarto de Servicios",
            Description  = "Habitación simple ideal para una persona o niño. Cama de una plaza, armario amplio y ventana al patio trasero.",
            Capacity     = 1,
            PricePerNight = 4200m,
            IsActive     = true,
        };
        AddPhotos(r1c, "bsas-r1c", 2);
        AddAmenities(r1c, Get("WiFi"), Get("Ropa de cama incluida"));

        p1.Rooms.Add(r1a);
        p1.Rooms.Add(r1b);
        p1.Rooms.Add(r1c);
        db.Properties.Add(p1);

        // ══════════════════════════════════════════════════════════════
        // PROPIEDAD 2 — Posada en Villa General Belgrano, Córdoba
        // ══════════════════════════════════════════════════════════════
        var p2 = new Property
        {
            OwnerId      = owner.Id,
            Name         = "Posada del Valle — Villa General Belgrano",
            Description  = "Encantadora posada de estilo bávaro en el corazón de Villa General Belgrano, Córdoba. A metros del centro de la localidad más alemana de Argentina. Desayuno incluido, jardín serrano y ambiente familiar.",
            Province     = "Córdoba",
            City         = "Villa General Belgrano",
            Address      = "Av. Julio Roca 854, Villa General Belgrano",
            PropertyType = PropertyType.Posada,
            IsActive     = true,
            IsApproved   = true,
        };
        p2.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cba-prop2-a/900/600", PublicId = "demo/cba-prop2-a", AltText = "Fachada de la posada",          IsCover = true,  DisplayOrder = 0 });
        p2.Photos.Add(new Photo { Url = "https://picsum.photos/seed/cba-prop2-b/900/600", PublicId = "demo/cba-prop2-b", AltText = "Jardín serrano",               IsCover = false, DisplayOrder = 1 });

        var r2a = new Room
        {
            PropertyId   = p2.Id,
            Title        = "Suite Premium con Jacuzzi",
            Description  = "Suite de lujo con cama king, jacuzzi privado, calefacción central y balcón con vista a las sierras. Desayuno buffet incluido.",
            Capacity     = 2,
            PricePerNight = 14500m,
            IsActive     = true,
        };
        AddPhotos(r2a, "cba-r2a", 3);
        AddAmenities(r2a, Get("WiFi"), Get("Calefacción"), Get("Aire acondicionado"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"), Get("Balcón / Terraza"));

        var r2b = new Room
        {
            PropertyId   = p2.Id,
            Title        = "Habitación Estándar Doble",
            Description  = "Habitación doble con cama matrimonial, baño privado con ducha, calefacción y vista al jardín. Desayuno continental incluido.",
            Capacity     = 2,
            PricePerNight = 8900m,
            IsActive     = true,
        };
        AddPhotos(r2b, "cba-r2b", 2);
        AddAmenities(r2b, Get("WiFi"), Get("Calefacción"), Get("TV"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Desayuno incluido"));

        p2.Rooms.Add(r2a);
        p2.Rooms.Add(r2b);
        db.Properties.Add(p2);

        // ══════════════════════════════════════════════════════════════
        // PROPIEDAD 3 — Cabaña en Chacras de Coria, Mendoza
        // ══════════════════════════════════════════════════════════════
        var p3 = new Property
        {
            OwnerId      = owner.Id,
            Name         = "Cabaña Viñas del Sur — Chacras de Coria",
            Description  = "Cabaña privada en Chacras de Coria, a minutos de las bodegas más importantes de Mendoza. Rodeada de viñedos, con deck privado, parrilla y vista a la Cordillera de los Andes. Perfecta para parejas o grupos pequeños que buscan desconectarse.",
            Province     = "Mendoza",
            City         = "Chacras de Coria",
            Address      = "Calle Carril Rodríguez Peña 3210, Chacras de Coria",
            PropertyType = PropertyType.Cabana,
            IsActive     = true,
            IsApproved   = true,
        };
        p3.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mdz-prop3-a/900/600", PublicId = "demo/mdz-prop3-a", AltText = "Cabaña con vista a los viñedos", IsCover = true,  DisplayOrder = 0 });
        p3.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mdz-prop3-b/900/600", PublicId = "demo/mdz-prop3-b", AltText = "Deck privado al atardecer",       IsCover = false, DisplayOrder = 1 });
        p3.Photos.Add(new Photo { Url = "https://picsum.photos/seed/mdz-prop3-c/900/600", PublicId = "demo/mdz-prop3-c", AltText = "Interior de la cabaña",           IsCover = false, DisplayOrder = 2 });

        var r3a = new Room
        {
            PropertyId   = p3.Id,
            Title        = "Cabaña Completa — Hasta 4 personas",
            Description  = "Cabaña entera con dormitorio principal (cama king), cama marinera en el altillo, living comedor, cocina equipada, baño completo y deck privado con parrilla. Capacidad total para 4 personas.",
            Capacity     = 4,
            PricePerNight = 17500m,
            IsActive     = true,
        };
        AddPhotos(r3a, "mdz-r3a", 4);
        AddAmenities(r3a, Get("WiFi"), Get("Cocina equipada"), Get("Parrilla / Asador"), Get("Estacionamiento"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("TV"), Get("Balcón / Terraza"), Get("Mascotas permitidas"));

        var r3b = new Room
        {
            PropertyId   = p3.Id,
            Title        = "Loft Superior — Para 2 personas",
            Description  = "Loft moderno en planta alta con cama queen, baño en suite, minifridge, cafetera y balcón privado con vista a los viñedos y la Cordillera.",
            Capacity     = 2,
            PricePerNight = 11200m,
            IsActive     = true,
        };
        AddPhotos(r3b, "mdz-r3b", 3);
        AddAmenities(r3b, Get("WiFi"), Get("Aire acondicionado"), Get("Calefacción"), Get("Ropa de cama incluida"), Get("Toallas incluidas"), Get("Balcón / Terraza"), Get("Caja fuerte"));

        p3.Rooms.Add(r3a);
        p3.Rooms.Add(r3b);
        db.Properties.Add(p3);

        await db.SaveChangesAsync();
    }

    private static void AddPhotos(Room room, string seed, int count)
    {
        for (var i = 0; i < count; i++)
            room.Photos.Add(new Photo
            {
                Url          = $"https://picsum.photos/seed/{seed}-{i}/800/600",
                PublicId     = $"demo/{seed}-{i}",
                AltText      = room.Title,
                IsCover      = i == 0,
                DisplayOrder = i,
            });
    }

    private static void AddAmenities(Room room, params Amenity?[] amenities)
    {
        foreach (var a in amenities)
            if (a is not null)
                room.RoomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = a.Id });
    }
}
