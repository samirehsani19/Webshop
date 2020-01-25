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
using System.IO;
using Microsoft.Win32;

namespace Butik
{
    class CartItem : IProduct
    {
        public Grid Grd = new Grid();
        public string ProductName { get; private set; }
        public int Price { get; private set; }
        public Button Btn { get; private set; }
        public int Amount { get; set; }

        private Label productTitle = new Label();
        public Label priceText = new Label();
        public Label amountText = new Label();
        public int totalPrice;
        public int singlePrice;

        public CartItem(string title, int amount, int price)
        {
            productTitle.Content = title;
            ProductName = title;
            Amount = amount;
            amountText.Content = amount;
            singlePrice = price;
            totalPrice = price * amount;
            priceText.Content = totalPrice + " SEK";
            Price = price;
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
            Grd.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            Grd.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            DefineLayout();
        }

        public void DefineLayout()
        {
            productTitle.FontSize = 18;
            productTitle.FontWeight = FontWeights.Bold;
            productTitle.Padding = new Thickness(2);

            StackPanel pTextContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 10, 0, 10)
            };
            pTextContainer.Children.Add(productTitle);
            Grid.SetColumn(pTextContainer, 0);
            Grd.Children.Add(pTextContainer);

            amountText.FontSize = 18;
            amountText.FontWeight = FontWeights.Bold;
            amountText.Margin = new Thickness(0, 0, 5, 0);
            amountText.VerticalAlignment = VerticalAlignment.Center;
            amountText.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(amountText, 1);
            Grd.Children.Add(amountText);

