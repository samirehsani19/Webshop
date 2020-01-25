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
using System.IO;
using Microsoft.Win32;

namespace Butik
{
    class Staff
    {
        public StackPanel panel;
        private Image prodImage = new Image();
        private Label imageName;
        private TextBox NameBox;
        private TextBox PriceBox;
        private TextBox DescriptionBox;
        private CheckBox Specialproduct;
        OpenFileDialog file = new OpenFileDialog();
        private string copyImgTo = "";
        private string copyImgFrom = "";
        bool canProceed = false;
        public void AddProducs()
        {
            panel = new StackPanel();
            
            Label addingPr = new Label
            {
                Content = "Please add a product",
                FontSize=18,
            };
            panel.Children.Add(addingPr);

            StackPanel addImagePanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            panel.Children.Add(addImagePanel);

            prodImage.Height = 100;
            prodImage.Width = 100;
            prodImage.Source = new BitmapImage(new Uri(@"VarImages\noImage_uploaded.PNG", UriKind.Relative));
            addImagePanel.Children.Add(prodImage);

            imageName = new Label
            {
                Content = "No image selected.",
                FontSize = 18,
                Margin = new Thickness(25,0,0,0),
                VerticalAlignment = VerticalAlignment.Center
            };
            addImagePanel.Children.Add(imageName);

            Button pImageBtn = new Button
            {
                Content="Upload image",
                HorizontalAlignment= HorizontalAlignment.Left,
                Background= Brushes.SkyBlue,
                Height=30,
            };
            panel.Children.Add(pImageBtn);
            pImageBtn.Click += PImage_Click;


            Label productName = new Label
            {
                Content = "Product name",
                FontSize=18,
            };
            panel.Children.Add(productName);

            NameBox = new TextBox
            {
                Width = 220,
                Height = 30,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left

            };
            panel.Children.Add(NameBox);


            Label productPrice = new Label
            {
                Content = "Product price",
                FontSize = 18,
            };
            panel.Children.Add(productPrice);

            PriceBox = new TextBox
            {
                Width = 220,
                Height = 30,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left

            };
            panel.Children.Add(PriceBox);
            PriceBox.PreviewTextInput += PriceBox_PreviewTextInput;



            Label description = new Label
            {
                Content = "Product description",
                FontSize = 18,
            };
            panel.Children.Add(description);


            DescriptionBox = new TextBox
            {
                Width = 433,
                Height = 50,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left

            };
            panel.Children.Add(DescriptionBox);

            Label specialP = new Label
            {
                Content="Special",
                FontSize=18,
            };
            panel.Children.Add(specialP);

            Specialproduct = new CheckBox();
            panel.Children.Add(Specialproduct);

            Button AddProduct = new Button
            {
                Content = "Add",
                Width = 80,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.SkyBlue,

            };
            panel.Children.Add(AddProduct);
            AddProduct.Click += AddProduct_Click;

        }
        private void PImage_Click(object sender, RoutedEventArgs e)
        {
            file.RestoreDirectory = true;
            file.InitialDirectory = @"C:\Windows\Temp";

            if (file.ShowDialog()==true)
            {
                prodImage.Source = new BitmapImage(new Uri(file.FileName));
                imageName.Content = Path.GetFileName(file.FileName);

                copyImgFrom = file.FileName;
                copyImgTo = Path.Combine(FileManager.ImgTempPath, Path.GetFileName(file.FileName));
                                              
            }

        }

        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(c => !char.IsDigit(c))) { e.Handled = true; }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            
            if(canProceed == false)
            {
                try
                {
                    if(NameBox.Text == "" || PriceBox.Text == "")
                    {
                        throw new NullReferenceException();
                    }
                    else if (NameBox.Text != "" ||  PriceBox.Text != "")
                    {
                        File.Copy(file.FileName, copyImgTo);
                        string WriteProductInfo = imageName.Content + ";" + NameBox.Text + ";" + DescriptionBox.Text + ";" + PriceBox.Text + ";" + Specialproduct.IsChecked;                                            
                        FileManager.WriteToCSV(FileManager.ProductTempPath, WriteProductInfo);
                        
                        canProceed = true;
                    }                    
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("You forgot to provide product's details");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("You forgot to upload a image");
                }
                catch (IOException)
                {
                    MessageBox.Show("A file with that name already exists in C:/Windows/Temp/ProductImages/.");
                }
            }

            if(canProceed == true)
            {
                panel.Children.Clear();

                Label addedProduct = new Label
                {
                    Content = "The following product added:",
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                panel.Children.Add(addedProduct);

                Label addedPrd = new Label
                {
                    Width = 433,
                    Height = 200,
                    FontSize = 16,
                    Margin = new Thickness(0, 15, 0, 15),
                    Background = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                panel.Children.Add(addedPrd);

                string[] array = File.ReadAllLines(FileManager.ProductTempPath);
                foreach (var item in array)
                {
                    addedPrd.Content = item;
                }

                Button addMorepr = new Button
                {
                    Content = "Add more products",
                    Width = 120,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = Brushes.SkyBlue,

                };
                panel.Children.Add(addMorepr);
                addMorepr.Click += AddMorepr_Click;

                Button exit = new Button
                {
                    Content = "Exit the program",
                    Width = 120,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = Brushes.SkyBlue,

                };
                panel.Children.Add(exit);
                exit.Click += Exit_Click;
            }
                          
        }
                                        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddMorepr_Click(object sender, RoutedEventArgs e)
        {
            panel.Children.Clear();

            AddMoreProduct a = new AddMoreProduct();
            a.AddP();
            panel.Children.Add(a.p);
        }
    }

    public class AddMoreProduct 
    {
        public StackPanel p;
        public void AddP()
        {
            p = new StackPanel();
            Staff s = new Staff();
            s.AddProducs();
            p.Children.Add(s.panel);
            
        }   
    }
}

