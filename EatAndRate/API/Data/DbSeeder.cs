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

        await db.SaveChangesAsync();
    }
}
