using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ToDoList
{
    class Program
    {
        /*
         * UI which displays options, tasks and collects user input for viewing, adding, editing and deleting tasks
         */
        static void Main(string[] args)
        {
            string input; // generic placeholder used to read in text for temporary processes
            string taskName; // used to store a task name for dictionary entry/access/deletion as the primary key
            string description; // used to store a task description for dictionary entry 
            string dueDate; // string to store due date for task
            bool exitApp = false; // a boolean used to determine whether to close the application
            DateTime date; // optional due date for task
            // dictionary used to store tasks (keys) and descriptions
            Dictionary<string, Item> taskList = new Dictionary<string, Item>();
            Console.WriteLine("Welcome to your to-do list");
            // check to see if there are any existing tasks, if not, prompt whether the user would like to create one
            if (taskList.Count == 0)
            {
                Console.Write("You currently have no current tasks, would you like to add a task? ");
                input = Console.ReadLine();
                exitApp = YesNo(input);
                // exit the application if the user answers no
                if (!exitApp)
                {
                    // wait 3 seconds and close the program
                    Console.Write("Goodbye");
                    Thread.Sleep(2000);
                    System.Environment.Exit(1);
                }
                // get task information otherwise
                Console.Write("Enter a name for your task: ");
                taskName = Console.ReadLine();
                Console.Write("Enter a description for your task: ");
                Item newTask = newItem();
                taskList.Add(taskName, newTask);
            }
            // print all the current tasks the user has stored
            // this is created with a "simulated" selection screen which uses an index based cursor and refreshes with screen clears
            Console.Clear();
            int taskIndex = 0;
            // get all the current task names to print 
            string[] tasks = taskList.Keys.ToArray(); // an array containing the primary keys and names of all current tasks
            // print the initial selection screen
            Console.WriteLine("Please select a task from the following or press N to create a new task");
            for (int i = 0; i < tasks.Length; i++)
            {
                if (i == taskIndex)
                {
                    Console.Write(">");
                }
                Console.WriteLine(tasks[i]);
            }
            // while the user has not chosen to exit the application (by pressing the escape key) run this loop indefinitely
            exitApp = false; // reset exitApp to false in the instance that on startup there were 0 tasks
            while (!exitApp)
            {
                // a flag to determine whether a valid button has been pressed 
                // poll the keyboard for user input 
                ConsoleKeyInfo keyPressed;
                keyPressed = Console.ReadKey();
                // if a valid navigation button has been pressed, update the task screen
                if (keyPressed.Key == ConsoleKey.DownArrow && taskIndex < tasks.Length - 1)
                {
                    taskIndex++;
                    drawTaskScreen(taskIndex, tasks);
                }
                else if (keyPressed.Key == ConsoleKey.UpArrow && taskIndex > 0)
                {
                    taskIndex--;
                    drawTaskScreen(taskIndex, tasks);
                }
                // if enter was pressed, navigate to the edit/remove task screen

                // if n was pressed, navigate to the create new task screen
                if (keyPressed.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    // get task information otherwise
                    Console.Write("Enter a name for your task: ");
                    taskName = Console.ReadLine();
                    Console.Write("Enter a description for your task: ");
                    Item newTask = newItem();
                    taskList.Add(taskName, newTask);
                    tasks = taskList.Keys.ToArray(); // update the current tasks array to contain the new task
                    drawTaskScreen(taskIndex, tasks);
                }

                // if esc was pressed, exit the application
                if (keyPressed.Key == ConsoleKey.Escape)
                {
                    exitApp = true;
                }
            }
            // close the application
            Console.Write("Goodbye");
            Thread.Sleep(2000);
            System.Environment.Exit(1);
        }


        /* Function to refresh the menu selection for the task selection screen
         * Clears the current console and updates the console with the list of tasks and new location for the > pointer
         * @param taskIndex: the index used to determine which line to draw the > pointer on 
         * @param tasks: a list of tasks to be completed
         */
        static void drawTaskScreen(int taskIndex, string[] tasks)
        {
            Console.Clear();
            for (int i = 0; i < tasks.Length; i++)
            {
                if (i == taskIndex)
                {
                    Console.Write(">");
                }
                Console.WriteLine(tasks[i]);
            }
        }

        /* 
         * function used to return a new Item class which stores a tasks description and due date (if given)
         * The user is prompted for a task descrpition and an (optional) due date which is parsed to determine if it is a valid
         * date format. A new Item is created using this information and then returned to the main function
         * @returns newTask, an Item class containing a description and valid dd/mm/yyyy datetime
         */
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

        /* 
         * function used to parse user provided console input to parse a yes or no string input 
         * The function will convert the input to lowercase and truncate it to a single letter (so yes, no, y, n etc, answers are valid)
         * and return an appropriate true or false value. If an invalid string is provided then the user will be prompted to 
         * provide a valid response repeatedly until one is given
         * @ param input: a string provided by the user
         * @ returns exitApp, a boolean value indicating whether the user entered yes or no
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
                    Console.Write("Please provide a yes or no answer: ");
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

// class used to represent a to do list entry, storing the description and optional due date for said task
public class Item
{
    public string Description { get; set; }
    public DateTime? DueDate { get; set; } // Nullable: can be null or a date
}

