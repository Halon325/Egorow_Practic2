using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Egorow_Practic
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        // Определение результата диалогового окна
        public enum Result
        {
            Yes,
            No,
            Cancel
        }
        public Result DialogResult { get; private set; }
        // Конструктор диалогового окна
        public MessageBoxWindow(string title, string message, string Yes, string Not, string Cancel)
        {
            InitializeComponent();
            Title.Text = title;
            MessageTextBlock.Text = message;

            // Установка видимости кнопок в зависимости от переданных параметров
            OkButton.Visibility = Yes == "Да" ? Visibility.Visible : Visibility.Hidden;
            NotButton.Visibility = Not == "Нет" ? Visibility.Visible : Visibility.Hidden;
            CancelButton.Visibility = Cancel == "Закрыть" ? Visibility.Visible : Visibility.Hidden;
        }
        // Обработчики событий для кнопок диалогового окна
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = Result.Yes;
            this.Close();
        }
        private void NotButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = Result.No;
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = Result.Cancel;
            this.Close();
        }
        // Обработчик события для перемещения окна
        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
