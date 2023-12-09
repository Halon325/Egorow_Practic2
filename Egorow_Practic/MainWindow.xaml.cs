using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Egorow_Practic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int counter = 10;
        ContextMenu contextMenu = new ContextMenu();
        Egorow_Dem23Entities1 MyDataBase = new Egorow_Dem23Entities1();
        public MainWindow()
        {
            InitializeComponent();
            OrderUpdater.UpdateOrders(); // Обновляем записи
            OffContextMenu();
            Timer();
            ClearCaptcha();
        }
        public int timeBlock = 180; // Создание таймера блокировки на 3 минуты, если пользователь был неактивен при входе
        DispatcherTimer timerBlock;
        public void Block(bool BlockingControl) // Блокировка входа, если окно пользователя было неактивно
        {
            Enter.IsEnabled = false;
            if (BlockingControl == false)
            {
                timerBlock = new DispatcherTimer();
                timerBlock.Interval = TimeSpan.FromSeconds(1);
                timerBlock.Tick += TimerBlock_Tick;
                // Запустите таймер
                timerBlock.Start();
            }
        }
        private void TimerBlock_Tick(object sender, EventArgs e)
        {
            if (timeBlock > 0)
            {
                timeBlock--;
                Enter.Content = TimeSpan.FromSeconds(timeBlock).ToString(@"mm\:ss");
            }
            else
            {
                Enter.IsEnabled = true;
                timerBlock.Stop(); // Остановка таймера
            }
        }
        public MainWindow(bool CheckBlock)
        {
            InitializeComponent();
            OrderUpdater.UpdateOrders(); // Обновляем записи заказов
            ClearCaptcha(); // Очищаем капчу
            Block(CheckBlock); // Проверяем нужна ли блокировка
            OffContextMenu(); // Убираем контекстное меню
            Timer(); // Вызываем метод для капчи
        }
        public void Timer()
        {
            timer = new DispatcherTimer(); // Создание таймера на 10 секунд блокировки при неправильном вводе капчи
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        public void OffContextMenu() // Очистка контекстного меню с поля для ввода пароля
        {
            contextMenu.Items.Clear();
            contextMenu.Visibility = Visibility.Hidden;
            this.Password.ContextMenu = contextMenu;
            this.PasswordText.ContextMenu = contextMenu;
        }
        private void ClearCaptcha() //Метод очищающий капчу, а также убирает блокировку
        {
            CanvasCaptcha.Children.Clear();
            Captcha.Visibility = Visibility.Hidden;
            Enter.IsEnabled = true;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (counter > 0)
                counter--;
            else
            {
                timer.Stop(); // Как только 10 секунд проходят, снимается блокировка кнопки
                Enter.IsEnabled = true;
                counter = 10;
            }
        }
        private void DragAndDrop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove(); // Изменение расположения
        }

        private void RollUp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Сворачивание окна
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Подтверждение", "Вы действительно хотите закрыть окно?", "Да", "Нет", "");
            messageBoxWindow.ShowDialog();
            if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes) // Подтверждение закрытия окна
            {
                this.Close();
            }
        }
        private void Login_PreviewKeyDown(object sender, KeyEventArgs e) // Запрещает копирование, вставку и вырезку данных с поля для ввода пароля
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control || e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private Random random = new Random();
        public int CountEnter = 0;
        // Обработчик события для кнопки входа
        private void PasswordText_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Password_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void Eye_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Visibility == Visibility.Visible) // По нажатию на глазик будет отображаться "открытый" или "закрытый" глазик
            {
                Password.Visibility = Visibility.Collapsed;
                PasswordText.Visibility = Visibility.Visible;
                PasswordText.Text = Password.Password;
                EyeImage.Source = new BitmapImage(new Uri("Picture\\EyeShow.png", UriKind.Relative));
            }
            else
            {
                Password.Visibility = Visibility.Visible;
                PasswordText.Visibility = Visibility.Collapsed;
                Password.Password = PasswordText.Text;
                EyeImage.Source = new BitmapImage(new Uri("Picture\\EyeClose.png", UriKind.Relative));
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isEye();
                // Получение введенного пароля
                string password = Password.Visibility != Visibility.Hidden ? Password.Password : PasswordText.Text;
                string passwordText = PasswordText.Visibility != Visibility.Hidden ? PasswordText.Text : Password.Password;
                // Проверка существования пользователя с введенными логином и паролем
                var user = MyDataBase.Employee.FirstOrDefault(u => u.Login.ToString() == Login.Text &&
                (u.Password.ToString() == password || u.Password.ToString() == passwordText));
                // Если пользователь найден и капча введена верно
                if (user != null && textCaptcha == InputCaptcha.Text)
                {
                    var employee = MyDataBase.Employee.FirstOrDefault(j => j.Login == Login.Text);
                    DateTime now = DateTime.Now;
                    DateTime dateWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                    // Добавление записи о входе в историю сотрудника
                    var EmployeeHistory = new HistoryEmployee { idEmployee = employee.Id, DateInput = dateWithoutMilliseconds, idInput = 2 };
                    MyDataBase.HistoryEmployee.Add(EmployeeHistory);
                    MyDataBase.SaveChanges();
                    // Открытие соответствующего окна в зависимости от должности пользователя
                    if (user.idPost == 1)
                    {
                        AdminForm adminForm = new AdminForm();
                        adminForm.Show();
                        this.Close();
                    }
                    else
                    {
                        if (user.idPost == 2)
                        {
                            CreateOrderForm createOrderForm = new CreateOrderForm(user.idPost);
                            createOrderForm.Show();
                            this.Close();
                        }
                        else
                        {
                            CreateOrderForm createOrderForm = new CreateOrderForm(user.idPost);
                            createOrderForm.Show();
                            this.Close();
                        }
                    }
                }
                else
                {
                    CountEnter++;
                    // Показ предупреждения и генерация новой капчи после двух неудачных попыток входа
                    if (CountEnter >= 2)
                    {
                        GenerateCaptcha();
                        this.Height = 500;
                        Captcha.Visibility = Visibility.Visible;
                        if (CountEnter > 2 && (user != null || textCaptcha == InputCaptcha.Text))
                        {
                            GenerateCaptcha();
                            Enter.IsEnabled = false;
                            timer.Start();
                        }
                    }
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Ошибка", "Введены неверные данные. Пожалуйста повторите ввод", "", "", "Закрыть");
                    messageBoxWindow.ShowDialog();
                    var employee = MyDataBase.Employee.FirstOrDefault(j => j.Login == Login.Text);
                    if (employee != null)
                    {
                        DateTime now = DateTime.Now;
                        DateTime dateWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                        // Добавление записи о неудачной попытке входа в историю сотрудника
                        var EmployeeHistory = new HistoryEmployee { idEmployee = employee.Id, DateInput = dateWithoutMilliseconds, idInput = 1 };
                        MyDataBase.HistoryEmployee.Add(EmployeeHistory);
                        MyDataBase.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow($"Авторизация", ex.Message, "", "", "Закрыть");
                messageBoxWindow.ShowDialog();
            }
        }
        public string textCaptcha = "";
        private void UpdateCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }
        private void GenerateCaptcha() // Метод генерирующий капчу
        {
            CanvasCaptcha.Children.Clear();
            textCaptcha = "";
            for (int i = 0; i < 5; i++)
            {
                TextBlock textBlock = new TextBlock // Происходит генерация букв и цифр для отображения
                {
                    Text = GetRandomChar().ToString(),
                    FontSize = 24,
                    Foreground = new SolidColorBrush(GetRandomColor()),
                    RenderTransform = new RotateTransform(random.Next(-30, 30))
                };
                textCaptcha += textBlock.Text;
                Canvas.SetLeft(textBlock, i * 30 + 10);
                Canvas.SetTop(textBlock, random.Next(0, 20));
                CanvasCaptcha.Children.Add(textBlock);
            }
            for (int i = 0; i < 15; i++)
            {
                Line line = new Line // Генерация линий - помех
                {
                    X1 = random.NextDouble() * CanvasCaptcha.ActualWidth,
                    Y1 = random.NextDouble() * CanvasCaptcha.ActualHeight,
                    X2 = random.NextDouble() * CanvasCaptcha.ActualWidth,
                    Y2 = random.NextDouble() * CanvasCaptcha.ActualHeight,
                    Stroke = new SolidColorBrush(GetRandomColor()),
                    StrokeThickness = 2
                };
                CanvasCaptcha.Children.Add(line);
                Ellipse ellipse = new Ellipse // Генерация кружков - помех
                {
                    Width = random.Next(10, 10),
                    Height = random.Next(10, 10),
                    Fill = new SolidColorBrush(GetRandomColor())
                };
                Canvas.SetLeft(ellipse, random.NextDouble() * CanvasCaptcha.ActualWidth);
                Canvas.SetTop(ellipse, random.NextDouble() * CanvasCaptcha.ActualHeight);
                CanvasCaptcha.Children.Add(ellipse);
            }
        }
        private char GetRandomChar() // Метод для генерация рандомных букв и цифр
        {
            int num = random.Next(0, 36);
            if (num < 26)
                return (char)('A' + num);
            else
                return (char)('0' + num - 26);
        }
        private System.Windows.Media.Color GetRandomColor() // Метод генерирующий рандомный цвет для всех элементов капчи
        {
            return System.Windows.Media.Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }
        public void isEye()
        {
            if (Password.Visibility == Visibility.Visible) // Присвоение текста в PasswordBox или TextBox в зависимости куда вводили пароль
            {
                PasswordText.Text = Password.Password;
            }
            else
            {
                Password.Password = PasswordText.Text;
            }
        }
    }
}
