using BrbHabitaciones.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Data.Seed;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Amenities.AnyAsync())
        {
            var amenities = new List<Amenity>
            {
                new() { Name = "WiFi",                          Icon = "wifi",              Category = "Conectividad" },
                new() { Name = "Estacionamiento",               Icon = "car",               Category = "Servicios" },
                new() { Name = "Aire acondicionado",            Icon = "snowflake",         Category = "Climatización" },
                new() { Name = "Calefacción",                   Icon = "thermometer",       Category = "Climatización" },
                new() { Name = "TV",                            Icon = "television",        Category = "Entretenimiento" },
                new() { Name = "Cocina equipada",               Icon = "cooking-pot",       Category = "Cocina" },
                new() { Name = "Lavarropas",                    Icon = "washing-machine",   Category = "Servicios" },
                new() { Name = "Piscina",                       Icon = "swimming-pool",     Category = "Exteriores" },
                new() { Name = "Parrilla / Asador",             Icon = "grill",             Category = "Exteriores" },
                new() { Name = "Ropa de cama incluida",         Icon = "bed",               Category = "Habitación" },
                new() { Name = "Toallas incluidas",             Icon = "towel",             Category = "Habitación" },
                new() { Name = "Balcón / Terraza",              Icon = "balcony",           Category = "Exteriores" },
                new() { Name = "Jardín",                        Icon = "plant",             Category = "Exteriores" },
                new() { Name = "Mascotas permitidas",           Icon = "paw-print",         Category = "Normas" },
                new() { Name = "Desayuno incluido",             Icon = "coffee",            Category = "Servicios" },
                new() { Name = "Caja fuerte",                   Icon = "lock-key",          Category = "Seguridad" },
                new() { Name = "Escritorio de trabajo",         Icon = "desk",              Category = "Trabajo" },
                new() { Name = "Acceso para silla de ruedas",   Icon = "wheelchair",        Category = "Accesibilidad" },
                new() { Name = "Detector de humo",              Icon = "fire",              Category = "Seguridad" },
                new() { Name = "Extintor de incendios",         Icon = "fire-extinguisher", Category = "Seguridad" },
            };

            db.Amenities.AddRange(amenities);
            await db.SaveChangesAsync();
        }

        await DemoSeedData.SeedAsync(db);
    }
}
