using System;
using System.Data.SQLite;
using System.IO;

namespace WinFormsCargoApp.Services
{
    public static class Database
    {
        private static readonly string DbFile = "cargo.db";
        private static readonly string ConnectionString = $"Data Source={DbFile};Version=3;";

        // ======================================================
        // Получение подключения
        // ======================================================
        public static SQLiteConnection GetConnection() =>
            new SQLiteConnection(ConnectionString);

        // ======================================================
        // Инициализация БД
        // ======================================================
        public static void Initialize()
        {
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            using var con = GetConnection();
            con.Open();

            CreateTables(con);
        }

        // ======================================================
        // Создание всех таблиц (если нет)
        // ======================================================
        private static void CreateTables(SQLiteConnection con)
        {
            string sqlClients =
@"
CREATE TABLE IF NOT EXISTS Clients(
    Name TEXT PRIMARY KEY,
    Type TEXT NOT NULL,
    TotalSpent REAL NOT NULL
);
";

            string sqlTariffs =
@"
CREATE TABLE IF NOT EXISTS Tariffs(
    Name TEXT PRIMARY KEY,
    Price REAL NOT NULL,
    MinWeight REAL NOT NULL,
    MaxWeight REAL NOT NULL
);
";

            string sqlOrders =
@"
CREATE TABLE IF NOT EXISTS Orders(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClientName TEXT NOT NULL,
    TariffName TEXT NOT NULL,
    Weight REAL NOT NULL,
    PricePerTon REAL NOT NULL,
    DiscountPercent REAL NOT NULL,
    FinalPrice REAL NOT NULL,
    CreatedAt TEXT NOT NULL,

    FOREIGN KEY(ClientName) REFERENCES Clients(Name),
    FOREIGN KEY(TariffName) REFERENCES Tariffs(Name)
);
";

            using var cmd1 = new SQLiteCommand(sqlClients, con);
            using var cmd2 = new SQLiteCommand(sqlTariffs, con);
            using var cmd3 = new SQLiteCommand(sqlOrders, con);

            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            cmd3.ExecuteNonQuery();
        }

        // ======================================================
        // Очистка всех таблиц (используется при Import JSON)
        // ======================================================
        public static void ClearAllTables()
        {
            using var con = GetConnection();
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "DELETE FROM Orders";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Clients";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Tariffs";
            cmd.ExecuteNonQuery();
        }
    }
}
