using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


enum Menu
{
    Tasks,
    Options
}

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
            
            DateTime date; // optional due date for task
            // dictionary used to store tasks (keys) and descriptions
            Dictionary<string, Item> taskList = new Dictionary<string, Item>();
            Console.WriteLine("Welcome to your to-do list");
            // check to see if there are any existing tasks, if not, prompt whether the user would like to create one
            if (taskList.Count == 0)
            {
                Console.Write("You currently have no current tasks, would you like to add a task? ");
                input = Console.ReadLine();
                // exit the application if the user answers no
                if (!YesNo(input))
                {
                    // wait 3 seconds and close the program
                    Console.Write("Goodbye");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
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
            int taskIndex = 0;
            // get all the current task names to print 
            string[] tasks = taskList.Keys.ToArray(); // an array containing the primary keys and names of all current tasks
            Console.Clear();
            drawTaskScreen(taskIndex, tasks, Menu.Tasks);
            // while the user has not chosen to exit the application (by pressing the escape key) run this loop indefinitely
            bool exitApp = false; // boolean used to keep the program running while the user has not pressed escape
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
                    Console.Clear();
                    drawTaskScreen(taskIndex, tasks, Menu.Tasks);
                }
                else if (keyPressed.Key == ConsoleKey.UpArrow && taskIndex > 0)
                {
                    taskIndex--;
                    Console.Clear();
                    drawTaskScreen(taskIndex, tasks, Menu.Tasks);
                }
                // if enter was pressed, navigate to the edit/remove task screen
                else if (keyPressed.Key == ConsoleKey.Enter)
                {
                    taskList = taskScreen(taskList, tasks[taskIndex]);                                                                                                                                                                                                                                                         
                }
                // if n was pressed, navigate to the create new task screen
                else if (keyPressed.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    bool validName = false;
                    Console.Write("Enter a name for your task: ");
                    taskName = Console.ReadLine();
                    while (!validName)
                    {
                        // check for duplicate tasks of that name
                        if (checkValidName(taskList, taskName))
                        {
                            validName = true;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("A task already exists under that name, please create a task with a new name");
                            // prompt the user to enter a new task name
                            Console.Write("Enter a name for your task: ");
                            taskName = Console.ReadLine();
                        }
                    }
                    Console.Write("Enter a description for your task: ");
                    Item newTask = newItem();
                    taskList.Add(taskName, newTask);
                    tasks = taskList.Keys.ToArray(); // update the current tasks array to contain the new task
                    drawTaskScreen(taskIndex, tasks, Menu.Tasks);
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
            Environment.Exit(1);
        }

        /*
         * Function used to display and interact with a task which has been selected form the navigation menu
         * Users can delete tasks and edit the name, description and due date on this screen 
         * @params taskList: a list of all the tasks on the to do list
         * @params index: the index of the selected task 
         * returns: a dictionary containing a list of all tasks post update
         */
        static Dictionary<string, Item> taskScreen(Dictionary<string, Item> taskList, string key)
        {
            // display the current task, task description and due date
            Console.Clear();
            Console.WriteLine($"Task name: {key}");
            Console.WriteLine($"Task Description {taskList[key].Description}");
            if (taskList[key].DueDate != null)
            {
                Console.WriteLine(taskList[key].DueDate);
            }
            // print the options for the current list
            int optionIndex = 0; // index representing the currently selected option
            // get all the current task names to print 
            string[] options = { "Edit task name", "Edit task description", "Edit task due date", "Delete task", "Go back" };
            drawTaskScreen(optionIndex, options, Menu.Options);
            
            return taskList;
        }

        /* Function to refresh the menu selection for menu selection screens
         * Clears the current console and updates the console with the list of options/tasks and new location for the > pointer
         * @param taskIndex: the index used to determine which line to draw the > pointer on 
         * @param tasks: a list of tasks/options to be selected
         */
        static void drawTaskScreen(int taskIndex, string[] tasks, Menu menuType)
        {
            if (menuType.Equals(Menu.Tasks))
            {
                Console.WriteLine("Please select a task from the following or press N to create a new task");
            }
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
         * Checks to see if a task name is valid (not a duplicate) and returns true or false accordingly
         * @param taskList: list of current tasks (keys) and their descriptions/due dates
         * @param taskName: proposed task name (primary key) for a new task
         * returns: false if a duplicate key is detected, otherwise true
         */
        static bool checkValidName(Dictionary<string, Item> taskList, string taskName)
        {
            if (taskList.ContainsKey(taskName))
            {
                return false;
            }
            return true;
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

