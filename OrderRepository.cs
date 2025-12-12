using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WinFormsCargoApp.Logic;

namespace WinFormsCargoApp.Services
{
    public static class OrderRepository
    {
        public static List<Order> GetAll(string orderBy = "Id ASC")
        {
            var list = new List<Order>();

            using var con = Database.GetConnection();
            con.Open();

            string sql =
                $"SELECT Id, ClientName, TariffName, Weight, PricePerTon, DiscountPercent, FinalPrice, CreatedAt FROM Orders ORDER BY {orderBy}";

            using var cmd = new SQLiteCommand(sql, con);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new Order
                {
                    Id = rd.GetInt32(0),
                    ClientName = rd.GetString(1),
                    TariffName = rd.GetString(2),
                    Weight = rd.GetDouble(3),
                    PricePerTon = rd.GetDouble(4),
                    DiscountPercent = rd.GetDouble(5),
                    FinalPrice = rd.GetDouble(6),
                    CreatedAt = rd.GetDateTime(7)
                });
            }

            return list;
        }

        public static void Insert(Order o)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql =
@"
INSERT INTO Orders(ClientName, TariffName, Weight, PricePerTon, DiscountPercent, FinalPrice, CreatedAt)
VALUES(@c, @t, @w, @p, @d, @f, @dt);
";

            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@c", o.ClientName);
            cmd.Parameters.AddWithValue("@t", o.TariffName);
            cmd.Parameters.AddWithValue("@w", o.Weight);
            cmd.Parameters.AddWithValue("@p", o.PricePerTon);
            cmd.Parameters.AddWithValue("@d", o.DiscountPercent);
            cmd.Parameters.AddWithValue("@f", o.FinalPrice);
            cmd.Parameters.AddWithValue("@dt", o.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public static void Update(Order o)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql =
@"
UPDATE Orders SET 
    ClientName=@c,
    TariffName=@t,
    Weight=@w,
    PricePerTon=@p,
    DiscountPercent=@d,
    FinalPrice=@f
WHERE Id=@id;
";

            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", o.Id);
            cmd.Parameters.AddWithValue("@c", o.ClientName);
            cmd.Parameters.AddWithValue("@t", o.TariffName);
            cmd.Parameters.AddWithValue("@w", o.Weight);
            cmd.Parameters.AddWithValue("@p", o.PricePerTon);
            cmd.Parameters.AddWithValue("@d", o.DiscountPercent);
            cmd.Parameters.AddWithValue("@f", o.FinalPrice);

            cmd.ExecuteNonQuery();
        }

        public static void Delete(int id)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "DELETE FROM Orders WHERE Id=@id";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public static bool ExistsForClient(string clientName)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Orders WHERE ClientName=@c";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@c", clientName);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static bool ExistsForTariff(string tariffName)
        {
            using var con = Database.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Orders WHERE TariffName=@t";
            using var cmd = new SQLiteCommand(sql, con);

            cmd.Parameters.AddWithValue("@t", tariffName);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
