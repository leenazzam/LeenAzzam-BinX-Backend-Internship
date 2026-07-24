using System;

// Book
public class Book
{
    public string Title { get; private set; }
    public bool IsBorrowed { get; set; }

    public Book(string bookTitle)
    {
        if (string.IsNullOrEmpty(bookTitle))
        {
            Console.WriteLine("Error: Book must have a title");
            Title = "Unknown";
        }
        else
        {
            Title = bookTitle;
        }

        IsBorrowed = false;
    }
}

//Member
public class Member
{
    public string Name { get; private set; }

    public Member(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Error: Member must have a name");
            Name = "Unknown";
        }
        else
        {
            Name = name;
        }
    }
}

//Record
public record BorrowRequest(string BookTitle, string MemberName, DateTime Date);

//Interface
public interface INotifiable
{
    void Notify(string message);
}

// Book can be notified
public class NotifiableBook : Book, INotifiable
{
    public NotifiableBook(string title) : base(title) { }

    public void Notify(string message)
    {
        Console.WriteLine("Book alert: " + message);
    }
}

// Member can be notified 
public class NotifiableMember : Member, INotifiable
{
    public NotifiableMember(string name) : base(name) { }

    public void Notify(string message)
    {
        Console.WriteLine("Member alert: " + message);
    }
}

class Program
{
    static void Main()
    {
        // Task 1
        Book book1 = new Book("Harry Potter");
        Member member1 = new Member("Leen");

        Console.WriteLine(book1.Title + " - Borrowed: " + book1.IsBorrowed);
        Console.WriteLine("Member name: " + member1.Name);

        // Task 2
        BorrowRequest request = new BorrowRequest("Harry Potter", "Leen", DateTime.Now);
        Console.WriteLine(request.BookTitle + " borrowed by " + request.MemberName);

        // Task 4
        NotifiableBook nb = new NotifiableBook("1984");
        NotifiableMember nm = new NotifiableMember("Sara");

        SendAlert(nb);
        SendAlert(nm);

        //invalid input
        Book invalidBook = new Book("");
        Console.WriteLine("Invalid book title is now: " + invalidBook.Title);
    }

    // works for any class that implements INotifiable
    static void SendAlert(INotifiable target)
    {
        target.Notify("This is a reminder!");
    }
}