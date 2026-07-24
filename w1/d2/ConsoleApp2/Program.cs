using System;

//task 1
int age = 25;
bool isStudent = true;
double gpa = 3.8;

string name = "Leen";
int[] scores = { 90, 85, 77 };
object obj = new object();

Console.WriteLine($"age is {age.GetType()}");
Console.WriteLine($"isStudent is {isStudent.GetType()}");
Console.WriteLine($"gpa is {gpa.GetType()}");
Console.WriteLine($"name is {name.GetType()}");
Console.WriteLine($"scores is {scores.GetType()}");
Console.WriteLine($"obj is {obj.GetType()}");

//task 2
static void ChangeValue(int num)
{
    num = 999;
}

static void ChangeArray(int[] arr)
{
    arr[0] = 999;
}
int number = 10;
Console.WriteLine($"Before: {number}");
ChangeValue(number);
Console.WriteLine($"After: {number}"); 

int[] numbers = { 1, 2, 3 };
Console.WriteLine($"Before ChangeArray: {numbers[0]}");
ChangeArray(numbers);
Console.WriteLine($"After ChangeArray: {numbers[0]}"); // تغيرت!


//task 3

Console.WriteLine(DescribeGrade(95)); // Excellent
Console.WriteLine(DescribeGrade(75)); // Proficient
Console.WriteLine(DescribeGrade(55)); // Developing
Console.WriteLine(DescribeGrade(30)); // Below Standard

static string DescribeGrade(int score) => score switch
{
    >= 90 => "Excellent",
    >= 70 => "Proficient",
    >= 50 => "Developing",
    _ => "Below Standard"
};

//task 4

Console.Write("Enter your name: ");
string? input = Console.ReadLine();

if (!string.IsNullOrEmpty(input))
{
    Console.WriteLine($"Hello, {input}!");
}
else
{
    Console.WriteLine("No name entered.");
}