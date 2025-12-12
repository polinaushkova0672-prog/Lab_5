using System;
using System.Linq;
using System.Windows.Forms;
using WinFormsCargoApp.Logic;
using WinFormsCargoApp.Services;

namespace WinFormsCargoApp
{
  public class OrderForm : Form
  {
    private Order? editingOrder;

    private ComboBox cbClient, cbTariff;
    private TextBox tbWeight;
    private Label lblPrice, lblFinal;
    private Button btnSave, btnCancel;

    private Client[] clients;
    private Tariff[] tariffs;

    public OrderForm()
    {
      LoadDataFromDb();
      InitializeComponents();
      Text = "Добавить заказ";
    }

    public OrderForm(Order order) : this()
    {
      editingOrder = order;
      Text = "Редактировать заказ";
      LoadOrderToForm(order);
    }

    private void LoadDataFromDb()
    {
      clients = ClientRepository.GetAll().ToArray();
      tariffs = TariffRepository.GetAll().ToArray();
    }

    private void InitializeComponents()
    {
      Width = 420;
      Height = 260;
      StartPosition = FormStartPosition.CenterParent;

      var lbl1 = new Label { Text = "Клиент:", Left = 10, Top = 20, Width = 80 };
      cbClient = new ComboBox { Left = 100, Top = 20, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

      var lbl2 = new Label { Text = "Тариф:", Left = 10, Top = 60, Width = 80 };
      cbTariff = new ComboBox { Left = 100, Top = 60, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
      cbTariff.SelectedValueChanged += (_, __) => UpdatePriceLabels();

      var lbl3 = new Label { Text = "Вес (тонн):", Left = 10, Top = 100, Width = 80 };
      tbWeight = new TextBox { Left = 100, Top = 100, Width = 120 };
      tbWeight.MaxLength = 7;
      tbWeight.TextChanged += (_, __) => UpdatePriceLabels();

      lblPrice = new Label { Text = "Цена/тонну: -", Left = 10, Top = 140, Width = 200 };
      lblFinal = new Label { Text = "Итог: -", Left = 220, Top = 140, Width = 200 };

      btnSave = new Button { Text = "Сохранить", Left = 100, Top = 180, Width = 100 };
      btnCancel = new Button { Text = "Отмена", Left = 220, Top = 180, Width = 100 };

      btnSave.Click += BtnSave_Click;
      btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

      Controls.AddRange(new Control[] {
                lbl1, cbClient, lbl2, cbTariff, lbl3, tbWeight,
                lblPrice, lblFinal, btnSave, btnCancel
            });

      FillLists();
    }

    private void FillLists()
    {
      cbClient.Items.Clear();
      foreach (var c in clients)
        cbClient.Items.Add(c.Name);

      if (cbClient.Items.Count > 0)
        cbClient.SelectedIndex = 0;

      cbTariff.Items.Clear();
      foreach (var t in tariffs)
        cbTariff.Items.Add(t.Name);

      if (cbTariff.Items.Count > 0)
        cbTariff.SelectedIndex = 0;

      UpdatePriceLabels();
    }

    private void LoadOrderToForm(Order order)
    {
      cbClient.SelectedItem = order.ClientName;
      cbTariff.SelectedItem = order.TariffName;
      tbWeight.Text = order.Weight.ToString();

      UpdatePriceLabels();
    }

    private void UpdatePriceLabels()
    {
      if (cbTariff.SelectedItem == null)
        return;

      string tariffName = cbTariff.SelectedItem.ToString()!;
      var tariff = tariffs.FirstOrDefault(t => t.Name == tariffName);
      if (tariff == null)
      {
        lblPrice.Text = "Цена/тонну: -";
        lblFinal.Text = "Итог: -";
        return;
      }

      lblPrice.Text = $"Цена/тонну: {tariff.Price}";

      if (!double.TryParse(tbWeight.Text, out double w) || w <= 0)
      {
        lblFinal.Text = "Итог: -";
        return;
      }

      var client = clients.FirstOrDefault(c => c.Name == cbClient.SelectedItem?.ToString());
      double discount = client?.GetDiscountPercent() ?? 0;

      double final = Math.Round(tariff.Price * w * (1 - discount / 100.0), 2);
      lblFinal.Text = $"Итог: {final}";
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
      if (cbClient.SelectedItem == null) { MessageBox.Show("Выберите клиента"); return; }
      if (cbTariff.SelectedItem == null) { MessageBox.Show("Выберите тариф"); return; }
      if (!double.TryParse(tbWeight.Text, out double weight) || weight <= 0)
      {
        MessageBox.Show("Неверный вес");
        return;
      }

      string clientName = cbClient.SelectedItem.ToString()!;
      string tariffName = cbTariff.SelectedItem.ToString()!;
      var tariff = tariffs.First(t => t.Name == tariffName);
      var client = clients.First(c => c.Name == clientName);

      double pricePerTon = tariff.Price;
      double discount = client.GetDiscountPercent();
      double finalPrice = Math.Round(pricePerTon * weight * (1 - discount / 100.0), 2);

      if (editingOrder == null)
      {
        // -------------------------
        // СОЗДАНИЕ ЗАКАЗА (INSERT)
        // -------------------------
        var newOrder = new Order
        {
          ClientName = clientName,
          TariffName = tariffName,
          Weight = weight,
          PricePerTon = pricePerTon,
          DiscountPercent = discount,
          FinalPrice = finalPrice,
          CreatedAt = DateTime.Now
        };

        OrderRepository.Insert(newOrder);

        // Обновляем траты клиента
        client.TotalSpent += finalPrice;
        ClientRepository.Update(client);
      }
      else
      {
        // -------------------------
        // РЕДАКТИРОВАНИЕ ЗАКАЗА
        // -------------------------
        double oldFinal = editingOrder.FinalPrice;

        editingOrder.ClientName = clientName;
        editingOrder.TariffName = tariffName;
        editingOrder.Weight = weight;
        editingOrder.PricePerTon = pricePerTon;
        editingOrder.DiscountPercent = discount;
        editingOrder.FinalPrice = finalPrice;

        OrderRepository.Update(editingOrder);

        // обновляем траты клиента:
        double diff = finalPrice - oldFinal;
        client.TotalSpent += diff;
        ClientRepository.Update(client);
      }

      DialogResult = DialogResult.OK;
    }
  }
}
