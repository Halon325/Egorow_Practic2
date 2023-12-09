using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddClient.xaml
    /// </summary>
    public partial class AddClient : Window
    {
        Egorow_Dem23Entities1 MyDataBase = new Egorow_Dem23Entities1();
        public AddClient()
        {
            InitializeComponent();
            TitleCity();
            TitleStreet();
            CodeClient.Text = MyDataBase.Client.Max(z => z.Id + 1).ToString();
            DateOfBirthClient.DisplayDateEnd = DateTime.Now;
        }
        // Объявление кнопки и текстового поля для добавления города
        public Button addCity;
        public TextBox titleCity;
        public StackPanel CityTitle;
        public object lastSelectedItemCity = null;
        // Создание текстового поля и кнопки для добавления города в базу данных
        public void TitleCity()
        {
            CityClient.Items.Clear();
            CityTitle = new StackPanel();
            CityTitle.Orientation = Orientation.Horizontal;
            addCity = new Button
            {
                Background = Brushes.Green,
                Foreground = new SolidColorBrush(Colors.Black),
                Content = "➕",
                HorizontalAlignment = HorizontalAlignment.Left,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1, 1, 1, 1),
            };
            addCity.Click += AddCity_Click;
            CityTitle.Children.Add(addCity);
            titleCity = new TextBox
            {
                Width = 200,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0, 0, 0, 1),
            };
            CityTitle.Children.Add(titleCity);
            CityClient.Items.Add(CityTitle);
            List<City> cityList = new List<City>(MyDataBase.City);
            for (int i = 0; i < cityList.Count; i++)
            {
                CityClient.Items.Add(cityList[i].Title);
            }
        }
        // Обработчик события для кнопки добавления города
        private void AddCity_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, если город уже существует или поле для ввода пустое
            if (MyDataBase.City.Any(z => z.Title == "г. " + titleCity.Text) && titleCity.Text.Length != 0)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Добавление города",
                    "Данный город уже существует, либо поля для ввода является пустым.", "", "", "Закрыть");
                messageBoxWindow.ShowDialog();
            }
            else
            {
                // Создание нового города
                City city = new City();
                city.Title = "г. " + titleCity.Text;
                MyDataBase.City.Add(city);
                MyDataBase.SaveChanges();
                // Обновление списка городов
                MyDataBase = new Egorow_Dem23Entities1();
                CityClient.Items.Clear();
                titleCity.Text = "";
                TitleCity();
            }
        }
        // Объявление кнопки и текстового поля для добавления улицы
        public Button addStreet;
        public TextBox titleStreet;
        public StackPanel StreetTitle;
        public object lastSelectedItemStreet = null;

        // Создание текстового поля и кнопки для добавления улицы в базу данных
        public void TitleStreet()
        {
            StreetClient.Items.Clear();
            StreetTitle = new StackPanel();
            StreetTitle.Orientation = Orientation.Horizontal;
            addStreet = new Button
            {
                Background = Brushes.Green,
                Foreground = new SolidColorBrush(Colors.Black),
                Content = "➕",
                HorizontalAlignment = HorizontalAlignment.Left,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1, 1, 1, 1),
            };
            addStreet.Click += AddStreet_Click;
            StreetTitle.Children.Add(addStreet);
            titleStreet = new TextBox
            {
                Width = 200,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0, 0, 0, 1),
            };
            StreetTitle.Children.Add(titleStreet);
            StreetClient.Items.Add(StreetTitle);
            List<Street> streetList = new List<Street>(MyDataBase.Street);
            for (int i = 0; i < streetList.Count; i++)
            {
                StreetClient.Items.Add(streetList[i].Street1);
            }
        }
        private void RollUp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Сворачивание окна
        }
        // Обработчик события для кнопки добавления улицы
        private void AddStreet_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, если улица уже существует или поле для ввода пустое
            if (MyDataBase.Street.Any(z => z.Street1 == "ул. " + titleStreet.Text) && titleStreet.Text.Length != 0)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Добавление улицы",
                    "Данная улица уже существует, либо поля для ввода является пустым.", "", "", "Закрыть");
                messageBoxWindow.ShowDialog();
            }
            else
            {
                // Создание новой улицы
                Street street = new Street();
                street.Street1 = "ул. " + titleStreet.Text;
                MyDataBase.Street.Add(street);
                MyDataBase.SaveChanges();
                // Обновление списка улиц
                MyDataBase = new Egorow_Dem23Entities1();
                StreetClient.Items.Clear();
                titleStreet.Text = "";
                TitleStreet();
            }
        }
        // Обработчик события для кнопки закрытия окна
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Обновление заказов
            OrderUpdater.UpdateOrders();
            // Показывает окно подтверждения перед закрытием окна
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Подтверждение", "Вы действительно хотите закрыть окно?", "Да", "Нет", "");
            messageBoxWindow.ShowDialog();
            // Если пользователь подтвердил закрытие окна
            if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes)
            {
                this.Close();
            }
        }
        // Обработчик события для изменения расположения окна
        private void DragAndDrop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove(); // Изменение расположения окна
        }


        // Обработчик события для кнопки сохранения нового клиента
        private void SaveAddClient_Click(object sender, RoutedEventArgs e)
        {
            // Показывает окно подтверждения перед добавлением нового клиента
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Подтверждение", "Вы действительно хотите добавить нового клиента?", "Да", "Нет", "");
            messageBoxWindow.ShowDialog();
            // Если пользователь подтвердил добавление нового клиента
            if (messageBoxWindow.DialogResult == MessageBoxWindow.Result.Yes)
            {
                try
                {
                    // Проверка на корректность введенных данных
                    Regex correctEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                    if (CodeClient.Text.Length == 8 && SurnameClient.Text.Length > 0 &&
                        NameClient.Text.Length > 0 && ForenameClient.Text.Length > 0 &&
                        SeriesClient.Text.Length == 4 && NumberClient.Text.Length == 6 &&
                        DateOfBirthClient.Text.Length > 0 && IndexClient.Text.Length > 0 &&
                        CityClient.Text.Length > 0 && StreetClient.Text.Length > 0 &&
                        HomeClient.Text.Length > 0 && FlatClient.Text.Length > 0 &&
                        EmailClient.Text.Length > 0 && correctEmail.IsMatch(EmailClient.Text))
                    {
                        // Проверка на существование клиента с такими же данными в базе данных
                        int codeClient = Convert.ToInt32(CodeClient.Text);
                        int seriesPassport = Convert.ToInt32(SeriesClient.Text);
                        int numberPassport = Convert.ToInt32(NumberClient.Text);
                        if (!MyDataBase.Client.Any(z => z.Email == EmailClient.Text
                        || (z.SeriesPassport == seriesPassport
                        && z.NumberPassport == numberPassport)
                        || z.Id == codeClient))
                        {
                            // Создание нового клиента
                            var newPassword = MyDataBase.Client.Max(p => p.Password);
                            int numberPassword = Convert.ToInt32(newPassword.Substring(2));
                            numberPassword++;
                            var client = new Client
                            {
                                Id = Convert.ToInt32(CodeClient.Text),
                                Surname = SurnameClient.Text,
                                Name = NameClient.Text,
                                Forename = ForenameClient.Text,
                                SeriesPassport = seriesPassport,
                                NumberPassport = numberPassport,
                                DateOfBirth = Convert.ToDateTime(DateOfBirthClient.Text),
                                Index = Convert.ToInt32(IndexClient.Text),
                                idCity = MyDataBase.City.Where(z => z.Title == CityClient.Text).First().Id,
                                IdStreet = MyDataBase.Street.Where(z => z.Street1 == StreetClient.Text).First().Id,
                                Home = HomeClient.Text,
                                Flat = FlatClient.Text,
                                Email = EmailClient.Text,
                                Password = "cl" + numberPassword.ToString(),
                            };
                            // Добавление нового клиента в базу данных
                            MyDataBase.Client.Add(client);
                            MyDataBase.SaveChanges();
                            // Закрытие окна
                            this.Close();
                        }
                        else
                        {
                            // Вывод сообщения об ошибке при существовании клиента с такими же данными
                            messageBoxWindow = new MessageBoxWindow("Добавление клиента",
                                "Ошибка, введенные данные уже имеются в базе данных " +
                                "и не могут совпадать.(Email, код клиента, серия и номер паспорта)\nПожалуйста, повторите ввод.", "", "", "Закрыть");
                            messageBoxWindow.ShowDialog();
                        }
                    }
                    else
                    {
                        // Вывод сообщения об ошибке при некорректном вводе данных
                        messageBoxWindow = new MessageBoxWindow("Добавление клиента", "Неккоректный ввод данных Email, либо были заполнены не все поля." +
                            "\nПожалуйста повторите ввод.", "", "", "Закрыть");
                        messageBoxWindow.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    // Вывод сообщения об ошибке
                    messageBoxWindow = new MessageBoxWindow("Добавление клиента", ex.Message, "", "", "Закрыть");
                    messageBoxWindow.ShowDialog();
                }
            }
        }

        // Обработчик события для изменения выбранной улицы
        private void StreetClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка, если выбранный элемент является StackPanel
            if (StreetClient.SelectedItem is StackPanel)
                StreetClient.SelectedItem = lastSelectedItemStreet;
            else
                lastSelectedItemStreet = StreetClient.SelectedItem;
        }
        // Обработчик события для изменения выбранного города
        private void CityClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка, если выбранный элемент является StackPanel
            if (CityClient.SelectedItem is StackPanel)
                CityClient.SelectedItem = lastSelectedItemCity;
            else
                lastSelectedItemCity = CityClient.SelectedItem;
        }
        // Обработчик события для ввода текста в поле серии
        private void SeriesClient_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка, если введенный символ не является цифрой
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
        // Обработчик события для изменения текста в поле серии
        private void SeriesClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Оставляем только цифры в тексте
            SeriesClient.Text = new string(SeriesClient.Text.Where(char.IsDigit).ToArray());
        }
        // Обработчик события для изменения текста в поле кода
        private void CodeClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Оставляем только цифры в тексте
            CodeClient.Text = new string(CodeClient.Text.Where(char.IsDigit).ToArray());
        }
        // Обработчик события для изменения текста в поле номера
        private void NumberClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Оставляем только цифры в тексте
            NumberClient.Text = new string(NumberClient.Text.Where(char.IsDigit).ToArray());
        }
        // Обработчик события для изменения текста в поле индекса
        private void IndexClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Оставляем только цифры в тексте
            IndexClient.Text = new string(IndexClient.Text.Where(char.IsDigit).ToArray());
        }
    }
}

