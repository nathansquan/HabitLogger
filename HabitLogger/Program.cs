using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habitTracker.db";

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    var command= connection.CreateCommand();

    command.CommandText = 
        @"CREATE TABLE IF NOT EXISTS drinking_water (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Quantity INTEGER)";
    
    command.ExecuteNonQuery();
    
    connection.Close();
}