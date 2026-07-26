using System;
using System.Collections.Generic;

// Task 1 & 2

public class Repository<T> where T : class// This means T must be a class (reference type), not int or double.
{
    private List<T> items = new List<T>();

    // Add a new item
    public void Add(T item)
    {
        items.Add(item);
    }

    // Return all items as read-only
    public IReadOnlyList<T> GetAll()
    {
        return items.AsReadOnly();
    }

    // Find items that match a condition
    public List<T> Find(Predicate<T> condition)
    {
        return items.FindAll(condition);
    }
}

// Product class
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"Product #{Id}: {Name} - ${Price}";
    }
}

// Customer class
public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }

    public override string ToString()
    {
        return $"Customer #{Id}: {FullName}";
    }
}

class Program
{
    static void Main()
    {
        // Task 3 Product
        Repository<Product> productRepo = new Repository<Product>();

        productRepo.Add(new Product { Id = 1, Name = "Laptop", Price = 950 });
        productRepo.Add(new Product { Id = 2, Name = "Mouse", Price = 25 });
        productRepo.Add(new Product { Id = 3, Name = "Keyboard", Price = 60 });

        Console.WriteLine("All Products:");
        foreach (Product p in productRepo.GetAll())
        {
            Console.WriteLine(p);
        }

        Console.WriteLine("\nProducts over $50:");
        foreach (Product p in productRepo.Find(x => x.Price > 50))
        {
            Console.WriteLine(p);
        }

        // Task 3 Customer
        Repository<Customer> customerRepo = new Repository<Customer>();

        customerRepo.Add(new Customer { Id = 1, FullName = "Ahmad Yousef" });
        customerRepo.Add(new Customer { Id = 2, FullName = "Lina Saleh" });

        Console.WriteLine("\nAll Customers:");
        foreach (Customer c in customerRepo.GetAll())
        {
            Console.WriteLine(c);
        }

        // Task 4
        IReadOnlyList<Product> allProducts = productRepo.GetAll();

        Console.WriteLine("\nProduct Count: " + allProducts.Count);

        // error because IReadOnlyList is read-only.
        // allProducts.Add(new Product());
    }
}