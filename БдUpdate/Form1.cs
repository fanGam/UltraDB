using System;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;

namespace БдUpdate
{
    public partial class Form1 : Form
    {
        // Объявление переменных для работы с базой данных и набором данных
        DataSet НаборДанных;
        OleDbDataAdapter Адаптер;
        OleDbConnection Подключение;
        OleDbCommand Команда;

        public Form1()
        {
            InitializeComponent(); // Инициализация компонентов формы
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация набора данных и подключения к базе данных
            НаборДанных = new DataSet();
            Подключение = new OleDbConnection(
                "Provider=SQLOLEDB;Data Source=ROG-ZEPHYRUS-G1\\SQLEXPRESS;Initial Catalog=RestaurantDB;Integrated Security=SSPI;");

            // Настройка текста и индексов кнопок
            button1.Text = "Читать из БД"; button1.TabIndex = 0;
            button2.Text = "Сохранить в БД";
            button3.Text = "Удалить ту строку";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Открытие подключения к базе данных
            if (Подключение.State == ConnectionState.Closed) Подключение.Open();

            // Очистка существующих данных в наборе данных перед загрузкой новых
            if (НаборДанных.Tables.Contains("Ингредиенты"))
            {
                НаборДанных.Tables["Ингредиенты"].Clear();
            }

            // Заполнение набора данных данными из таблицы "Ингредиенты"
            Адаптер = new OleDbDataAdapter("SELECT * FROM [Ингредиенты]", Подключение);
            Адаптер.Fill(НаборДанных, "Ингредиенты");

            // Установка источника данных для DataGridView
            dataGridView1.DataSource = НаборДанных;
            dataGridView1.DataMember = "Ингредиенты";

            // Закрытие подключения
            Подключение.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Подготовка команд для вставки и обновления данных в таблице "Ингредиенты"
            OleDbCommand insertCommand = new OleDbCommand(
                "INSERT INTO [Ингредиенты] ([Название], [Единица_измерения], [Цена_закупки]) VALUES (?, ?, ?)");
            insertCommand.Parameters.Add("Название", OleDbType.VarWChar, 100, "Название");
            insertCommand.Parameters.Add("Единица_измерения", OleDbType.VarWChar, 20, "Единица_измерения");
            insertCommand.Parameters.Add("Цена_закупки", OleDbType.Decimal, 10, "Цена_закупки");

            OleDbCommand updateCommand = new OleDbCommand(
                "UPDATE [Ингредиенты] SET [Название] = ?, [Единица_измерения] = ?, [Цена_закупки] = ? WHERE ([id_ингредиента] = ?)");
            updateCommand.Parameters.Add("Название", OleDbType.VarWChar, 100, "Название");
            updateCommand.Parameters.Add("Единица_измерения", OleDbType.VarWChar, 20, "Единица_измерения");
            updateCommand.Parameters.Add("Цена_закупки", OleDbType.Decimal, 10, "Цена_закупки");
            updateCommand.Parameters.Add(new OleDbParameter("orig_id_ингредиента", OleDbType.Integer, 0, ParameterDirection.Input, false, (Byte)0, (Byte)0, "id_ингредиента", System.Data.DataRowVersion.Original, null));

            // Привязка команд к адаптеру
            Адаптер.InsertCommand = insertCommand;
            Адаптер.UpdateCommand = updateCommand;

            // Установка соединения для команд
            insertCommand.Connection = Подключение;
            updateCommand.Connection = Подключение;

            try
            {
                // Обновление базы данных с использованием данных из набора данных
                var kol = Адаптер.Update(НаборДанных, "Ингредиенты");
                MessageBox.Show("Обновлено " + kol.ToString() + " записей");
            }
            catch (Exception Ситуация)
            {
                // Обработка ошибок при обновлении
                MessageBox.Show(Ситуация.Message, "Недоразумение");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // Проверка, выбрана ли хотя бы одна строка в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;

                // Удаление выбранной строки из набора данных
                НаборДанных.Tables["Ингредиенты"].Rows[selectedIndex].Delete();

                // Подготовка команды для удаления записи из базы данных
                Команда = new OleDbCommand();
                Команда.CommandText = "DELETE FROM [Ингредиенты] WHERE ([id_ингредиента] = ?)";
                Команда.Parameters.Clear();
                Команда.Parameters.Add(new OleDbParameter("id_ингредиента", OleDbType.Integer, 0, "id_ингредиента"));

                // Привязка команды удаления к адаптеру
                Адаптер.DeleteCommand = Команда;
                Команда.Connection = Подключение;

                try
                {
                    // Обновление базы данных с удалением записи
                    var kol = Адаптер.Update(НаборДанных, "Ингредиенты");
                    MessageBox.Show("Удалено " + kol.ToString() + " записей");
                }
                catch (Exception Ситуация)
                {
                    // Обработка ошибок при удалении
                    MessageBox.Show(Ситуация.Message, "Недоразумение");
                }
            }
            else
            {
                // Уведомление пользователя о необходимости выбора записи для удаления
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }
    }
}