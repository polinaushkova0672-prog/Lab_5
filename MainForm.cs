using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WinFormsCargoApp.Logic;
using WinFormsCargoApp.Services;

namespace WinFormsCargoApp
{
  public class MainForm : Form
  {
    private TabControl tab;

    // ====== ЗАКАЗЫ ======
    private DataGridView dgvOrders;
    private BindingList<Order> ordersBinding;
    private string sortOrdersColumn = "Id";
    private bool sortOrdersAsc = true;

    // ====== КЛИЕНТЫ ======
    private DataGridView dgvClients;
    private BindingList<Client> clientsBinding;
    private string sortClientsColumn = "Name";
    private bool sortClientsAsc = true;

    // ====== ТАРИФЫ ======
    private DataGridView dgvTariffs;
    private BindingList<Tariff> tariffsBinding;
    private string sortTariffsColumn = "Name";
    private bool sortTariffsAsc = true;


    public MainForm()
    {
      Text = "Cargo Company — SQLite + Tabs";
      Width = 1100;
      Height = 700;
      StartPosition = FormStartPosition.CenterScreen;

      Database.Initialize();
      EnsureInitialData();

      InitializeUI();
      LoadAllTabs();
    }
    private void Tab_SelectedIndexChanged(object? sender, EventArgs e)
    {
      int index = tab.SelectedIndex;

      switch (index)
      {
        case 0: LoadOrders(); break;
        case 1: LoadClients(); break;
        case 2: LoadTariffs(); break;
      }
    }

    // ===============================
    // Первичная загрузка данных в БД
    // ===============================
    private void EnsureInitialData()
    {
      if (ClientRepository.GetAll().Count == 0)
      {
        ClientRepository.Insert(new Client("Иван", ClientType.Постоянный));
        ClientRepository.Insert(new Client("Мария", ClientType.Постоянный));
        ClientRepository.Insert(new Client("Петр", ClientType.Обычный));
        ClientRepository.Insert(new Client("Анна", ClientType.Обычный));
      }

      if (TariffRepository.GetAll().Count == 0)
      {
        TariffRepository.Insert(new Tariff("Эконом", 1500, 0, 10));
        TariffRepository.Insert(new Tariff("Стандарт", 1200, 10, 50));
        TariffRepository.Insert(new Tariff("Бизнес", 1000, 50, 200));
        TariffRepository.Insert(new Tariff("Премиум", 800, 200, double.MaxValue));
      }
    }

    // ===============================
    //  ИНИЦИАЛИЗАЦИЯ UI
    // ===============================
    private void InitializeUI()
    {
      // === ПАНЕЛЬ JSON ===
      var topPanel = new Panel
      {
        Dock = DockStyle.Top,
        Height = 50
      };

      var btnLoadJson = new Button
      {
        Text = "Загрузить JSON",
        Left = 10,
        Top = 10,
        Width = 150
      };
      btnLoadJson.Click += BtnLoadJson_Click;

      var btnSaveJson = new Button
      {
        Text = "Сохранить JSON",
        Left = 170,
        Top = 10,
        Width = 150
      };
      btnSaveJson.Click += BtnSaveJson_Click;

      topPanel.Controls.Add(btnLoadJson);
      topPanel.Controls.Add(btnSaveJson);


      // === TABCONTROL ===
      tab = new TabControl
      {
        Dock = DockStyle.Fill
      };

      tab.TabPages.Add(CreateOrdersTab());
      tab.TabPages.Add(CreateClientsTab());
      tab.TabPages.Add(CreateTariffsTab());

      tab.SelectedIndexChanged += Tab_SelectedIndexChanged;

      Controls.Add(tab);
      Controls.Add(topPanel);
    }

    // #################### TAB: ORDERS ####################

    private TabPage CreateOrdersTab()
    {
      var page = new TabPage("Заказы");

      dgvOrders = new DataGridView
      {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = false,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
      };

      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 80 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Клиент", DataPropertyName = "ClientName", Width = 140 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тариф", DataPropertyName = "TariffName", Width = 140 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Вес", DataPropertyName = "Weight", Width = 80 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена/т", DataPropertyName = "PricePerTon", Width = 80 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Скидка %", DataPropertyName = "DiscountPercent", Width = 80 });
      dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Итог", DataPropertyName = "FinalPrice", Width = 120 });

      dgvOrders.ColumnHeaderMouseClick += DgvOrders_ColumnHeaderMouseClick;

      var panel = CreateButtonsPanel(
                onAdd: BtnAddOrder,
                onEdit: BtnEditOrder,
                onDelete: BtnDeleteOrder
            );

      page.Controls.Add(dgvOrders);
      page.Controls.Add(panel);

      return page;
    }

