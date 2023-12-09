using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для CreateOrderForm.xaml
    /// </summary>
    public partial class CreateOrderForm : Window
    {
        Egorow_Dem23Entities1 MyDataBase = new Egorow_Dem23Entities1();
        DispatcherTimer timer = new DispatcherTimer();
        int timeLeft = 600; // Установите начальное время в 10 минуты (600 секунд)
        public CreateOrderForm(int idUser)
        {
            InitializeComponent();
            // Обновление заказов
            OrderUpdater.UpdateOrders();
            // Установка заголовка окна в зависимости от должности пользователя
            if (idUser == 2)
                TitleWindow.Text = "Продавец";
            else
                TitleWindow.Text = "Старший смены";
            // Установка источников данных для списков заказов, клиентов и услуг
            OrderClient.ItemsSource = MyDataBase.Client.ToList();
            OrderService.ItemsSource = MyDataBase.Service.ToList();
            ClientListView.ItemsSource = MyDataBase.ClientView.ToList();
            ServiceListView.ItemsSource = MyDataBase.Service.ToList();
            // Установка интервала таймера в 1 секунду
            timer.Interval = TimeSpan.FromSeconds(1);
            // Добавление обработчиков событий для движения мыши и нажатия клавиш
            this.MouseMove += MainWindow_MouseMove;
            this.KeyDown += MainWindow_KeyDown;
            // Добавление обработчика события для таймера
            timer.Tick += Timer_Tick;
            // Запуск таймера
            timer.Start();
            // Заполнение выпадающего списка времени аренды
            for (int i = 40; i <= 720; i = i + 40)
            {
                TimeRentalCB.Items.Add(i.ToString());
            }
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            // Сбросьте таймер и время при движении мыши
            timer.Stop();
            timeLeft = 600;
            timer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Сбросьте таймер и время при нажатии клавиши
            timer.Stop();
            timeLeft = 600;
            timer.Start();
        }
        MessageBoxWindow messageBoxWindow;
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            OrderUpdater.UpdateOrders();
            // Обновите отображение оставшегося времени
            TimerBlocking.Text = "Таймер: " + TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");
            if (timeLeft == 300)
            {
                timer.Stop();
                timeLeft = 300; timer.Start();
                messageBoxWindow = new MessageBoxWindow("Таймер", "До закрытия окна осталось пять минут", "", "", "Закрыть");
                // Покажите предупреждение, когда остается одна минута
                messageBoxWindow.ShowDialog();
            }
            else if (timeLeft <= 0)
            {
                MainWindow mainWindow = new MainWindow(false);
                mainWindow.Show();
                messageBoxWindow.Close();
                this.Close();
                // Ваш код для блокировки входа здесь
                timer.Stop(); // Остановите таймер
            }
        }
        public int countClickUnWrap = 0;
        private void UnWrap_Click(object sender, RoutedEventArgs e) // Развернуть окно на весь экран
        {
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
        private void RollUp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Сворачивание окна
        }

        // Обработчик события для кнопки закрытия окна
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Показывает окно подтверждения перед закрытием
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Подтверждение", "Вы действительно хотите закрыть окно?", "Да", "Нет", "");
            messageBoxWindow.ShowDialog();
            // Если пользователь подтвердил закрытие окна
            if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes)
            {
                // Открывает главное окно
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                // Обновляет заказы
                OrderUpdater.UpdateOrders();
                // Изменяет статус заказов с idStatus 3 на 1
                using (var context = new Egorow_Dem23Entities1())
                {
                    var orders = context.Order.Where(o => o.idStatus == 3);
                    foreach (var order in orders)
                    {
                        order.idStatus = 1;
                    }
                    context.SaveChanges();
                }
                // Закрывает текущее окно
                this.Close();
            }
        }
        private void DragAndDrop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove(); // Изменение расположения
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient addClient = new AddClient(); // Открытие окна для добавления клиента
            addClient.ShowDialog();
        }

        // Обработчик события для кнопки добавления услуги
        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxWindow messageBoxWindow;
            try
            {
                // Генерация кода услуги
                string CodeService = CodeServiceCreate();
                // Проверка на существование услуги с таким же кодом
                if (MyDataBase.Service.Any(z => z.CodeService == CodeService))
                {
                    CodeService = CodeServiceCreate();
                }
                else
                {
                    // Проверка на корректность введенных данных
                    if (TitleService.Text.Length > 0 && !MyDataBase.Service.Any(z => z.Title == TitleService.Text) && PriceService.Text.Length > 0)
                    {
                        // Создание новой услуги
                        var service = new Service
                        {
                            IdService = MyDataBase.Service.Max(z => z.IdService + 1),
                            Title = TitleService.Text,
                            CodeService = CodeService,
                            Price = Convert.ToDecimal(PriceService.Text)
                        };
                        // Добавление новой услуги в базу данных
                        MyDataBase.Service.Add(service);
                        MyDataBase.SaveChanges();
                        // Обновление списка услуг
                        MyDataBase = new Egorow_Dem23Entities1();
                        ServiceListView.ItemsSource = MyDataBase.Service.ToList();
                    }
                    else
                    {
                        // Вывод сообщения об ошибке при некорректном вводе данных
                        messageBoxWindow = new MessageBoxWindow("Добавление услуг", "Некорректный ввод данных. " +
                            "Это может быть связанно с тем, что данная услуга уже существует, либо поля для ввода не имеют значения." +
                            "\nПовторите ввод.", "", "", "Закрыть");
                        messageBoxWindow.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке
                messageBoxWindow = new MessageBoxWindow($"Добавление услуги", ex.Message, "", "", "Закрыть");
                messageBoxWindow.ShowDialog();
            }
        }
        // Функция для создания кода услуги
        public string CodeServiceCreate()
        {
            string codeService = "";
            Random random = new Random();
            // Генерация случайной длины от 5 до 10
            int length = random.Next(5, 11);
            for (int i = 0; i < length; i++)
            {
                // Определение, будет ли следующий символ буквой или цифрой
                int type = random.Next(0, 2);
                if (type == 0)
                {
                    // Генерация случайной цифры
                    codeService += random.Next(0, 10).ToString();
                }
                else
                {
                    // Генерация случайной заглавной буквы
                    codeService += (char)random.Next('A', 'Z' + 1);
                }
            }
            return codeService;
        }
        // Обработчик события для текстового поля поиска клиентов
        private void SearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Получение списка клиентов
            List<ClientView> clientView = MyDataBase.ClientView.ToList();
            string searchText = "";
            // Проверка, если поле для поиска пустое, то выводится представление
            if (SearchClient.Text.Length == 0)
            {
                ClientListView.ItemsSource = MyDataBase.ClientView.ToList();
            }
            else
            {
                // Осуществление поиска клиентов, содержащих введенное слово
                searchText = SearchClient.Text;
                var filteredRecords = clientView.Where(r => r.Client.ToLower().StartsWith(searchText.ToLower()) |
                r.DateOfBirth.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.SeriesPassport.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.NumberPassport.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.Index.ToString().ToLower().StartsWith(searchText.ToLower())
                | r.Title.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.Street.ToString().ToLower().StartsWith(searchText.ToLower()) |
                r.Email.ToLower().StartsWith(searchText.ToLower())
                | r.Id.ToString().ToLower().StartsWith(searchText.ToLower()));
                // Вывод найденных клиентов
                ClientListView.ItemsSource = filteredRecords;
            }

            // Поиск клиентов для заказа
            List<Client> clients = MyDataBase.Client.ToList();
            if (SearchOrderClient.Text.Length == 0)
                OrderClient.ItemsSource = clients;
            else
            {
                searchText = SearchOrderClient.Text;
                var filterOrderClient = clients.Where(z => z.Surname.ToLower().StartsWith(searchText.ToLower()) |
                z.Name.ToLower().StartsWith(searchText.ToLower()) | z.Forename.ToLower().StartsWith(searchText.ToLower()) |
                z.Id.ToString().ToLower().StartsWith(searchText.ToLower()));
                OrderClient.ItemsSource = filterOrderClient;
            }

            // Установка флага выбора для клиентов
            foreach (var client in clients)
            {
                if (ClientTextBlock.Text == $"{client.Surname} {client.Name} {client.Forename} ({client.Id})")
                    client.IsChecked = true;
            }
        }
        // Обработчик события для текстового поля поиска услуг
        private void SearchService_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = "";
            // Получение списка всех услуг
            List<Service> allServices = MyDataBase.Service.ToList();
            // Проверка, если поле для поиска пустое, то выводятся все услуги
            if (SearchService.Text.Length == 0)
                ServiceListView.ItemsSource = allServices;
            else
            {
                // Осуществление поиска услуг, содержащих введенное слово
                searchText = SearchService.Text;
                var filter = allServices.Where(z => z.Title.ToLower().StartsWith(searchText.ToLower()) |
                   z.Price.ToString().ToLower().StartsWith(searchText.ToLower()) |
                   z.CodeService.ToLower().StartsWith(searchText.ToLower()));
                ServiceListView.ItemsSource = filter;
            }
            // Поиск услуг для заказа
            if (SearchOrderService.Text.Length == 0)
                OrderService.ItemsSource = allServices;
            else
            {
                searchText = SearchOrderService.Text;
                var filter = allServices.Where(z => z.Title.ToLower().StartsWith(searchText.ToLower()) |
                   z.Price.ToString().ToLower().StartsWith(searchText.ToLower()) |
                   z.CodeService.ToLower().StartsWith(searchText.ToLower()));
                OrderService.ItemsSource = filter;
            }
        }
        int codeClient = 0;
        // Обработчик события для выбора клиента
        private void ClientRB_Checked(object sender, RoutedEventArgs e)
        {
            // Получение выбранного клиента
            var selectedClient = (Client)((RadioButton)sender).DataContext;
            selectedClient.IsChecked = true;
            // Обновление TextBlock с ФИО клиента
            ClientTextBlock.Text = $"{selectedClient.Surname} {selectedClient.Name} {selectedClient.Forename}";
            codeClient = selectedClient.Id;
        }

        // Обработчик события для выбора услуги
        private void ServiceCB_Checked(object sender, RoutedEventArgs e)
        {
            // Получение выбранной услуги
            var selectedService = (Service)((CheckBox)sender).DataContext;
            selectedService.IsChecked = true;
            // Добавление услуги в GroupBox
            SelectedServicesPanel.Children.Add(new TextBlock { Text = $"{selectedService.Title}" });
            // Отключение всех CheckBox в ListBox
            foreach (var item in OrderService.Items)
            {
                var checkBox = FindCheckBoxInItem(item);
                if (checkBox != null && checkBox != sender)
                {
                    checkBox.IsEnabled = false;
                }
            }
            // Отключение выбора услуг
            OrderService.IsEnabled = false;
        }

        // Обработчик события для снятия выбора услуги
        private void ServiceCB_Unchecked(object sender, RoutedEventArgs e)
        {
            // Получение выбранной услуги
            var selectedService = (Service)((CheckBox)sender).DataContext;
            selectedService.IsChecked = false;
            // Удаление услуги из GroupBox
            foreach (TextBlock textBlock in SelectedServicesPanel.Children)
            {
                if (textBlock.Text == $"{selectedService.Title}")
                {
                    SelectedServicesPanel.Children.Remove(textBlock);
                    break;
                }
            }
            // Если все услуги сняты с выбора, отключение выбора услуг
            if (SelectedServicesPanel.Children.Count == 0)
            {
                OrderService.IsEnabled = false;
            }
        }

        // Обработчик события для кнопки добавления новой услуги
        private void AddNewService_Click(object sender, RoutedEventArgs e)
        {
            // Включение только тех CheckBox в ListBox, которые еще не были выбраны
            foreach (var item in OrderService.Items)
            {
                var checkBox = FindCheckBoxInItem(item);
                if (checkBox != null && !checkBox.IsChecked.GetValueOrDefault())
                {
                    checkBox.IsEnabled = true;
                }
            }
            // Включение выбора услуг
            OrderService.IsEnabled = true;
        }
        // Функция для поиска CheckBox в элементе ListBox
        private CheckBox FindCheckBoxInItem(object item)
        {
            // Поиск CheckBox в элементе ListBox
            var container = (ListBoxItem)OrderService.ItemContainerGenerator.ContainerFromItem(item);
            if (container != null)
            {
                return container.FindName("ServiceCB") as CheckBox;
            }
            return null;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            MyDataBase = new Egorow_Dem23Entities1();
            OrderClient.ItemsSource = MyDataBase.Client.ToList();
            OrderService.ItemsSource = MyDataBase.Service.ToList();
            ClientListView.ItemsSource = MyDataBase.ClientView.ToList();
            ServiceListView.ItemsSource = MyDataBase.Service.ToList();
            OrderUpdater.UpdateOrders();
        }

        // Обработчик события для кнопки создания заказа
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxWindow messageBoxWindow;
            var selectedClient = (Client)OrderClient.SelectedItem;
            var selectedService = (Service)OrderService.SelectedItem;
            // Проверка, если клиент выбран
            if (selectedClient != null)
            {
                // Показывает окно подтверждения перед созданием заказа
                messageBoxWindow = new MessageBoxWindow("Подтверждение",
                    "Вы действительно хотите сформировать заказ?", "Да", "Нет", "");
                messageBoxWindow.ShowDialog();
                // Если пользователь подтвердил создание заказа
                if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes)
                {
                    // Проверка, если выбраны услуги и время аренды
                    if (SelectedServicesPanel.Children.Count != 0 && TimeRentalCB.Text.Length > 0)
                    {
                        // Создание нового заказа
                        int idOrder = MyDataBase.Order.Max(z => z.Id + 1);
                        DateTime now = DateTime.Now;
                        DateTime date = new DateTime(now.Year, now.Month, now.Day);
                        TimeSpan time = new TimeSpan(now.Hour, now.Minute, now.Second);
                        var order = new Order
                        {
                            Id = idOrder,
                            DateCreate = date,
                            TimeOrder = time,
                            idClient = codeClient,
                            idStatus = 3,
                            DateClose = null,
                            TimeRental = Convert.ToInt32(TimeRentalCB.SelectedItem.ToString())
                        };
                        // Вычисление времени закрытия заказа
                        DateTime closeTime = date.Add(time).AddMinutes(order.TimeRental);
                        // Если время закрытия заказа находится в следующем дне, то устанавливаем DateClose как следующий день
                        if (closeTime.Date > date.Date)
                        {
                            order.DateClose = closeTime.Date;
                        }
                        else // Иначе устанавливаем DateClose как текущий день
                        {
                            order.DateClose = date.Date;
                        }
                        // Добавление нового заказа в базу данных
                        MyDataBase.Order.Add(order);
                        // Добавление выбранных услуг в заказ
                        foreach (TextBlock textBlock in SelectedServicesPanel.Children)
                        {
                            // Получение названия услуги из TextBlock
                            string serviceName = textBlock.Text;
                            // Поиск услуги в базе данных по названию
                            var service = MyDataBase.Service.FirstOrDefault(s => s.Title == serviceName);
                            if (service != null)
                            {
                                var serviceAndOrder = new ServiceandOrder
                                {
                                    idOrder = selectedClient.Id.ToString() + "/" + date.ToString("dd.MM.yyyy"),
                                    DateCreate = date,
                                    idClient = selectedClient.Id,
                                    idService = service.IdService, // Используем id найденной услуги
                                    NumberOrder = idOrder
                                };
                                MyDataBase.ServiceandOrder.Add(serviceAndOrder);
                            }
                        }
                        // Сохранение изменений в базе данных
                        MyDataBase.SaveChanges();
                        // Очистка выбранных услуг и клиента
                        SelectedServicesPanel.Children.Clear();
                        ClientTextBlock.Text = "";
                        OrderService.IsEnabled = true;
                        TimeRentalCB.Text = "";
                    }
                    else
                    {
                        // Вывод сообщения об ошибке при некорректном вводе данных
                        messageBoxWindow = new MessageBoxWindow("Оформление заказа",
               "Необходимо выбрать время аренды.", "", "", "Закрыть");
                        messageBoxWindow.ShowDialog();
                    }
                }
            }
        }
        // Обработчик события для выбора клиента при нажатии на его данные
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Получение выбранного клиента
            var grid = (Grid)sender;
            var radioButton = (RadioButton)grid.FindName("ClientRB");
            // Установка флага выбора для клиента
            radioButton.IsChecked = true;
        }
        // Обработчик события для выбора услуги при нажатии на ее данные
        private void OrderServiceGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Получение выбранной услуги
            var grid = (Grid)sender;
            var checkBox = (CheckBox)grid.FindName("ServiceCB");
            // Установка флага выбора для услуги
            checkBox.IsChecked = true;
        }
    }
}