            priceText.FontSize = 18;
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
                Content = "Remove",
                Foreground = Brushes.White,
                Background = Brushes.Red,
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(Btn, 3);
            Grd.Children.Add(Btn);
        }
    }
    class Cart
    {
        public ScrollViewer scroll;
        Grid cartGrid = new Grid();
        Grid itemGrid;
        private ComboBox combo;
        public static Dictionary<string, int> ProductCollection = new Dictionary<string, int>();
        public static List<IProduct> CartItems = new List<IProduct>();

        private Button pay;

        private Label totalPriceText;
        private Label discountedAmntText;
        private Label showDiscountText;
        private int commonFontSize = 18;
        private int discount = 0;
        private int totalPrice = 0;
        private double priceWithDiscount = 0;

        Receipt receipt = new Receipt();

        public void CustomerReceipt()
        {
            scroll = new ScrollViewer();
            scroll.Content = cartGrid;
            cartGrid.ShowGridLines = true;

            cartGrid.Background = Brushes.Pink;
            cartGrid.ShowGridLines = false;
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition());

            Label companyName = new Label
            {
                Content = "SaMic",
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.AliceBlue,
                FontSize = 35,
            };
            cartGrid.Children.Add(companyName);
            Grid.SetRow(companyName, 0);
            Grid.SetColumnSpan(companyName, 2);

            Label name = new Label
            {
                Content = "Your product:",
                FontSize = 25,
            };
            cartGrid.Children.Add(name);
            Grid.SetRow(name, 1);
            Grid.SetColumn(name, 0);

            itemGrid = new Grid();
            Grid.SetRow(itemGrid, 2);
            Grid.SetColumnSpan(itemGrid, 2);
            cartGrid.Children.Add(itemGrid);

            string[] tmpProd = FileManager.ShowCuredProducts();
            foreach (var item in ProductCollection)
            {
                for (int i = 0; i < tmpProd.Length; i++)
                {
                    string[] splitTmp = tmpProd[i].Split(';');

                    if (item.Key.Contains(splitTmp[1]))
                    {
                        int cost = int.Parse(splitTmp[3]);
                        CartItems.Add(new CartItem(item.Key, item.Value, cost));
                    }
                }
            }

            for (int i = 0; i < CartItems.Count; i++)
            {
                CartItems[i].InitializeGrid();
                CartItems[i].Btn.Click += RemoveItem_Click;
                itemGrid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(CartItems[i].ReturnGrid(), i);
                itemGrid.Children.Add(CartItems[i].ReturnGrid());
            }

            StackPanel discountPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Grid.SetRow(discountPanel, 3);
            Grid.SetColumn(discountPanel, 0);
            cartGrid.Children.Add(discountPanel);

            Label discountLabel = new Label
            {
                Content = "Select a discount code",
                FontSize = commonFontSize,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 10, 0, 3)
            };
            discountPanel.Children.Add(discountLabel);

            combo = new ComboBox
            {
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = commonFontSize,
                Margin = new Thickness(0, 0, 0, 10)
            };
            discountPanel.Children.Add(combo);
            combo.SelectionChanged += Combo_SelectionChanged;           

            StackPanel summaryPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Grid.SetRow(summaryPanel, 3);
            Grid.SetColumn(summaryPanel, 1);
            cartGrid.Children.Add(summaryPanel);

            foreach(CartItem c in CartItems)
            {
                totalPrice += c.singlePrice * c.Amount;
            }

            totalPriceText = new Label
            {
                Content = "Total Price:\t" + totalPrice + " SEK",
                FontSize = commonFontSize,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 10, 0, 0)
            };
            summaryPanel.Children.Add(totalPriceText);

            showDiscountText = new Label
            {
                Content = "Discount:\t",
                FontSize = commonFontSize,
                FontWeight = FontWeights.SemiBold
            };
            summaryPanel.Children.Add(showDiscountText);

            discountedAmntText = new Label
            {
                Content = "Subtotal:\t" + totalPrice + " SEK",
                FontSize = commonFontSize,
                FontWeight = FontWeights.SemiBold
            };
            summaryPanel.Children.Add(discountedAmntText);

            List<string> voucherCode = FileManager.ShowCuredVouchers().ToList();
            int index = 0;
            while (index < voucherCode.Count)
            {
                string[] info = voucherCode[index].Split(';');
                combo.Items.Add(info[0] + "\t" + info.Last() + "%");
                index++;
            }

            StackPanel botLeftPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            Grid.SetRow(botLeftPanel, 4);
            Grid.SetColumn(botLeftPanel, 0);
            cartGrid.Children.Add(botLeftPanel);

            Button keepShopping = new Button
            {
                Content = "<< Keep Shopping",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 20, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                MinWidth = 150,
                Height = 40,
                Foreground = Brushes.Black,
                Background = Brushes.Gold,
                FontSize = 15,
            };
            keepShopping.Click += KeepShopping_Click;
            botLeftPanel.Children.Add(keepShopping);

            Button saveCart = new Button
            {
                Content = "Download Cart",
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                MinWidth = 150,
                Height = 40,
                Foreground = Brushes.Black,
                Background = Brushes.DodgerBlue,
                FontSize = 15,
            };
            saveCart.Click += SaveCart_Click;         
            botLeftPanel.Children.Add(saveCart);

            pay = new Button
            {
                Content = "Pay",
                FontWeight = FontWeights.SemiBold,
                MinWidth = 150,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = Brushes.White,
                Background = Brushes.Green,
                FontSize = 15
            };
            pay.Click += Pay_Click;
            Grid.SetRow(pay, 4);
            Grid.SetColumn(pay, 1);
            cartGrid.Children.Add(pay);
        }

        private void KeepShopping_Click(object sender, RoutedEventArgs e)
        {
            CartItems.Clear();

            itemGrid.Children.Clear();
            itemGrid.ColumnDefinitions.Clear();
            itemGrid.RowDefinitions.Clear();

            cartGrid.Children.Clear();
            cartGrid.ColumnDefinitions.Clear();
            cartGrid.RowDefinitions.Clear();

            var itemsToRemove = ProductCollection.Where(f => f.Value == 0).ToArray();
            foreach (var item in itemsToRemove)
                ProductCollection.Remove(item.Key);

            Customer customer = new Customer();
            customer.CustomerView();
            cartGrid.Children.Add(customer.mainGrid);
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        private void SaveCart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.RestoreDirectory = true;
            file.InitialDirectory = @"C:\Windows\Temp";
            file.FileName = "Cart.csv";
            file.Filter = "Excel File | *.csv";

            string data = "";

            if (file.ShowDialog()==true)
            {

                var itemsToRemove = ProductCollection.Where(f => f.Value == 0).ToArray();
                foreach (var item in itemsToRemove)
                    ProductCollection.Remove(item.Key);

                foreach (var item in ProductCollection)
                    data += item.Key + ";" + item.Value + "\r\n";

                File.WriteAllText(file.FileName, data, System.Text.Encoding.Default);
            }
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {         
            string[] tmp = combo.SelectedItem.ToString().Split('\t');
            discount = int.Parse(tmp[1].TrimEnd('%'));
            showDiscountText.Content = "Discount:\t" + discount + " %";

            priceWithDiscount = (double)totalPrice - ((double)totalPrice * ((double)discount/100));
            discountedAmntText.Content = "With discount:\t" + priceWithDiscount + " SEK";
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            for (int i = 0; i < CartItems.Count; i++)
            {
                if (CartItems[i].ProductName == btn.Tag.ToString())
                {
                    CartItems[i].Amount--;

                    foreach (CartItem cI in CartItems)
                    {                                                                       
                        if (cI.ProductName == btn.Tag.ToString())
                        {
                            if (cI.Amount > 0)
                            {
                                cI.amountText.Content = CartItems[i].Amount;
                                cI.priceText.Content = (cI.singlePrice * CartItems[i].Amount) + " SEK";
                                totalPrice -= (cI.singlePrice);
                            }
                            else if(cI.Amount <=0)
                            {
                                totalPrice -= (cI.singlePrice);
                                itemGrid.Children.Remove(CartItems[i].ReturnGrid());
                            }
                        }
                    }
                }
            }

            if (ProductCollection.ContainsKey(btn.Tag.ToString()))
                ProductCollection[btn.Tag.ToString()]-= 1;

            totalPriceText.Content = "Total price:\t" + totalPrice + " SEK";
            priceWithDiscount = (double)totalPrice - ((double)totalPrice * ((double)discount / 100));
            discountedAmntText.Content = "Subtotal:\t" + priceWithDiscount + " SEK";
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            cartGrid.Children.Clear();
            cartGrid.RowDefinitions.Clear();
            cartGrid.ColumnDefinitions.Clear();
            receipt.GetDiscount = discount;
            receipt.GetTotalPrice = totalPrice;

            var tmp = CartItems.Where(c => c.Amount > 0);
            foreach(IProduct ip in tmp)
            {
                receipt.ReceiptProducts.Add(ip);
            }
            
            receipt.ShowReceipt();
            cartGrid.Children.Add(receipt.recieptGrid);
        }
    }


    class Receipt
    {
        public Grid recieptGrid = new Grid();
        public List<IProduct> ReceiptProducts = new List<IProduct>();
        public int GetTotalPrice { get; set; }
        public int GetDiscount { get; set; }
        
        public void ShowReceipt()
        {
            recieptGrid.ShowGridLines = false;
            recieptGrid.Background = Brushes.White;
            recieptGrid.Margin = new Thickness(30);

            for (int i = 0; i < 3; i++)
            {
                recieptGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int j = 0; j < 5; j++)
            {
                recieptGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            StackPanel headerInfo = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 25, 0, 25)
            };
            Grid.SetRow(headerInfo, 0);
            Grid.SetColumnSpan(headerInfo, 3);
            recieptGrid.Children.Add(headerInfo);

            Label company = CreateLabel("SaMic Driks", 25);
            company.HorizontalContentAlignment = HorizontalAlignment.Center;
            headerInfo.Children.Add(company);

            TextBlock adress = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Text = "Adress: Kungsgatan 1 \r\n415 42 Gothenburg \r\nPhone: 073 445 45 45\r\n" + DateTime.Now,
                TextAlignment = TextAlignment.Center,
            };
            headerInfo.Children.Add(adress);

            Random rnd = new Random();
            int recieptNr = rnd.Next(143, 236565);
            Label recieptText = CreateLabel("Reciept No", 20);
            recieptText.Content += " # " + recieptNr;
            recieptText.Foreground = Brushes.White;
            recieptText.Background = Brushes.Green;
            Grid.SetRow(recieptText, 1);
            Grid.SetColumnSpan(recieptText, 3);
            recieptGrid.Children.Add(recieptText);

            Grid productGrid = new Grid();
            productGrid.ShowGridLines = false;
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(productGrid, 2);
            Grid.SetColumnSpan(productGrid, 3);
            recieptGrid.Children.Add(productGrid);

            Label prodName = CreateLabel("Product", 15);
            Label prodAmount = CreateLabel("Amount", 15);
            Label prodPrice = CreateLabel("Price", 15);
            Grid.SetRow(prodName, 0);
            Grid.SetRow(prodAmount, 0);
            Grid.SetRow(prodPrice, 0);
            Grid.SetColumn(prodName, 0);
            Grid.SetColumn(prodAmount, 1);
            Grid.SetColumn(prodPrice, 2);
            productGrid.Children.Add(prodName);
            productGrid.Children.Add(prodAmount);
            productGrid.Children.Add(prodPrice);

            for (int i = 0; i < ReceiptProducts.Count; i++)
            {
                productGrid.RowDefinitions.Add(new RowDefinition());
                Label l1 = CreateLabel(ReceiptProducts[i].ProductName, 15);
                l1.Name = "nameLabel" + i;
                Label l2 = CreateLabel(ReceiptProducts[i].Amount.ToString(), 15);
                l2.Name = "amountLabel" + i;                
                Label l3 = CreateLabel((ReceiptProducts[i].Amount * ReceiptProducts[i].Price) + " SEK", 15);
                l3.Name = "costLabel" + i;
                Grid.SetRow(l1, i + 1);
                Grid.SetRow(l2, i + 1);
                Grid.SetRow(l3, i + 1);
                Grid.SetColumn(l1, 0);
                Grid.SetColumn(l2, 1);
                Grid.SetColumn(l3, 2);
                productGrid.Children.Add(l1);
                productGrid.Children.Add(l2);
                productGrid.Children.Add(l3);
            }

            StackPanel priceInfo = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(priceInfo, 3);
            Grid.SetColumn(priceInfo, 1);
            recieptGrid.Children.Add(priceInfo);

            Label totalPriceTxt = CreateLabel("Total price: ", 15);
            Label discountTxt = CreateLabel("Discount: ", 15);
            Label discountedAmntTxt = CreateLabel("With discount: ", 18);
            totalPriceTxt.FontWeight = FontWeights.Bold;
            discountTxt.FontWeight = FontWeights.Bold;
            discountedAmntTxt.FontWeight = FontWeights.UltraBlack;
            priceInfo.Children.Add(totalPriceTxt);
            priceInfo.Children.Add(discountTxt);
            priceInfo.Children.Add(discountedAmntTxt);

            StackPanel priceData = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(priceData, 3);
            Grid.SetColumn(priceData, 2);
            recieptGrid.Children.Add(priceData);

            double totalWithDiscount = GetTotalPrice - (GetTotalPrice * ((double)GetDiscount / 100));
            Label totPrice = CreateLabel(GetTotalPrice.ToString() + " SEK", 15);
            Label disc = CreateLabel(GetDiscount.ToString() + " %", 15);
            Label tPrice = CreateLabel(totalWithDiscount.ToString() + " SEK", 15);
            totPrice.FontWeight = FontWeights.Bold;
            disc.FontWeight = FontWeights.Black;
            disc.Foreground = Brushes.Green;
            tPrice.FontWeight = FontWeights.UltraBold;
            priceData.Children.Add(totPrice);
            priceData.Children.Add(disc);
            priceData.Children.Add(tPrice);

            Label welcomeAgain = CreateLabel("Happy Shopping, don't drink it all at once!", 15);
            welcomeAgain.HorizontalAlignment = HorizontalAlignment.Center;
            welcomeAgain.Margin = new Thickness(0, 25, 0, 0);
            Grid.SetRow(welcomeAgain, 4);
            Grid.SetColumnSpan(welcomeAgain, 3);
            recieptGrid.Children.Add(welcomeAgain);

        }

        private Label CreateLabel(string content, int fontSize)
        {
            Label tmp = new Label()
            {
                Content = content,
                FontSize = fontSize,
                FontWeight = FontWeights.DemiBold,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            return tmp;
        }
    }

}



