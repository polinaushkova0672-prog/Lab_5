using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WinFormsCargoApp.Models;
using WinFormsCargoApp.Logic;

namespace WinFormsCargoApp.Services
{
    public static class FileService
    {
        private static readonly JsonSerializerOptions opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        // ===========================
        //     EXPORT DB → JSON
        // ===========================
        public static void ExportDbToJson(string path)
        {
            var model = new DataModel
            {
                Clients = ClientRepository.GetAll(),
                Tariffs = TariffRepository.GetAll(),
                Orders = OrderRepository.GetAll()
            };

            string json = JsonSerializer.Serialize(model, opts);
            File.WriteAllText(path, json);
        }

        // ===========================
        //     IMPORT JSON → DB
        // ===========================
        public static void ImportJsonToDb(string path)
        {
            var json = File.ReadAllText(path);
            var model = JsonSerializer.Deserialize<DataModel>(json, opts);
            if (model == null) return;

            Database.ClearAllTables();

            // Вставляем клиентов
            foreach (var c in model.Clients)
                ClientRepository.Insert(c);

            // Вставляем тарифы
            foreach (var t in model.Tariffs)
                TariffRepository.Insert(t);

            // Вставляем заказы
            foreach (var o in model.Orders)
                OrderRepository.Insert(o);
        }
    }
}
