using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WinFormsCargoApp.Logic;

namespace WinFormsCargoApp.Services
{
    public static class ClientRepository
    {
        public static List<Client> GetAll()
        {
            var list = new List<Client>();

            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT Name, Type, TotalSpent FROM Clients";
            using var cmd = new SQLiteCommand(sql, con);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new Client
                {
                    Name = rd.GetString(0),
                    Type = Enum.Parse<ClientType>(rd.GetString(1)),
                    TotalSpent = rd.GetDouble(2)
                });
            }

            return list;
        }

        public static bool Exists(string name)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Clients WHERE Name=@n";
            using var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.AddWithValue("@n", name);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static void Insert(Client c)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "INSERT INTO Clients(Name, Type, TotalSpent) VALUES(@n, @t, @s)";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", c.Name);
            cmd.Parameters.AddWithValue("@t", c.Type.ToString());
            cmd.Parameters.AddWithValue("@s", c.TotalSpent);

            cmd.ExecuteNonQuery();
        }

        public static void Update(Client c)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "UPDATE Clients SET Type=@t, TotalSpent=@s WHERE Name=@n";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", c.Name);
            cmd.Parameters.AddWithValue("@t", c.Type.ToString());
            cmd.Parameters.AddWithValue("@s", c.TotalSpent);

            cmd.ExecuteNonQuery();
        }

        public static void Delete(string name)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "DELETE FROM Clients WHERE Name=@n";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@n", name);

            cmd.ExecuteNonQuery();
        }
    }
}
