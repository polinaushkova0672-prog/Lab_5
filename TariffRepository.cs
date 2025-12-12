using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WinFormsCargoApp.Logic;

namespace WinFormsCargoApp.Services
{
    public static class TariffRepository
    {
        public static List<Tariff> GetAll()
        {
            var list = new List<Tariff>();

            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT Name, Price, MinWeight, MaxWeight FROM Tariffs";
            using var cmd = new SQLiteCommand(sql, con);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new Tariff
                {
                    Name = rd.GetString(0),
                    Price = rd.GetDouble(1),
                    MinWeight = rd.GetDouble(2),
                    MaxWeight = rd.GetDouble(3)
                });
            }

            return list;
        }

        public static bool Exists(string name)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Tariffs WHERE Name=@n";
            using var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.AddWithValue("@n", name);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static void Insert(Tariff t)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql =
                "INSERT INTO Tariffs(Name, Price, MinWeight, MaxWeight) VALUES(@n, @p, @min, @max)";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@p", t.Price);
            cmd.Parameters.AddWithValue("@min", t.MinWeight);
            cmd.Parameters.AddWithValue("@max", t.MaxWeight);

            cmd.ExecuteNonQuery();
        }

        public static void Update(Tariff t)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql =
                "UPDATE Tariffs SET Price=@p, MinWeight=@min, MaxWeight=@max WHERE Name=@n";

            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@p", t.Price);
            cmd.Parameters.AddWithValue("@min", t.MinWeight);
            cmd.Parameters.AddWithValue("@max", t.MaxWeight);

            cmd.ExecuteNonQuery();
        }

        public static void Delete(string name)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "DELETE FROM Tariffs WHERE Name=@n";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", name);

            cmd.ExecuteNonQuery();
        }
    }
}
