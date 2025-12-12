using System;
using System.Globalization;
using System.Windows.Forms;
using WinFormsCargoApp.Logic;
using WinFormsCargoApp.Services;

namespace WinFormsCargoApp
{
    public class TariffForm : Form
    {
        private TextBox tbName;
        private TextBox tbPrice;
        private TextBox tbMinW;
        private TextBox tbMaxW;

        private Button btnSave, btnCancel;

        private Tariff editingTariff;

        public TariffForm()
        {
            InitializeUI();
            Text = "Добавить тариф";
        }

        public TariffForm(Tariff tariff) : this()
        {
            editingTariff = tariff;
            Text = "Изменить тариф";
            LoadTariff(tariff);
        }

        private void InitializeUI()
        {
            Width = 400;
            Height = 280;
            StartPosition = FormStartPosition.CenterParent;

            int labelX = 10;
            int tbX = 150;
            int w = 200;

            var lbl1 = new Label { Text = "Название:", Left = labelX, Top = 20, Width = 120 };
            tbName = new TextBox { Left = tbX, Top = 20, Width = w };

            var lbl2 = new Label { Text = "Цена (руб/т):", Left = labelX, Top = 60, Width = 120 };
            tbPrice = new TextBox { Left = tbX, Top = 60, Width = w };

            var lbl3 = new Label { Text = "Мин. вес:", Left = labelX, Top = 100, Width = 120 };
            tbMinW = new TextBox { Left = tbX, Top = 100, Width = w };

            var lbl4 = new Label { Text = "Макс. вес:", Left = labelX, Top = 140, Width = 120 };
            tbMaxW = new TextBox { Left = tbX, Top = 140, Width = w };

            btnSave = new Button { Text = "Сохранить", Left = 70, Top = 190, Width = 120 };
            btnCancel = new Button { Text = "Отмена", Left = 210, Top = 190, Width = 120 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lbl1, tbName,
                lbl2, tbPrice,
                lbl3, tbMinW,
                lbl4, tbMaxW,
                btnSave, btnCancel
            });
        }

        private void LoadTariff(Tariff t)
        {
            tbName.Text = t.Name;
            tbPrice.Text = t.Price.ToString(CultureInfo.InvariantCulture);
            tbMinW.Text = t.MinWeight.ToString(CultureInfo.InvariantCulture);
            tbMaxW.Text = double.IsInfinity(t.MaxWeight) ? "" : t.MaxWeight.ToString(CultureInfo.InvariantCulture);

            // Имя менять нельзя — используется в заказах
            tbName.Enabled = false;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string name = tbName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название тарифа");
                return;
            }

            if (!double.TryParse(tbPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double price) || price <= 0)
            {
                MessageBox.Show("Некорректная цена");
                return;
            }

            if (!double.TryParse(tbMinW.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double minW) || minW < 0)
            {
                MessageBox.Show("Некорректный минимальный вес");
                return;
            }

            double maxW;
            if (string.IsNullOrWhiteSpace(tbMaxW.Text))
                maxW = double.MaxValue;
            else if (!double.TryParse(tbMaxW.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out maxW) || maxW <= 0)
            {
                MessageBox.Show("Некорректный максимальный вес");
                return;
            }

            if (maxW <= minW)
            {
                MessageBox.Show("Максимальный вес должен быть больше минимального");
                return;
            }

            // ========== Добавление ==========
            if (editingTariff == null)
            {
                if (TariffRepository.Exists(name))
                {
                    MessageBox.Show("Тариф с таким названием уже существует");
                    return;
                }

                TariffRepository.Insert(new Tariff(name, price, minW, maxW));
            }
            else
            {
                // ========== Редактирование ==========
                editingTariff.Price = price;
                editingTariff.MinWeight = minW;
                editingTariff.MaxWeight = maxW;

                TariffRepository.Update(editingTariff);
            }

            DialogResult = DialogResult.OK;
        }
    }
}
