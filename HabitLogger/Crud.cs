using System.Data;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;

namespace HabitLogger;

public class Crud
{
    private static string _connectionString = @"Data Source=habitTracker.db";

    static void CreateDataBase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS drinking_water (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                )";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    public static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;
        while (closeApp == false)
        {
            Console.WriteLine("\n\nMain Menu");
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("\nType 0 o Close Applications");
            Console.WriteLine("Type 1 to View All Records.");
            Console.WriteLine("Type 2 to Insert Record");
            Console.WriteLine("Type 3 to Deleted Record.");
            Console.WriteLine("Type 4 to Update Record.");
            Console.WriteLine("---------------------------------------------------");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("\nGoodbye!\n");
                    closeApp = true;
                    break;

                case "1":
                    GetAllRecords();
                    break;

                case "2":
                    Insert();
                    break;

                case "3":
                    Delete();
                    break;
                
                // case "4":
                //     Update();
                //     break;
                //
                // default:
                //     Console.WriteLine("\nInvalid Command. Please try a number from 0 to 4");
                //     break;
            }
        }
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"SELECT * FROM drinking_water";

            List<DrinkingWater> tableData = new();

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                }
            }
            else
            {
                Console.WriteLine("No rows found");
            }

            connection.Close();

            Console.WriteLine("--------------------------------------------------\n");
            foreach (var dw in tableData)
            {
                Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy")} - Quantity: {dw.Quantity}");
            }

            Console.WriteLine("--------------------------------------------------");
        }
    }

    private static void Insert()
    {
        string date = GetDateInput();

        int quantity =
            GetNumberInput(
                "\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    internal static string GetDateInput()
    {
        Console.WriteLine("\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu");

        string dateInput = Console.ReadLine();

        if (dateInput == "0")
            GetUserInput();

        return dateInput;
    }

    internal static int GetNumberInput(string message)
    {
        Console.WriteLine(message);
        string numberInput = Console.ReadLine();

        if (numberInput == "0")
            GetUserInput();

        int finalInput = Convert.ToInt32(numberInput);

        return finalInput;
    }

    private static void Delete()
    {
        Console.Clear();
        GetAllRecords();

        var recordId =
            GetNumberInput("\n\nPlease type the Id of the record you want to delete. Type 0 to return to main menu");

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = '{recordId}'";

            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                Delete();
            }
        }
        
        Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");
        
        GetUserInput();
    }
}