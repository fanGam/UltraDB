// Программа обновляет (НЕ ДОБАВЛЯЕТ) записи (Update) в таблице базы данных MS Access
using System;
using System.Data;
using System.Windows.Forms;
// Другие директивы using удалены, поскольку они не используются в данной
// программе
// ~ ~ ~ ~ ~ ~ ~ ~ 
// А данную директиву добавим для краткости выражений:
using System.Data.OleDb;
using System.Data.SqlClient;
namespace БдUpdate
{
    public partial class Form1 : Form
    {
        // ~ ~ ~ ~ ~ ~ ~ ~ 
        // Объявляем эти переменные вне всех процедур, чтобы
        // они были видны из любой из процедур:
        DataSet НаборДанных;
        OleDbDataAdapter Адаптер;
        OleDbConnection Подключение;
        OleDbCommand Команда;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            НаборДанных = new DataSet();
            Подключение = new OleDbConnection(
                    "Provider=SQLOLEDB;Data Source=ROG-ZEPHYRUS-G1\\SQLEXPRESS;Initial Catalog=RestaurantDB;Integrated Security=SSPI;");
            //Подключение = new OleDbConnection(
            //    "Data Source=\"database.mdb\";User " +
            //    "ID=Admin;Provider=\"Microsoft.Jet.OLEDB.4.0\";");
            Команда = new OleDbCommand();
            button1.Text = "Читать из БД"; button1.TabIndex = 0;
            button2.Text = "Сохранить в БД";
            button3.Text = "Удалить ту строку";
        }
        private void button1_Click(object sender, EventArgs e) // Читать из БД
        { 
            if (Подключение.State ==
                            ConnectionState.Closed) Подключение.Open();
            if (НаборДанных.Tables.Contains("Ингредиенты"))
            {
                // Очищаем существующие данные в DataTable
                НаборДанных.Tables["Ингредиенты"].Clear();
            }
            Адаптер = new OleDbDataAdapter(
                            "SELECT * FROM [Ингредиенты]", Подключение);
            // Заполняем DataSet результатом SQL-запроса
            Адаптер.Fill(НаборДанных, "Ингредиенты");
            // Содержимое DataSet в виде строки XML для отладки:
            var СтрокаXML = НаборДанных.GetXml();
            // Указываем источник данных для сетки данных:
            dataGridView1.DataSource = НаборДанных;
            // Указываем имя таблицы в наборе данных:
            dataGridView1.DataMember = "Ингредиенты";
            Подключение.Close();
        }
        private void button2_Click(object sender, EventArgs e) // Сохранить в базе данных
        {
            // Создаем команды
            OleDbCommand insertCommand = new OleDbCommand(
                "INSERT INTO [Ингредиенты] ([Название], [Единица_измерения], [Цена_закупки]) VALUES (?, ?, ?)");
            insertCommand.Parameters.Add("Название", OleDbType.VarWChar, 100, "Название");
            insertCommand.Parameters.Add("Единица_измерения", OleDbType.VarWChar, 20, "Единица_измерения");
            insertCommand.Parameters.Add("Цена_закупки", OleDbType.Decimal, 10, "Цена_закупки");

            // Создаем команду для обновления
            OleDbCommand updateCommand = new OleDbCommand(
                "UPDATE [Ингредиенты] SET [Название] = ?, [Единица_измерения] = ?, [Цена_закупки] = ? WHERE ([id_ингредиента] = ?)");
            updateCommand.Parameters.Add("Название", OleDbType.VarWChar, 100, "Название");
            updateCommand.Parameters.Add("Единица_измерения", OleDbType.VarWChar, 20, "Единица_измерения");
            updateCommand.Parameters.Add("Цена_закупки", OleDbType.Decimal, 10, "Цена_закупки");
            updateCommand.Parameters.Add(new OleDbParameter("orig_id_ингредиента", OleDbType.Integer, 0, ParameterDirection.Input, false, (Byte)0, (Byte)0, "id_ингредиента", System.Data.DataRowVersion.Original, null));

            // Устанавливаем команды для адаптера
            Адаптер.InsertCommand = insertCommand;
            Адаптер.UpdateCommand = updateCommand;

            // Устанавливаем соединение
            insertCommand.Connection = Подключение;
            updateCommand.Connection = Подключение;

            try
            {
                // Update возвращает количество измененных строк
                var kol = Адаптер.Update(НаборДанных, "Ингредиенты");
                MessageBox.Show("Обновлено " + kol.ToString() + " записей");
            }
            catch (Exception Ситуация)
            {
                MessageBox.Show(Ситуация.Message, "Недоразумение");
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем индекс выбранной строки
                int selectedIndex = dataGridView1.SelectedRows[0].Index;

                // Удаляем строку из DataSet
                НаборДанных.Tables["Ингредиенты"].Rows[selectedIndex].Delete();

                // Создаем команду для удаления
                Команда.CommandText = "DELETE FROM [Ингредиенты] WHERE ([id_ингредиента] = ?)";
                Команда.Parameters.Clear();
                Команда.Parameters.Add(new OleDbParameter("id_ингредиента", OleDbType.Integer, 0, "id_ингредиента"));

                Адаптер.DeleteCommand = Команда;
                Команда.Connection = Подключение;

                try
                {
                    // Удаляем запись из базы данных
                    var kol = Адаптер.Update(НаборДанных, "Ингредиенты");
                    MessageBox.Show("Удалено " + kol.ToString() + " записей");
                }
                catch (Exception Ситуация)
                {
                    MessageBox.Show(Ситуация.Message, "Недоразумение");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }
    }
}
