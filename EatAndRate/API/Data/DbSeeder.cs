using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var pizzaKing = await db.Franchises
            .FirstOrDefaultAsync(f => f.Name == "Pizza King");

        if (pizzaKing is null)
        {
            pizzaKing = new Franchise { Name = "Pizza King" };
            db.Franchises.Add(pizzaKing);
            await db.SaveChangesAsync();
        }

        var burgerHouse = await db.Franchises
            .FirstOrDefaultAsync(f => f.Name == "Burger House");

        if (burgerHouse is null)
        {
            burgerHouse = new Franchise { Name = "Burger House" };
            db.Franchises.Add(burgerHouse);
            await db.SaveChangesAsync();
        }

        var alice = await db.Users
            .FirstOrDefaultAsync(u => u.Username == "alice");

        if (alice is null)
        {
            alice = new User
            {
                Username = "alice",
                Email = "alice@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };

            db.Users.Add(alice);
            await db.SaveChangesAsync();
        }

        var bob = await db.Users
            .FirstOrDefaultAsync(u => u.Username == "bob");

        if (bob is null)
        {
            bob = new User
            {
                Username = "bob",
                Email = "bob@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };

            db.Users.Add(bob);
            await db.SaveChangesAsync();
        }

        var bellaNapoli = await db.Restaurants
            .FirstOrDefaultAsync(r => r.Name == "Bella Napoli");

        if (bellaNapoli is null)
        {
            bellaNapoli = new Restaurant
            {
                Name = "Bella Napoli",
                Address = "Main Street 12",
                City = "Budapest",
                FranchiseId = pizzaKing.Id
            };

            db.Restaurants.Add(bellaNapoli);
            await db.SaveChangesAsync();
        }

        var downtownBurger = await db.Restaurants
            .FirstOrDefaultAsync(r => r.Name == "Downtown Burger");

        if (downtownBurger is null)
        {
            downtownBurger = new Restaurant
            {
                Name = "Downtown Burger",
                Address = "Kossuth Street 5",
                City = "Budapest",
                FranchiseId = burgerHouse.Id
            };

            db.Restaurants.Add(downtownBurger);
            await db.SaveChangesAsync();
        }

        if (!await db.Reviews.AnyAsync(r => r.UserId == alice.Id &&
                                            r.RestaurantId == bellaNapoli.Id))
        {
            db.Reviews.Add(new Review
            {
                UserId = alice.Id,
                RestaurantId = bellaNapoli.Id,
                FoodRating = 5,
                ServiceRating = 4,
                AtmosphereRating = 5,
                Comment = "Excellent pizza and a pleasant atmosphere."
            });
        }

        if (!await db.Reviews.AnyAsync(r => r.UserId == bob.Id &&
                                            r.RestaurantId == bellaNapoli.Id))
        {
            db.Reviews.Add(new Review
            {
                UserId = bob.Id,
                RestaurantId = bellaNapoli.Id,
                FoodRating = 4,
                ServiceRating = 5,
                AtmosphereRating = 4,
                Comment = "Very good food and friendly service."
            });
        }

        if (!await db.Reviews.AnyAsync(r => r.UserId == alice.Id &&
                                            r.RestaurantId == downtownBurger.Id))
        {
            db.Reviews.Add(new Review
            {
                UserId = alice.Id,
                RestaurantId = downtownBurger.Id,
                FoodRating = 4,
                ServiceRating = 4,
                AtmosphereRating = 3,
                Comment = "Good burger, but the restaurant was noisy."
            });
        }

        var chainRestaurants = new (string Franchise, string Name, string Address, string City)[]
        {
            ("McDonald's", "McDonald's Nyugati", "Teréz körút 62.", "Budapest"),
            ("McDonald's", "McDonald's Szeged Belváros", "Kárász utca 5.", "Szeged"),
            ("McDonald's", "McDonald's Debrecen Piac", "Piac utca 20.", "Debrecen"),

            ("Burger King", "Burger King Blaha", "Blaha Lujza tér 1.", "Budapest"),
            ("Burger King", "Burger King Győr", "Baross Gábor út 10.", "Győr"),
            ("Burger King", "Burger King Pécs", "Király utca 8.", "Pécs"),

            ("KFC", "KFC Oktogon", "Oktogon 3.", "Budapest"),
            ("KFC", "KFC Debrecen", "Csapó utca 30.", "Debrecen"),
            ("KFC", "KFC Miskolc", "Széchenyi István út 15.", "Miskolc"),

            ("Jóasszony", "Jóasszony Váci út", "Váci út 25.", "Budapest"),
            ("Jóasszony", "Jóasszony Rákóczi út", "Rákóczi út 12.", "Budapest"),
            ("Jóasszony", "Jóasszony Szeged", "Tisza Lajos körút 40.", "Szeged"),

            ("Simon's Burger", "Simon's Burger Belváros", "Király utca 25.", "Budapest"),
            ("Simon's Burger", "Simon's Burger Győr", "Czuczor Gergely utca 12.", "Győr"),
            ("Simon's Burger", "Simon's Burger Veszprém", "Óváros tér 6.", "Veszprém"),

            ("Starbucks", "Starbucks Váci utca", "Váci utca 18.", "Budapest"),
            ("Starbucks", "Starbucks Andrássy", "Andrássy út 30.", "Budapest"),
            ("Starbucks", "Starbucks Debrecen", "Piac utca 45.", "Debrecen"),

            ("Bamba Marha", "Bamba Marha Október 6.", "Október 6. utca 19.", "Budapest"),
            ("Bamba Marha", "Bamba Marha Corvin", "Futó utca 37.", "Budapest"),
            ("Bamba Marha", "Bamba Marha Szeged", "Dugonics tér 9.", "Szeged"),

            ("Bellozzo", "Bellozzo Allee", "Október huszonharmadika utca 8.", "Budapest"),
            ("Bellozzo", "Bellozzo Pécs", "Bajcsy-Zsilinszky utca 11.", "Pécs"),
            ("Bellozzo", "Bellozzo Miskolc", "Szentpáli utca 2.", "Miskolc"),

            ("Zing", "Zing Astoria", "Károly körút 12.", "Budapest"),
            ("Zing", "Zing Szeged", "Kárász utca 14.", "Szeged"),
            ("Zing", "Zing Győr", "Baross Gábor út 22.", "Győr"),

            ("Pizza Hut", "Pizza Hut Nagykörút", "Erzsébet körút 30.", "Budapest"),
            ("Pizza Hut", "Pizza Hut Debrecen", "Piac utca 33.", "Debrecen"),
            ("Pizza Hut", "Pizza Hut Pécs", "Rákóczi út 20.", "Pécs"),

            ("Subway", "Subway Deák tér", "Deák Ferenc tér 4.", "Budapest"),
            ("Subway", "Subway Miskolc", "Széchenyi István út 40.", "Miskolc"),
            ("Subway", "Subway Veszprém", "Kossuth Lajos utca 7.", "Veszprém"),

            ("Popeyes", "Popeyes Corvin", "Üllői út 45.", "Budapest"),
            ("Popeyes", "Popeyes Szeged", "Tisza Lajos körút 22.", "Szeged"),
            ("Popeyes", "Popeyes Debrecen", "Csapó utca 12.", "Debrecen")
        };

        foreach (var item in chainRestaurants)
        {
            var franchise = await db.Franchises
                .FirstOrDefaultAsync(f => f.Name == item.Franchise);

            if (franchise is null)
            {
                franchise = new Franchise { Name = item.Franchise };
                db.Franchises.Add(franchise);
                await db.SaveChangesAsync();
            }

            if (!await db.Restaurants.AnyAsync(r => r.Name == item.Name))
            {
                db.Restaurants.Add(new Restaurant
                {
                    Name = item.Name,
                    Address = item.Address,
                    City = item.City,
                    FranchiseId = franchise.Id
                });
                await db.SaveChangesAsync();
            }
        }

        await db.SaveChangesAsync();
    }
}
