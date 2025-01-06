using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalseChart
{
    public partial class Form1 : Form
    {
        private class SaleRecord
        {
            public DateTime SaleDate { get; set; }
            public decimal TotalWithTax { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DrawChart();
        }
        private void DrawChart()
        {
            string connectionString = "Data Source=your_database.db;";
            List<SaleRecord> sales = new List<SaleRecord>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT SALE_DATE, TOTAL_WITH_TAX FROM TAB_SALE";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string saleDateString = reader.GetString(0);
                        DateTime saleDate = DateTime.ParseExact(saleDateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        decimal totalWithTax = reader.GetDecimal(1);

                        sales.Add(new SaleRecord { SaleDate = saleDate, TotalWithTax = totalWithTax });
                    }
                }
            }

            // 時間別の合計を計算
            var hourlySales = sales
                .GroupBy(s => new DateTime(s.SaleDate.Year, s.SaleDate.Month, s.SaleDate.Day, s.SaleDate.Hour, 0, 0))
                .Select(g => new { Time = g.Key, TotalWithTax = g.Sum(s => s.TotalWithTax) })
                .OrderBy(g => g.Time)
                .ToList();

        }
    }

}
