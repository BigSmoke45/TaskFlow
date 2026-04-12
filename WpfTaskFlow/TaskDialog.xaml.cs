using System.Windows;

namespace WpfTaskFlow
{
    public partial class TaskDialog : Window
    {
        // Результати в MainWindow
        public string TaskTitle { get; private set; } = "";
        public string TaskDescription { get; private set; } = "";
        public string TaskPriority { get; private set; } = "Medium";

        // Конструктор для нової задачі
        public TaskDialog()
        {
            InitializeComponent();
        }

        // Конструктор для редагування заповнює поля існуючими даними
        public TaskDialog(string title, string? description, string priority)
        {
            InitializeComponent();
            TitleBox.Text = title;
            DescBox.Text = description ?? "";

            // Правильний пункт в ComboBox
            foreach (System.Windows.Controls.ComboBoxItem item in PriorityBox.Items)
            {
                if (item.Content.ToString() == priority)
                {
                    PriorityBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Назва не може бути порожньою.", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TaskTitle = TitleBox.Text.Trim();
            TaskDescription = DescBox.Text.Trim();
            TaskPriority = (PriorityBox.SelectedItem as System.Windows.Controls.ComboBoxItem)
                           ?.Content.ToString() ?? "Medium";

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}