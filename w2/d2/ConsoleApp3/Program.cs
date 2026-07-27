using System;
using System.Collections.Generic;
using System.Linq;

// Customer Class
class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Order> Orders { get; set; } = new();
}

// Order Class
class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public double Amount { get; set; }
}

class Program
{
    static void Main()
    {
        // Customers
        List<Customer> customers = new()
        {
            new Customer { Id = 1, Name = "Ali" },
            new Customer { Id = 2, Name = "Sara" },
            new Customer { Id = 3, Name = "Lina" }
        };

        // Orders 
        List<Order> orders = new()
        {
            new Order { Id = 1, CustomerId = 1, Amount = 100 },
            new Order { Id = 2, CustomerId = 1, Amount = 150 },
            new Order { Id = 3, CustomerId = 2, Amount = 200 },
            new Order { Id = 4, CustomerId = 2, Amount = 50 },
            new Order { Id = 5, CustomerId = 3, Amount = 300 },
            new Order { Id = 6, CustomerId = 3, Amount = 100 }
        };

        foreach (var customer in customers)
        {
            customer.Orders = orders
                .Where(o => o.CustomerId == customer.Id)
                .ToList();
        }

        // GroupBy
        Console.WriteLine("=== GroupBy ===");

        var grouped = orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Total = g.Sum(x => x.Amount)
            });

        foreach (var item in grouped)
        {
            Console.WriteLine($"Customer {item.CustomerId} -> Total = {item.Total}");
        }

        // Join
        Console.WriteLine("\n=== Join ===");

        var joined = customers.Join(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (c, o) => new
            {
                c.Name,
                o.Amount
            });

        foreach (var item in joined)
        {
            Console.WriteLine($"{item.Name} : {item.Amount}");
        }

       
        // SelectMany
        Console.WriteLine("\n=== SelectMany ===");

        var allOrders = customers.SelectMany(c => c.Orders);

        foreach (var order in allOrders)
        {
            Console.WriteLine($"Order {order.Id} : {order.Amount}");
        }

        //  Deferred Execution
        Console.WriteLine("\n=== Deferred Execution ===");

        var expensiveOrders = orders.Where(o => o.Amount > 100);

        orders.Add(new Order
        {
            Id = 7,
            CustomerId = 1,
            Amount = 500
        });

        foreach (var order in expensiveOrders)
        {
            Console.WriteLine(order.Amount);
        }
    }
}