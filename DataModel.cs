using Microsoft.VisualBasic.Logging;
using System.Collections.Generic;

namespace WinFormsCargoApp.Models
{
    public class DataModel
    {
        public List<Logic.Client> Clients { get; set; } = new List<Logic.Client>();
        public List<Logic.Tariff> Tariffs { get; set; } = new List<Logic.Tariff>();
        public List<Logic.Order> Orders { get; set; } = new List<Logic.Order>();
    }
}
