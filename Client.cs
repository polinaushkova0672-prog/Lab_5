using System.Text.Json.Serialization;

namespace WinFormsCargoApp.Logic
{
    public enum ClientType { Обычный, Постоянный }

    public class Client
    {
        public string Name { get; set; }
        public ClientType Type { get; set; }
        public double TotalSpent { get; set; }

        public Client() { }
        public Client(string name, ClientType type = ClientType.Обычный)
        {
            Name = name;
            Type = type;
            TotalSpent = 0;
        }

        public double GetDiscountPercent()
        {
            return Type == ClientType.Постоянный ? 10 : 0; 
        }
    }
}
