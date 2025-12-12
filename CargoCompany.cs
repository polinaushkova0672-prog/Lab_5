using System.Collections.Generic;
using System.Linq;

namespace WinFormsCargoApp.Logic
{
  public class CargoCompany
  {
    public List<Client> Clients { get; set; } = new List<Client>();
    public List<Tariff> Tariffs { get; set; } = new List<Tariff>();
    public List<Order> Orders { get; set; } = new List<Order>();

    private int nextOrderId = 1;

    public CargoCompany() { }

    public void InitializeDefaultTariffs()
    {
      Tariffs.Clear();
      Tariffs.Add(new Tariff("Эконом", 1500, 0, 10));
      Tariffs.Add(new Tariff("Стандарт", 1200, 10, 50));
      Tariffs.Add(new Tariff("Бизнес", 1000, 50, 200));
      Tariffs.Add(new Tariff("Премиум", 800, 200, double.MaxValue));
    }

    public Tariff SelectTariffByWeight(double weight) => Tariffs.FirstOrDefault(t => t.CanApply(weight));

    public void AddClient(Client c)
    {
      if (Clients.Any(x => x.Name == c.Name)) throw new System.Exception("Клиент с таким именем уже существует");
      Clients.Add(c);
    }

    public void AddTariff(Tariff t) => Tariffs.Add(t);

    public Order CreateOrder(string clientName, string tariffName, double weight)
    {
      var client = Clients.FirstOrDefault(c => c.Name == clientName);
      var tariff = Tariffs.FirstOrDefault(t => t.Name == tariffName);
      if (client == null || tariff == null) return null;

      var order = new Order(nextOrderId++, clientName, tariffName, weight, tariff.Price, client.GetDiscountPercent());
      Orders.Add(order);
      client.TotalSpent += order.FinalPrice;
      return order;
    }

    public void UpdateOrder(Order updated)
    {
      var existing = Orders.FirstOrDefault(o => o.Id == updated.Id);
      if (existing == null) return;
      existing.ClientName = updated.ClientName;
      existing.TariffName = updated.TariffName;
      existing.Weight = updated.Weight;
      existing.PricePerTon = updated.PricePerTon;
      existing.DiscountPercent = updated.DiscountPercent;
      existing.CalculateFinal();
    }

    public void DeleteOrder(int id) => Orders.RemoveAll(o => o.Id == id);

    // After loading from JSON, ensure nextOrderId is correct
    public void FixNextId()
    {
      if (Orders.Count == 0) nextOrderId = 1;
      else nextOrderId = Orders.Max(o => o.Id) + 1;
    }
  }
}