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
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace Butik
{
    interface IProduct
    {
        void InitializeGrid();
        Grid ReturnGrid();
        string ProductName { get; }
        Button Btn { get; }
        int Price { get; }
        int Amount { get; set; }
    }
    class RegularProduct : IProduct
    {
        public Grid Grd = new Grid();
        public string ProductName { get; private set; }
        public int Price { get; private set; }
        public Button Btn { get; private set; }
        public int Amount { get; set; }

        private Image productImage;
        private string imgName;
        private string imgPath = FileManager.ImgTempPath;
        private Label productTitle = new Label();
        private TextBlock description = new TextBlock();
        private Label priceText = new Label();


        public RegularProduct(string img, string title, string desc, string price)
        {
            imgName = img;
            productTitle.Content = title;
            ProductName = title;
            description.Text = desc;
            priceText.Content = price + " SEK";
            Price = int.Parse(price);
        }

        public Grid ReturnGrid()
        {
            return Grd;
        }

        public void InitializeGrid()
        {
            Grd.ShowGridLines = false;
            Grd.Margin = new Thickness(0, 5, 0, 5);
            Grd.Background = Brushes.White;
            Grd.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grd.ColumnDefinitions.Add(new ColumnDefinition());
            Grd.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            Grd.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            DefineLayout();
        }

        private void DefineLayout()
        {
            int commonFontSize1 = 18;
            int commonFontSize2 = 14;

            productImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill,
                Height = 80,
                Width = 80
            };
            Grid.SetColumn(productImage, 0);
            Grd.Children.Add(productImage);

            //set source of productImage
            DirectoryInfo dir = new DirectoryInfo(imgPath);
            foreach (FileInfo fi in dir.GetFiles())
            {
                if (fi.FullName.Contains(imgName))
                    productImage.Source = new BitmapImage(new Uri(fi.FullName, UriKind.Absolute));
            }
         
            StackPanel pTextContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 10, 0, 10)
            };
            Grid.SetColumn(pTextContainer, 1);
            Grd.Children.Add(pTextContainer);

            productTitle.FontSize = commonFontSize1;
            productTitle.FontWeight = FontWeights.Bold;
            productTitle.Padding = new Thickness(2);
            pTextContainer.Children.Add(productTitle);

            description.FontSize = commonFontSize2;
            description.FontWeight = FontWeights.SemiBold;
            productTitle.Padding = new Thickness(2);
            pTextContainer.Children.Add(description);

            priceText.FontSize = commonFontSize1;
            priceText.FontWeight = FontWeights.Bold;
            priceText.Margin = new Thickness(0, 0, 5, 0);
            priceText.VerticalAlignment = VerticalAlignment.Center;
            priceText.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(priceText, 2);
            Grd.Children.Add(priceText);

            Btn = new Button
            {
                Tag = ProductName,
                Height = 40,
                Width = 120,
                Margin = new Thickness(0, 0, 10, 0),
                Content = "Add to cart",
                Foreground = Brushes.White,
                Background = Brushes.Green,
                FontWeight = FontWeights.SemiBold,
                FontSize = commonFontSize2,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(Btn, 3);
            Grd.Children.Add(Btn);
        }
    }

    class FeaturedProduct : IProduct
    {
        public Grid Grd = new Grid();
        public string ProductName { get; private set; }
        public int Price { get; private set; }
        public Button Btn { get; private set; }
        public int Amount { get; set; }

        private Image productImage;
        private string imgName;
        private string imgPath = FileManager.ImgTempPath;
        private Label productTitle = new Label();
        private TextBlock description = new TextBlock();
        private Label priceText = new Label();

        public FeaturedProduct(string img, string title, string desc, string price)
        {
            imgName = img;
            productTitle.Content = title;
            ProductName = title;
            description.Text = desc;
            priceText.Content = price + " SEK";
            Price = int.Parse(price);
        }

        public Grid ReturnGrid()
        {
            return Grd;
        }

        public void InitializeGrid()
        {
            Grd.ShowGridLines = false;
            Grd.Margin = new Thickness(0, 0, 0, 20);
            Grd.Background = Brushes.Gray;
            Grd.ColumnDefinitions.Add(new ColumnDefinition());
            Grd.ColumnDefinitions.Add(new ColumnDefinition());
            Grd.RowDefinitions.Add(new RowDefinition());
            Grd.RowDefinitions.Add(new RowDefinition());
            Grd.RowDefinitions.Add(new RowDefinition());

            DefineLayout();
        }

        private void DefineLayout()
        {
            Brush commonTextColor = Brushes.AntiqueWhite;
            int commonFontSize1 = 18;
            int commonFontSize2 = 14;

            productImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill,
                Height = 180,
            };            
            Grid.SetColumnSpan(productImage, 2);
            Grid.SetRow(productImage, 0);
            Grd.Children.Add(productImage);

            //set source of productImage
            DirectoryInfo dir = new DirectoryInfo(imgPath);
            foreach(FileInfo fi in dir.GetFiles())
            {
                if (fi.FullName.Contains(imgName))
                    productImage.Source = new BitmapImage(new Uri(fi.FullName, UriKind.Absolute));
                string dirImgName = Path.GetFileName(fi.ToString());
            }
            
            StackPanel pTextContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };
            Grid.SetColumnSpan(pTextContainer, 2);
            Grid.SetRow(pTextContainer, 1);
            Grd.Children.Add(pTextContainer);

            productTitle.FontSize = commonFontSize1;
            productTitle.Foreground = commonTextColor;
            productTitle.FontWeight = FontWeights.Bold;
            productTitle.Padding = new Thickness(2);
            pTextContainer.Children.Add(productTitle);

            description.FontSize = commonFontSize2;
            description.Foreground = commonTextColor;
            description.FontWeight = FontWeights.SemiBold;
            productTitle.Padding = new Thickness(2);
            pTextContainer.Children.Add(description);

            priceText.FontSize = commonFontSize1;
            priceText.Foreground = commonTextColor;
            priceText.FontWeight = FontWeights.Bold;
            priceText.Margin = new Thickness(10, 0, 0, 15);
            priceText.VerticalAlignment = VerticalAlignment.Center;
            priceText.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetColumn(priceText, 0);
            Grid.SetRow(priceText, 2);
            Grd.Children.Add(priceText);

            Btn = new Button
            {
                Tag = ProductName,
                Height = 40,
                Width = 120,
                Margin = new Thickness(0, 0, 10, 15),
                Content = "Add to cart",
                Foreground = commonTextColor,
                Background = Brushes.Green,
                FontWeight = FontWeights.SemiBold,
                FontSize = commonFontSize2,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(Btn, 1);
            Grid.SetRow(Btn, 2);
            Grd.Children.Add(Btn);
        }
    }
    public class Customer
    {
        public Grid mainGrid;
        public Grid customerGrid;
        private Grid featuredGrid;
        private Grid regularGrid;

        Button shoppingCart;
        Button uploadCart;
        int cartAmnt = 0;

        Cart cart = new Cart();
        List<IProduct> iProduct = new List<IProduct>();

        public void CustomerView()
        {
            CustomerLayout();

            Image logoImg = new Image()
            {
                Source = new BitmapImage(new Uri(@"VarImages\samic_logo.png", UriKind.Relative)),
                Margin = new Thickness(0, 0, 0, 40)
            };
            Grid.SetRow(logoImg, 0);
            customerGrid.Children.Add(logoImg);            

            Label contactInfo = new Label
            {
                Content = "CONTACT:\r\nAdress: Kungsgatan 1 \r\n415 42 Gothenburg \r\nPhone: 073 445 45 45",
                FontSize = 14,
                Margin = new Thickness(0, 20, 0, 20),
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(contactInfo, 4);
            customerGrid.Children.Add(contactInfo);

            StackPanel topRight = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            Grid.SetColumn(topRight, 2);
            Grid.SetRow(topRight, 0);
            customerGrid.Children.Add(topRight);

            uploadCart = new Button
            {
                Content = $"Upload Cart",
                FontSize = 15,
                FontWeight = FontWeights.DemiBold,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = Brushes.DodgerBlue,
            };
            uploadCart.Click += UploadCart_Click;
            topRight.Children.Add(uploadCart);

            shoppingCart = new Button
            {
                Content = $"Shopping Cart: {cartAmnt}",
                FontSize = 15,
                FontWeight = FontWeights.DemiBold,
                Padding = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = Brushes.Gold,
            };
            shoppingCart.Click += ClickCart;
            topRight.Children.Add(shoppingCart);
            UpdateCartAmount();

            featuredGrid = new Grid();
            Grid.SetRow(featuredGrid, 2);
            Grid.SetColumnSpan(featuredGrid, 3);
            customerGrid.Children.Add(featuredGrid);

            regularGrid = new Grid();
            Grid.SetRow(regularGrid, 3);
            Grid.SetColumnSpan(regularGrid, 3);
            customerGrid.Children.Add(regularGrid);

            //instantiate all products and put into List<IProduct>iProduct
            string[] showProducts = FileManager.ShowCuredProducts();
            for (int i = 0; i < showProducts.Length; i++)
            {
                string[] tmp = showProducts[i].Split(';');
                if (tmp[4] != "True")
                    iProduct.Add(new RegularProduct(tmp[0], tmp[1], tmp[2], tmp[3]));
                else
                    iProduct.Add(new FeaturedProduct(tmp[0], tmp[1], tmp[2], tmp[3]));
            }

            //display all products
            DisplayProducts(true);
        }

        private void UploadCart_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            string path = @"C:\Windows\Temp";
            file.RestoreDirectory = true;
            file.InitialDirectory = path;

            Cart.ProductCollection.Clear();

            if (file.ShowDialog() == true)
            {
                string[] shoppingData = File.ReadAllLines(file.FileName);

                for (int i = 0; i < shoppingData.Length; i++)
                {
                    string[] temp = shoppingData[i].Split(';');
                    Cart.ProductCollection.Add(temp[0], int.Parse(temp[1]));
                }

                UpdateCartAmount();
            }           
        }

        private void ClickCart(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Clear();
            mainGrid.ColumnDefinitions.Clear();
            mainGrid.RowDefinitions.Clear();           

            cart.CustomerReceipt();
            mainGrid.Children.Add(cart.scroll);
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            foreach (IProduct prod in iProduct)
            {
                if (prod.ProductName == btn.Tag.ToString())
                {
                    if (Cart.ProductCollection.ContainsKey(prod.ProductName))
                        Cart.ProductCollection[prod.ProductName] += 1;
                    else
                        Cart.ProductCollection.Add(prod.ProductName, 1);
                }                
            }
            
            UpdateCartAmount();
        }

        private void UpdateCartAmount()
        {
            cartAmnt = 0;
            foreach (KeyValuePair<string, int> dict in Cart.ProductCollection)
                cartAmnt += dict.Value;
            shoppingCart.Content = $"Shopping Cart: {cartAmnt}";
        }
        
        private void CustomerLayout()
        {
            mainGrid = new Grid();
            mainGrid.Background = Brushes.LightGray;
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());

            customerGrid = new Grid();
            customerGrid.ShowGridLines = false;
            customerGrid.Margin = new Thickness(30);
            customerGrid.Background = Brushes.LightGray;
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            customerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            customerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            customerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            customerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            customerGrid.RowDefinitions.Add(new RowDefinition());

            ScrollViewer scrollViewer = new ScrollViewer { Content = customerGrid };
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            mainGrid.Children.Add(scrollViewer);
        }

        //Display featured products two different ways (random or not)     
        private void DisplayProducts(bool randomFeaturedProducts)
        {
            foreach (IProduct ip in iProduct)
            {
                ip.InitializeGrid();
                ip.Btn.Click += Buy_Click;
            }

            Random rndFeatured = new Random();
            var featuredP = iProduct.Where(f => f.GetType() == typeof(FeaturedProduct)).Select(f => f);

            if(featuredP.Count() > 3)
            {
                if (randomFeaturedProducts == true)
                {

                    List<IProduct> sortedFeaturedP = new List<IProduct>();
                    while (sortedFeaturedP.Count < 3)
                    {
                        var rndSingleFeatured = featuredP.ElementAtOrDefault(rndFeatured.Next(0, featuredP.Count()));
                        if (!sortedFeaturedP.Contains(rndSingleFeatured))
                            sortedFeaturedP.Add(rndSingleFeatured);
                    }

                    for (int i = 0; i < sortedFeaturedP.Count; i++)
                    {
                        featuredGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        Grid.SetColumn(sortedFeaturedP[i].ReturnGrid(), i);
                        featuredGrid.Children.Add(sortedFeaturedP[i].ReturnGrid());
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        featuredGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        Grid.SetColumn(featuredP.ElementAt(i).ReturnGrid(), i);
                        featuredGrid.Children.Add(featuredP.ElementAt(i).ReturnGrid());
                    }
                }
            }
            else
            {
                int colIndex = 0;
                foreach (IProduct featP in featuredP)
                {
                    featuredGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(featP.ReturnGrid(), colIndex);
                    featuredGrid.Children.Add(featP.ReturnGrid());
                    colIndex++;
                }
            }
            

            var regularP = iProduct.Where(f => f.GetType() == typeof(RegularProduct)).Select(f => f);
            int rowIndex = 0;
            foreach (IProduct regP in regularP)
            {
                regularGrid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(regP.ReturnGrid(), rowIndex);
                regularGrid.Children.Add(regP.ReturnGrid());
                rowIndex++;
            }
        }
    }
}
    


