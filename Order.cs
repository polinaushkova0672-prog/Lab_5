using System;

namespace WinFormsCargoApp.Logic
{
  public class Order
  {
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string TariffName { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double PricePerTon { get; set; }
    public double DiscountPercent { get; set; }
    public double FinalPrice { get; set; }
    public DateTime CreatedAt { get; set; }

    public Order() { }

    public Order(int id, string clientName, string tariffName, double weight, double pricePerTon, double discountPercent)
    {
      Id = id;
      ClientName = clientName;
      TariffName = tariffName;
      Weight = weight;
      PricePerTon = pricePerTon;
      DiscountPercent = discountPercent;
      CreatedAt = DateTime.Now;
      FinalPrice = CalculateFinal();
    }

    public double CalculateFinal()
    {
      double total = PricePerTon * Weight;
      total -= total * (DiscountPercent / 100.0);
      FinalPrice = Math.Round(total, 2);
      return FinalPrice;
    }
  }
}