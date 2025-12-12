namespace WinFormsCargoApp.Logic
{
    public class Tariff
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }

        public Tariff() { }
        public Tariff(string name, double price, double minW, double maxW)
        {
            Name = name; Price = price; MinWeight = minW; MaxWeight = maxW;
        }

        public bool CanApply(double weight) => weight >= MinWeight && weight < MaxWeight;
    }
}