    // #################### TAB: CLIENTS ####################

    private TabPage CreateClientsTab()
    {
      var page = new TabPage("Клиенты");

      dgvClients = new DataGridView
      {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = false,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
      };

      dgvClients.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Имя", DataPropertyName = "Name", Width = 200 });
      dgvClients.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 130 });
      dgvClients.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Потрачено", DataPropertyName = "TotalSpent", Width = 120 });

      dgvClients.ColumnHeaderMouseClick += DgvClients_ColumnHeaderMouseClick;

      var panel = CreateButtonsPanel(
                onAdd: BtnAddClient,
                onEdit: BtnEditClient,
                onDelete: BtnDeleteClient
            );

      page.Controls.Add(dgvClients);
      page.Controls.Add(panel);

      return page;
    }

    // #################### TAB: TARIFFS ####################

    private TabPage CreateTariffsTab()
    {
      var page = new TabPage("Тарифы");

      dgvTariffs = new DataGridView
      {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = false,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
      };

      dgvTariffs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", Width = 200 });
      dgvTariffs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена", DataPropertyName = "Price", Width = 120 });
      dgvTariffs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Мин. вес", DataPropertyName = "MinWeight", Width = 120 });
      dgvTariffs.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Макс. вес", DataPropertyName = "MaxWeight", Width = 120 });

      dgvTariffs.ColumnHeaderMouseClick += DgvTariffs_ColumnHeaderMouseClick;

      var panel = CreateButtonsPanel(
                onAdd: BtnAddTariff,
                onEdit: BtnEditTariff,
                onDelete: BtnDeleteTariff
            );

      page.Controls.Add(dgvTariffs);
      page.Controls.Add(panel);

      return page;
    }

    // =======================
    // ПАНЕЛЬ КНОПОК TAB
    // =======================
    private Panel CreateButtonsPanel(Action onAdd, Action onEdit, Action onDelete)
    {
      var panel = new Panel
      {
        Dock = DockStyle.Bottom,
        Height = 50
      };

      var btnAdd = new Button { Text = "Добавить", Left = 10, Width = 120, Top = 10 };
      var btnEdit = new Button { Text = "Изменить", Left = 140, Width = 120, Top = 10 };
      var btnDelete = new Button { Text = "Удалить", Left = 270, Width = 120, Top = 10 };

      btnAdd.Click += (_, __) => onAdd();
      btnEdit.Click += (_, __) => onEdit();
      btnDelete.Click += (_, __) => onDelete();

      panel.Controls.AddRange(new[] { btnAdd, btnEdit, btnDelete });
      return panel;
    }

    // ===============================
    //   ЗАГРУЗКА ВСЕХ ВКЛАДОК
    // ===============================
    private void LoadAllTabs()
    {
      LoadOrders();
      LoadClients();
      LoadTariffs();
    }

    // ######################## ORDERS ########################

    private void LoadOrders()
    {
      var list = OrderRepository.GetAll($"{sortOrdersColumn} {(sortOrdersAsc ? "ASC" : "DESC")}");
      ordersBinding = new BindingList<Order>(list);
      dgvOrders.DataSource = ordersBinding;
    }

    private void DgvOrders_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
      string col = dgvOrders.Columns[e.ColumnIndex].DataPropertyName;

      if (sortOrdersColumn == col)
        sortOrdersAsc = !sortOrdersAsc;
      else
      {
        sortOrdersColumn = col;
        sortOrdersAsc = true;
      }
      LoadOrders();
    }

    private void BtnAddOrder()
    {
      using var f = new OrderForm();
      if (f.ShowDialog() == DialogResult.OK)
        LoadOrders();
    }

    private void BtnEditOrder()
    {
      if (dgvOrders.CurrentRow == null) return;
      var order = dgvOrders.CurrentRow.DataBoundItem as Order;
      if (order == null) return;

      using var f = new OrderForm(order);
      if (f.ShowDialog() == DialogResult.OK)
        LoadOrders();
    }

    private void BtnDeleteOrder()
    {
      if (dgvOrders.CurrentRow == null) return;
      var order = dgvOrders.CurrentRow.DataBoundItem as Order;
      if (order == null) return;

      if (MessageBox.Show("Удалить заказ?", "Подтвердите", MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        OrderRepository.Delete(order.Id);
        LoadOrders();
      }
    }

    // ######################## CLIENTS ########################

    private void LoadClients()
    {
      var list = ClientRepository.GetAll()
                .OrderByDynamic(sortClientsColumn, sortClientsAsc)
                .ToList();

      clientsBinding = new BindingList<Client>(list);
      dgvClients.DataSource = clientsBinding;
    }

    private void DgvClients_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
      string col = dgvClients.Columns[e.ColumnIndex].DataPropertyName;

      if (sortClientsColumn == col)
        sortClientsAsc = !sortClientsAsc;
      else
      {
        sortClientsColumn = col;
        sortClientsAsc = true;
      }

      LoadClients();
    }

    private void BtnAddClient()
    {
      using var f = new ClientForm();
      if (f.ShowDialog() == DialogResult.OK)
        LoadClients();
    }

    private void BtnEditClient()
    {
      if (dgvClients.CurrentRow == null) return;
      var c = dgvClients.CurrentRow.DataBoundItem as Client;
      if (c == null) return;

      using var f = new ClientForm(c);
      if (f.ShowDialog() == DialogResult.OK)
        LoadClients();
    }

    private void BtnDeleteClient()
    {
      if (dgvClients.CurrentRow == null) return;
      var c = dgvClients.CurrentRow.DataBoundItem as Client;
      if (c == null) return;

      if (OrderRepository.ExistsForClient(c.Name))
      {
        MessageBox.Show("Нельзя удалить клиента — он используется в заказах.");
        return;
      }

      ClientRepository.Delete(c.Name);
      LoadClients();
    }

    // ######################## TARIFFS ########################

    private void LoadTariffs()
    {
      var list = TariffRepository.GetAll()
                .OrderByDynamic(sortTariffsColumn, sortTariffsAsc)
                .ToList();

      tariffsBinding = new BindingList<Tariff>(list);
      dgvTariffs.DataSource = tariffsBinding;
    }

    private void DgvTariffs_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
      string col = dgvTariffs.Columns[e.ColumnIndex].DataPropertyName;

      if (sortTariffsColumn == col)
        sortTariffsAsc = !sortTariffsAsc;
      else
      {
        sortTariffsColumn = col;
        sortTariffsAsc = true;
      }

      LoadTariffs();
    }

    private void BtnAddTariff()
    {
      using var f = new TariffForm();
      if (f.ShowDialog() == DialogResult.OK)
        LoadTariffs();
    }

    private void BtnEditTariff()
    {
      if (dgvTariffs.CurrentRow == null) return;
      var t = dgvTariffs.CurrentRow.DataBoundItem as Tariff;
      if (t == null) return;

      using var f = new TariffForm(t);
      if (f.ShowDialog() == DialogResult.OK)
        LoadTariffs();
    }

    private void BtnDeleteTariff()
    {
      if (dgvTariffs.CurrentRow == null) return;
      var t = dgvTariffs.CurrentRow.DataBoundItem as Tariff;
      if (t == null) return;

      if (OrderRepository.ExistsForTariff(t.Name))
      {
        MessageBox.Show("Нельзя удалить тариф — он используется в заказах.");
        return;
      }

      TariffRepository.Delete(t.Name);
      LoadTariffs();
    }

    // ============================
    // JSON IMPORT / EXPORT
    // ============================
    private void BtnLoadJson_Click(object? sender, EventArgs e)
    {
      var dlg = new OpenFileDialog { Filter = "JSON|*.json" };
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        FileService.ImportJsonToDb(dlg.FileName);
        LoadAllTabs();
        MessageBox.Show("Данные успешно импортированы");
      }
    }

    private void BtnSaveJson_Click(object? sender, EventArgs e)
    {
      var dlg = new SaveFileDialog { Filter = "JSON|*.json" };
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        FileService.ExportDbToJson(dlg.FileName);
        MessageBox.Show("Данные сохранены в JSON");
      }
    }
  }

  // ===============================
  // ВСПОМОГАТЕЛЬНЫЙ ДЕЛЕГАТ LINQ
  // ===============================
  public static class OrderByHelper
  {
    public static IOrderedEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> src, string prop, bool asc)
    {
      var p = typeof(T).GetProperty(prop);
      return asc
          ? src.OrderBy(x => p.GetValue(x))
          : src.OrderByDescending(x => p.GetValue(x));
    }
  }
}
