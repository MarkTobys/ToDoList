using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

// Enum used to denote which menu type to draw when DrawTaskScreen is called
enum Menu
{
    Tasks,
    Options
}

namespace ToDoList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear(); // clear the screen after the app is launched
            string input; // generic placeholder used to read in text for temporary processes
            string taskName; // used to store a task name for dictionary entry/access/deletion as the primary 
            // dictionary used to store tasks (keys) and descriptions
            Dictionary<string, Item> taskList = ReadSavedData();
            Console.WriteLine("Welcome to your to-do list");
            // check to see if there are any existing tasks, if not, prompt whether the user would like to create one
            if (taskList.Count == 0)
            {
                Console.Write("You currently have no current tasks, would you like to add a task? ");
                input = Console.ReadLine();
                // exit the application if the user answers no
                if (!YesNo(input))
                {
                    // wait 2 seconds and close the program
                    Console.Write("Goodbye");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }
                // get task information otherwise
                Console.Write("Enter a name for your task: ");
                taskName = Console.ReadLine();
                Item newTask = NewItem();
                taskList.Add(taskName, newTask);
            }
            // print all the current tasks the user has stored
            int taskIndex = 0; // index to store > pointer location
            string[] tasks = taskList.Keys.ToArray(); // an array containing the primary keys and names of all current tasks
            DrawTaskScreen(taskIndex, tasks, Menu.Tasks);
            // while the user has not chosen to exit the application (by pressing the escape key) run this loop indefinitely
            bool exitApp = false; // flag used to keep the program running while the user has not pressed escape
            while (!exitApp)
            { 
                ConsoleKeyInfo keyPressed;
                keyPressed = Console.ReadKey(intercept:true);
                // if a valid navigation button has been pressed, update the task screen
                if (keyPressed.Key == ConsoleKey.DownArrow && taskIndex < tasks.Length - 1)
                {
                    taskIndex++;
                    DrawTaskScreen(taskIndex, tasks, Menu.Tasks);
                }
                else if (keyPressed.Key == ConsoleKey.UpArrow && taskIndex > 0)
                {
                    taskIndex--;
                    DrawTaskScreen(taskIndex, tasks, Menu.Tasks);
                }
                // if enter was pressed, navigate to the edit/remove task screen
                else if (keyPressed.Key == ConsoleKey.Enter)
                {
                    taskList = taskScreen(taskList, tasks[taskIndex]);
                    tasks = taskList.Keys.ToArray(); // update the tasks (keys) 
                    SaveData(taskList); // update the save data 
                    DrawTaskScreen(taskIndex, tasks, Menu.Tasks);
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
                        if (CheckValidName(taskList, taskName))
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
                    Item newTask = NewItem();
                    taskList.Add(taskName, newTask);
                    tasks = taskList.Keys.ToArray(); // update the current tasks array to contain the new task
                    SaveData(taskList); // update the save data 
                    DrawTaskScreen(taskIndex, tasks, Menu.Tasks);
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
            // print the options for the current list
            int optionIndex = 0; // index representing the currently selected option
            DrawTask(taskList, key);
            // get all the current task names to print 
            string[] options = { "Edit task name", "Edit task description", "Edit task due date", "Delete task", "Go back" };
            DrawTaskScreen(optionIndex, options, Menu.Options);
            // loop to handle all user interaction events
            bool goBack = false; // keeps the user in the edit task screen until they select go back or hit escape
            while (!goBack)
            {
                // poll the keyboard for user input 
                ConsoleKeyInfo keyPressed;
                keyPressed = Console.ReadKey();
                // if a valid navigation button has been pressed, update the task screen
                if (keyPressed.Key == ConsoleKey.DownArrow && optionIndex < 4)
                {
                    optionIndex++;
                    DrawTask(taskList, key);
                    DrawTaskScreen(optionIndex, options, Menu.Options);
                }
                else if (keyPressed.Key == ConsoleKey.UpArrow && optionIndex > 0)
                {
                    optionIndex--;
                    DrawTask(taskList, key);
                    DrawTaskScreen(optionIndex, options, Menu.Options);
                } 
                else if (keyPressed.Key == ConsoleKey.Escape)
                {
                    goBack = true; // return to the task list menu if escape is pressed
                } 
                else if (keyPressed.Key == ConsoleKey.Enter)
                {
                    // if enter is pressed, check the index to determine which menu option was selected and act accordingly
                    switch (optionIndex)
                    {
                        case 0:     // edit task name
                            Item newTask = new Item();
                            (string, Item) newEntry = NewName(taskList, key); // prompt user for a new task                          
                            taskList.Remove(key); // remove the old key from the dictionary   
                            key = newEntry.Item1;
                            taskList.Add(key, newEntry.Item2);
                            DrawTask(taskList, key); // refresh the screen to draw the new task
                            DrawTaskScreen(optionIndex, options, Menu.Options);
                            break;
                        case 1:     // update task description
                            Console.Clear();
                            Console.WriteLine($"Current description: {taskList[key].Description}");
                            Console.Write("Updated description: ");
                            string newDesc = Console.ReadLine();
                            taskList[key].Description = newDesc;
                            DrawTask(taskList, key);
                            DrawTaskScreen(optionIndex, options, Menu.Options);
                            break;
                        case 2:     // edit task date
                            Console.Clear();
                            if (taskList[key].DueDate != null)
                            {
                                Console.WriteLine($"Current date: {taskList[key].DueDate}");
                            } else
                            {
                                Console.WriteLine("Task has no current due date");
                            }
                            Console.Write("New due date: ");
                            string newDate = Console.ReadLine();
                            DateTime? tempDate = null;
                            bool validDate = false; // check to see if the date string provided is correct
                            while (validDate == false)
                                if (DateTime.TryParseExact(newDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                                {
                                    if (parsedDate < DateTime.Now)
                                    {
                                        Console.Write("The due date for a task cannot be before the current date, please enter a valid date: ");
                                        newDate = Console.ReadLine();
                                    }
                                    else
                                    {
                                        tempDate = parsedDate;
                                        validDate = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("You due date was not formatted correctly, it must be in the form dd/mm/yyyy");
                                    Console.Write("Enter a due date for your task in the form of dd/mm/yyyy: ");
                                    newDate = Console.ReadLine();
                                }
                            taskList[key].DueDate = tempDate;
                            DrawTask(taskList, key); 
                            DrawTaskScreen(optionIndex, options, Menu.Options);
                            break;
                        case 3: // delete item
                            Console.Clear();
                            Console.WriteLine("are you absolutely sure you want to delete this to do list entry?");
                            Console.Write("Once you delete it, it can no longer be retrieved: ");
                            string answer = Console.ReadLine();
                            if (YesNo(answer))
                            {
                                taskList.Remove(key);
                                goBack = true;
                            }
                            else
                            {
                                DrawTask(taskList, key);
                                DrawTaskScreen(optionIndex, options, Menu.Options);
                            }
                            break;
                        case 4: // last option
                            goBack = true; 
                            break;
                    }         
                        
                }
            }
            return taskList;
        }
        
        /*
         * Takes a new name provided to the user in EditName and creates a duplicate Item to return to taskScreen function
         * @params taskList: list of existing tasks on the to do list
         * @params key: current taskname and primary key 
         */
        static (string, Item) NewName(Dictionary<string, Item> taskList, string key)
        {
            // promt user for a new task name
            string newName = EditName(taskList, key);
            // temporarly store a copy of the desc and date (if it exists) and delete the old key pair from the dict
            string tempDesc = taskList[key].Description;
            DateTime? tempDate = null;
            if (taskList[key].DueDate != null)
            {
                tempDate = taskList[key].DueDate;
            }
            // create a new task and item pair and add them to the task list
            Item newTask = new Item();
            newTask.Description = tempDesc;
            newTask.DueDate = tempDate;
            return (newName, newTask);
        }

        /*
        *   Function which searches for ToDoList.txt CSV file and reads the contents to create the task dictionary for the program, returning
        *   an empty library if the file cannot be found or is empty otherwise
        *   @returns: a Dictionary with a string type as the primary key and Item as contents with the information from ToDoList.txt, or empty if there is no file/file is empty
        */
        static Dictionary<string, Item> ReadSavedData() 
        {
            string fileName = "ToDoList.txt";
            Dictionary<string, Item> taskList = new Dictionary<string, Item>(); 
            // search for the ToDoList.txt file and create one if it does not exist
            if (File.Exists("ToDoList.txt")) {
                // read in each line of the to do list, parsing the CSV's and adding them to the dictionary
                // Keep in mind there is no error checking in place for file reading so any manual changes made to ToDoList.txt may cause crashes
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        Item tempItem = new Item();
                        tempItem.Description = values[1]; // 2nd value is the description 
                        if (values[2] == "null") {
                            tempItem.DueDate = null;
                        } else {
                            DateTime.TryParseExact(values[2], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate); // 3rd value is datetime
                            tempItem.DueDate = parsedDate;
                        }
                        taskList.Add(values[0], tempItem);
                    }
                }
            } 
            else 
            {
                File.Create(fileName).Dispose(); // create the file if it does not exist
            }
            return taskList;
        }
        
        /*
        *   Function which saves the current state of the taskList to ToDoList.txt in a CSV format, formatted as task name, task description, task date
        */
        static void SaveData(Dictionary<string, Item> taskList) 
        {
            // overwrite the existing ToDoList.txt
            using (StreamWriter writer = new StreamWriter("ToDoList.txt", append: false))
                {
                    // iterate through each task in taskList
                    foreach (var task in taskList)
                    {
                        string currentTask = task.Key;
                        string description = task.Value.Description;
                        // check to see if the due date is null or a date, if it is a date convert it to a string, otherwise store the due date as "null"
                        string due = "null"; // by default, set due to null in the instance that no due date is provided
                        if (taskList[currentTask].DueDate != null) {
                            due = task.Value.DueDate.ToString();
                        }
                        writer.WriteLine($"{currentTask}, {description}, {due}");
                    }
                }
        }

        /* 
         * Function which allows the user to edit a tasks name (key) which is then returned and used to update the task list
         * @param name: string representing the current tasks name and primary key in the dictionary
         * @param taskList: dictionary of tasks used to determine if the new task name is valid to avoid duplicate keys
         * returns: updated string and primary key
         */
        static string EditName(Dictionary<string, Item> taskList, string name)
        {
            // prompt the user to edit their task name
            Console.Clear();
            Console.WriteLine($"Current name: {name}");
            Console.Write("New name: ");
            string newName = Console.ReadLine();
            // check to see if the updated name is a duplicate key
            bool validName = false;
            while (!validName)
            {
                if (CheckValidName(taskList, newName))
                {
                    validName = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("A task already exists under that name, please create a task with a new name");
                    // prompt the user to enter a new task name
                    Console.Write("Edit the name of your task: ");
                    Console.Write(name);
                    newName = Console.ReadLine();
                }
            }
            return newName;
        }

        /*
         * Function which draws the current task name, description and due date (if one exists)
         * @param taskList: dictionary of tasks on the to do list
         * @param key: string containing the name of the current information task being printed 
         */
        static void DrawTask(Dictionary<string, Item> taskList, string key)
        {
            // display the current task, task description and due date
            Console.Clear();
            Console.WriteLine($"Task name: {key}");
            Console.WriteLine($"Task Description: {taskList[key].Description}");
            if (taskList[key].DueDate != null)
            {
                Console.WriteLine($"due date: {taskList[key].DueDate}");
            }
            Console.WriteLine();
            return;
        }
        
        /* Function to refresh the menu selection for menu selection screens
         * Clears the current console and updates the console with the list of options/tasks and new location for the > pointer
         * @param taskIndex: the index used to determine which line to draw the > pointer on 
         * @param tasks: a list of tasks/options to be selected
         */
        static void DrawTaskScreen(int taskIndex, string[] tasks, Menu menuType)
        {
            if (tasks.Length == 0)
            {
                Console.Clear();
                Console.WriteLine("You currently have no tasks to complete. Press N to create a new task");
            } 
            else 
            {
                if (menuType.Equals(Menu.Tasks))
                {
                    Console.Clear();
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
        }

        /* 
         * function used to return a new Item class which stores a tasks description and due date (if given)
         * The user is prompted for a task descrpition and an (optional) due date which is parsed to determine if it is a valid
         * date format. A new Item is created using this information and then returned to the main function
         * @returns newTask, an Item class containing a description and valid dd/mm/yyyy datetime
         */
        static Item NewItem()
        {
            Console.Write("Enter a description for your task: ");
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
                        if (parsedDate < DateTime.Now)
                        {
                            Console.Write("The due date for a task cannot be before the current date, please enter a valid date: ");
                            dueDate = Console.ReadLine();
                        }
                        else
                        {
                            tempDate = parsedDate;
                            validDate = true;
                        }
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
        static bool CheckValidName(Dictionary<string, Item> taskList, string taskName)
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

