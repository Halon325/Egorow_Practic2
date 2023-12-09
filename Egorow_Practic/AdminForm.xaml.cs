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
using System.Windows.Threading;

namespace Egorow_Practic
{
    /// <summary>
    /// Логика взаимодействия для AdminForm.xaml
    /// </summary>
    public partial class AdminForm : Window
    {
        // Создание экземпляра базы данных
        Egorow_Dem23Entities1 MyDataBase = new Egorow_Dem23Entities1();
        // Создание таймера
        DispatcherTimer timer = new DispatcherTimer();
        // Установка начального времени в 10 минут (600 секунд)
        int timeLeft = 600;
        public AdminForm()
        {
            InitializeComponent();
            // Обновление заказов
            OrderUpdater.UpdateOrders();
            // Получение списка логинов сотрудников
            var itemsEmployee = MyDataBase.Employee.Select(z => z.Login).ToList();
            itemsEmployee.Add("Отобразить всех");
            // Установка источника данных для выпадающего списка логинов сотрудников
            LoginEmployee.ItemsSource = itemsEmployee;
            // Установка источника данных для списка заказов
            OrderListView.ItemsSource = MyDataBase.OrderView.ToList();
            // Установка источника данных для списка сотрудников
            EmployeeListView.ItemsSource = MyDataBase.EmployeeView.ToList();
            // Установка интервала таймера в 1 секунду
            timer.Interval = TimeSpan.FromSeconds(1);
            // Добавление обработчиков событий для движения мыши и нажатия клавиш
            this.MouseMove += MainWindow_MouseMove;
            this.KeyDown += MainWindow_KeyDown;
            // Добавление обработчика события для таймера
            timer.Tick += Timer_Tick;
            // Запуск таймера
            timer.Start();
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            // Сброс таймера и времени при движении мыши
            timer.Stop();
            timeLeft = 600;
            timer.Start();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Сброс таймера и времени при нажатии клавиши
            timer.Stop();
            timeLeft = 600;
            timer.Start();
        }
        MessageBoxWindow messageBoxWindow;
        // Обработчик события для таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Обновление заказов
            OrderUpdater.UpdateOrders();
            // Уменьшение оставшегося времени
            timeLeft--;
            // Обновление отображения оставшегося времени
            TimerBlocking.Text = "Таймер: " + TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");
            // Показывает предупреждение, когда остается пять минут
            if (timeLeft == 300)
            {
                timer.Stop();
                timeLeft = 300;
                timer.Start();
                messageBoxWindow = new MessageBoxWindow("Таймер", "До закрытия окна осталось пять минут", "", "", "Закрыть");
                messageBoxWindow.ShowDialog();
            }
            // Блокирует вход, когда время истекает
            else if (timeLeft <= 0)
            {
                MainWindow mainWindow = new MainWindow(false);
                mainWindow.Show();
                messageBoxWindow.Close();
                this.Close();
                timer.Stop(); // Остановка таймера
            }
        }
        // Счетчик кликов для кнопки разворачивания окна
        public int countClickUnWrap = 0;
        // Обработчик события для кнопки разворачивания окна
        private void UnWrap_Click(object sender, RoutedEventArgs e)
        {
            // Переключение между развернутым и нормальным состоянием окна
            if (countClickUnWrap == 0)
            {
                UnWrap.Content = "◱";
                this.WindowState = WindowState.Maximized;
                countClickUnWrap++;
            }
            else
            {
                UnWrap.Content = "▢";
                this.WindowState = WindowState.Normal;
                countClickUnWrap = 0;
            }
        }
        // Обработчик события для кнопки сворачивания окна
        private void RollUp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        // Обработчик события для кнопки закрытия окна
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Подтверждение закрытия окна
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Подтверждение", "Вы действительно хотите закрыть окно?", "Да", "Нет", "");
            messageBoxWindow.ShowDialog();
            if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
        // Обработчик события для изменения расположения окна
        private void DragAndDrop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        // Обработчик события для текстового поля поиска заказов
        private void SearchOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Получение списка заказов
            List<OrderView> orderView = MyDataBase.OrderView.ToList();
            // Проверка, если поле для поиска пустое, то выводится представление
            if (SearchOrder.Text.Length == 0)
            {
                OrderListView.ItemsSource = MyDataBase.OrderView.ToList();
            }
            else
            {
                // Осуществление поиска заказов, содержащих введенное слово
                string searchText = SearchOrder.Text;
                var filteredRecords = orderView.Where(r => r.Client.ToLower().StartsWith(searchText.ToLower()) |
                r.Service.ToLower().StartsWith(searchText.ToLower()) |
                r.DateCreate.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.TimeOrder.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.TimeRental.ToString().ToLower().StartsWith(searchText.ToLower())
                | r.DateClose.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.Price.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.Street.ToLower().StartsWith(searchText.ToLower())
                | r.Status.ToLower().StartsWith(searchText.ToLower()));
                // Вывод найденных заказов
                OrderListView.ItemsSource = filteredRecords;
            }
        }

        // Обработчик события для изменения выбранного элемента в выпадающем списке сотрудников
        private void LoginEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка, если выбран элемент "Отобразить всех", то отображаются все сотрудники
            if (LoginEmployee.SelectedItem != null && LoginEmployee.SelectedItem.ToString() == "Отобразить всех")
                EmployeeListView.ItemsSource = MyDataBase.EmployeeView.ToList();
            else if (LoginEmployee.SelectedItem != null)
                // Если выбран конкретный сотрудник, то отображается только этот сотрудник
                EmployeeListView.ItemsSource = MyDataBase.EmployeeView.Where(z => z.Login == LoginEmployee.SelectedItem.ToString()).ToList();
            else
                // Если ничего не выбрано, то отображаются все сотрудники
                EmployeeListView.ItemsSource = MyDataBase.EmployeeView.ToList();
        }

        // Обработчик события для кнопки очистки фильтра
        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            // Очистка выбранного элемента в выпадающем списке
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoginEmployee.SelectedItem = null;
                // Добавление логинов в выпадающий список и элемента для отображения всех сотрудников
                var items = MyDataBase.Employee.Select(z => z.Login).ToList();
                items.Add("Отобразить всех");
                LoginEmployee.ItemsSource = items;
                // Отображение всех сотрудников
                EmployeeListView.ItemsSource = MyDataBase.EmployeeView.ToList();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        // Обработчик события при активации окна
        private void Window_Activated(object sender, EventArgs e)
        {
            // Создание нового экземпляра базы данных
            MyDataBase = new Egorow_Dem23Entities1();
            // Обновление списка заказов и сотрудников
            OrderListView.ItemsSource = MyDataBase.OrderView.ToList();
            EmployeeListView.ItemsSource = MyDataBase.EmployeeView.ToList();
            // Обновление заказов
            OrderUpdater.UpdateOrders();
        }
    }
}

