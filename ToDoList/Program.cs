using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ToDoList
{
    class Program
    {
        static void Main(string[] args)
        {
            string input; // generic placeholder used to read in text for temporary processes
            string taskName; // used to store a task name for dictionary entry/access/deletion as the primary key
            string description; // used to store a task description for dictionary entry 
            string dueDate; // string to store due date for task
            DateTime date; // optional due date for task
            // dictionary used to store tasks (keys) and descriptions
            Dictionary<string, Item> taskList = new Dictionary<string, Item>();

            Console.WriteLine("Welcome to your to-do list");

            if (taskList.Count == 0)
            {
                Console.WriteLine("You currently have no current tasks, would you like to add a task?");
                input = Console.ReadLine();
                bool exitApp = YesNo(input);
                // exit the application if the user answers no
                if (!exitApp)
                {
                    // wait 3 seconds and close the program
                    Console.Write("Goodbye");
                    Thread.Sleep(2000);
                    System.Environment.Exit(1);
                }
                // get task information
                Console.Write("Enter a name for your task: ");
                taskName = Console.ReadLine();
                Console.Write("Enter a description for your task: ");
                Item newTask = newItem();
                taskList.Add(taskName, newTask);
            }
            string temp = taskList.Keys.First();
            Console.WriteLine(temp);
            Console.WriteLine($"{taskList[temp].Description}\n{taskList[temp].DueDate}");
            
        }

        static Item newItem()
        {
            Item task = new Item(); // the new task description and due date which will be returned
            string description = Console.ReadLine();
            Console.Write("would you like to add a due date for your task?: ");
            string input = Console.ReadLine(); // temp input variable to get y/n answer
            DateTime? tempDate = null;
            if (YesNo(input))
            {
                Console.Write("Enter a due date for your task in the form of dd/mm/yyyy: ");
                string dueDate = Console.ReadLine();
                bool validDate = false; // check to see if the date string provided is correct
                while (validDate == false)
                    if (DateTime.TryParseExact(dueDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        tempDate = parsedDate;
                        validDate = true;
                    }
                    else
                    {
                        Console.WriteLine("You due date was not formatted correctly, it must be in the form dd/mm/yyyy");
                        Console.Write("Enter a due date for your task in the form of dd/mm/yyyy: ");
                        dueDate = Console.ReadLine();
                    }
                task.Description = description;
                task.DueDate = tempDate;
            }
            else
            {
                task.Description = description;
                task.DueDate = null;
            }
            return task;
        }

        /* function used to parse user provided console input to parse a yes or no string input 
         * The function will convert the input to lowercase and truncate it to a single letter (so yes, no, y, n etc, answers are valid)
         * and return an appropriate true or false value. If an invalid string is provided then the user will be prompted to 
         * provide a valid response repeatedly until one is given
         * @ input: a string provided by the user
         * returns exitApp: a boolean value indicating whether the user entered yes or no
         */
        static bool YesNo(string input)
        {
            input = input.ToLower(); // convert to lowercase 
            input = input.Substring(0, 1); // remove everything except the first letter
            bool exitApp = false; // return value
            bool validInput = false; // maintains infinite input loop until user inputs valid answer
            while (validInput == false)
            {
                if (!input.Equals("y") && !input.Equals("n"))
                {
                    Console.WriteLine("Please provide a yes or no answer:");
                    input = Console.ReadLine();
                }
                else if (input.Equals("y"))
                {
                    exitApp = true;
                    validInput = true;
                } else if (input.Equals("n")) {
                    exitApp = false;
                    validInput = true;
                }
            }
            return exitApp;
        }
    } 
}

// class used to represent a to do list entry, storing the description and optional date for said task
public class Item
{
    public string Description { get; set; }
    public DateTime? DueDate { get; set; } // Nullable: can be null or a date
}

