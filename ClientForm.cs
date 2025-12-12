using System;
using System.Windows.Forms;
using WinFormsCargoApp.Logic;
using WinFormsCargoApp.Services;

namespace WinFormsCargoApp
{
    public class ClientForm : Form
    {
        private TextBox tbName;
        private ComboBox cbType;
        private Button btnSave, btnCancel;

        private Client editingClient;

        public ClientForm()
        {
            InitializeUI();
            Text = "Добавить клиента";
        }

        public ClientForm(Client client) : this()
        {
            editingClient = client;
            Text = "Изменить клиента";

            LoadClient(client);
        }

        private void InitializeUI()
        {
            Width = 350;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;

            var lbl1 = new Label { Text = "Имя:", Left = 10, Top = 20, Width = 100 };
            tbName = new TextBox { Left = 120, Top = 20, Width = 180 };

            var lbl2 = new Label { Text = "Тип:", Left = 10, Top = 60, Width = 100 };
            cbType = new ComboBox
            {
                Left = 120,
                Top = 60,
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbType.Items.Add(ClientType.Обычный);
            cbType.Items.Add(ClientType.Постоянный);
            cbType.SelectedIndex = 0;

            btnSave = new Button { Text = "Сохранить", Left = 40, Top = 110, Width = 120 };
            btnCancel = new Button { Text = "Отмена", Left = 180, Top = 110, Width = 120 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lbl1, tbName,
                lbl2, cbType,
                btnSave, btnCancel
            });
        }

        private void LoadClient(Client c)
        {
            tbName.Text = c.Name;
            cbType.SelectedItem = c.Type;

            // Имя менять нельзя — иначе нарушим FK логики заказов
            tbName.Enabled = false;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string name = tbName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите имя клиента");
                return;
            }

            var type = (ClientType)cbType.SelectedItem;

            if (editingClient == null)
            {
                // === ДОБАВЛЕНИЕ ===
                if (ClientRepository.Exists(name))
                {
                    MessageBox.Show("Клиент с таким именем уже существует");
                    return;
                }

                ClientRepository.Insert(new Client(name, type));
            }
            else
            {
                // === РЕДАКТИРОВАНИЕ ===
                editingClient.Type = type;
                ClientRepository.Update(editingClient);
            }

            DialogResult = DialogResult.OK;
        }
    }
}
