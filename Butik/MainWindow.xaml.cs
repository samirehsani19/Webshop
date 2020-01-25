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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace Butik
{
    public partial class MainWindow : Window
    {
        public Grid grid;
        public StackPanel panel;
        private TextBox Username;
        private TextBox Password;

        public void Start()
        {
            grid = (Grid)Content;
            MinHeight = 600;
            MinWidth = 850;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = Brushes.Orange;

            panel = new StackPanel();
            grid.Children.Add(panel);            

            Label startUp = new Label
            {
                Content = "Please choose an option",
                FontSize = 18,
                Background = Brushes.LightGreen
            };
            panel.Children.Add(startUp);

            Button customer = new Button
            {
                Content = "Customer",
                FontSize = 18
            };
            panel.Children.Add(customer);
            customer.Click += Costumer_Click;

            Button staff = new Button
            {
                Content = "Staff",
                FontSize = 18
            };
            panel.Children.Add(staff);
            staff.Click += Staff_Click;
        }

        public void UserInfo()
        {
            StackPanel panel = new StackPanel();
            grid.Children.Add(panel);

            Label userName = new Label
            {
                Content = "Enter your username",
                FontSize = 18,

            };
            panel.Children.Add(userName);

            Username = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 220,
                Height = 30,
                FontSize = 18,

            };
            panel.Children.Add(Username);

            Label password = new Label
            {
                Content = "Enter your password",
                FontSize = 18,

            };
            panel.Children.Add(password);

            Password = new TextBox
            {
                Width = 220,
                Height = 30,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,

            };
            panel.Children.Add(Password);


            Button Login = new Button
            {
                Content = "Login",
                FontSize = 18,
                Width = 200,
                Background = Brushes.SkyBlue,
                HorizontalAlignment = HorizontalAlignment.Left,

            };
            panel.Children.Add(Login);
            Login.Click += Login_Click;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "brad" && Password.Text == "pitt")
            {
                grid.Children.Clear();

                Staff staff = new Staff();
                staff.AddProducs();
                grid.Children.Add(staff.panel);
            }
            else
            {
                MessageBox.Show("Wrong username or password!!");
            }
        }

        private void Staff_Click(object sender, RoutedEventArgs e)
        {
            grid.Children.Clear();
            UserInfo();
        }

        private void Costumer_Click(object sender, RoutedEventArgs e)
        {
            grid.Children.Clear();

            Customer customer = new Customer();
            customer.CustomerView();
            grid.Children.Add(customer.mainGrid);
        }

        public MainWindow()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            InitializeComponent();
            FileManager.InitializeFiles();
            if(FileManager.ErrorMessage.Count != 0)
            {
                foreach (string em in FileManager.ErrorMessage)
                    MessageBox.Show("There are invalid items that will not be displayed in the shop.\r\nSee logfile for more info:\r\n\r\n" + em);
            }
            
            Start();
        }
    }
}
