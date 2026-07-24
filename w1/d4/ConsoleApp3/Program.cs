using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Book class from Day 3
public class Book
{
    public string Title { get; private set; }
    public string Genre { get; private set; }
    public double Price { get; private set; }
    public bool IsBorrowed { get; set; }

    public Book(string title, string genre, double price, bool isBorrowed)
    {
        Title = title;
        Genre = genre;
        Price = price;
        IsBorrowed = isBorrowed;
    }
}

class Program
{
    static async Task Main()
    {
        // Task 1 
        List<Book> books = new List<Book>
        {
            new Book("Harry Potter", "Fantasy", 15.99, true),
            new Book("1984", "Dystopian", 10.50, false),
            new Book("The Hobbit", "Fantasy", 12.00, true),
            new Book("Dune", "Sci-Fi", 18.75, false),
            new Book("Clean Code", "Programming", 25.00, true),
            new Book("Atomic Habits", "Self-Help", 14.20, false),
            new Book("The Alchemist", "Fiction", 9.99, true),
            new Book("Sapiens", "History", 16.40, false)
        };

        Console.WriteLine("All Books");
        foreach (Book b in books)
        {
            Console.WriteLine(b.Title + " - " + b.Genre + " - $" + b.Price);
        }

        // Task 2 
        var borrowedBooks = books.Where(b => b.IsBorrowed).ToList();
        Console.WriteLine("\nBorrowed Books");
        foreach (Book b in borrowedBooks)
        {
            Console.WriteLine(b.Title);
        }

        var titles = books.Select(b => b.Title).ToList();
        Console.WriteLine("\nBook Titles");
        foreach (string title in titles)
        {
            Console.WriteLine(title);
        }

        double averagePrice = books.Average(b => b.Price);
        Console.WriteLine("\nAverage Price");
        Console.WriteLine("Average price: $" + averagePrice.ToString("F2"));

        // Task 3

        static async Task<string> SimulateApiCall()
        {
        await Task.Delay(2000); 
        return "successfully!";
        }
        Console.WriteLine("\nFetching Data");
        Console.WriteLine("Fetching data...");
        string result = await SimulateApiCall();
        Console.WriteLine(result);

        // Task 4 
        Console.WriteLine("\nInput Validation");
        Console.Write("Enter a number: ");
        string? userInput = Console.ReadLine();

        bool isValid = int.TryParse(userInput, out int number);

        if (isValid)
        {
            Console.WriteLine("entered: " + number);
        }
        else
        {
            Console.WriteLine("not a valid");
        }
    }

   
}