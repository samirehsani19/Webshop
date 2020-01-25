using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Butik
{
    class FileManager
    {   
        public static string ProductPath { get => productPath; }
        public static string ProductTempPath { get => productTempPath; }
        public static string CartPath { get => cartPath; }
        public static string ImgTempPath { get => imgTempPath; }
        public static List<string> ErrorMessage { get => errorMessage; }

        private static string productPath = @"Products.csv";
        private static string productTempPath = @"C:\Windows\Temp\Products.csv";
        private static string voucherPath = @"Vouchers.csv";
        private static string cartPath = @"C:\Windows\Temp\Cart.csv";
        private static string imgPath = @"ProductImages";
        private static string imgTempPath = @"C:\Windows\Temp\ProductImages\";
        private static string[] readProducts;
        private static List<string> invalidProducts = new List<string>();
        private static string[] readVouchers;
        private static List<string> invalidVouchers = new List<string>();
        private static List<string> errorMessage = new List<string>();

        public static void InitializeFiles()
        {            
            //Copy products.csv to Windows/Temp
            CopyCSVToTemp(productTempPath);
            //Copy ProductImages from solution to Windows/Temp
            CopyFolder(imgPath, imgTempPath);

            readProducts = ReadFromCSV(productTempPath);
            invalidProducts = DetectCSVErrors(readProducts);
            if (invalidProducts.Count != 0)
                WriteErrorLog(invalidProducts, "Product");

            readVouchers = ReadFromCSV(voucherPath);
            invalidVouchers = DetectCSVErrors(readVouchers);
            if (invalidVouchers.Count != 0)
                WriteErrorLog(invalidVouchers, "Voucher");            
        }

        private static void CopyCSVToTemp(string path)
        {
            if (!File.Exists(path))
            {
                File.Copy(productPath, productTempPath);
            }            
        }

        public static string[] ReadFromCSV(string path)
        {   
            string[] tmp = File.ReadAllLines(path, System.Text.Encoding.Default);
            return tmp;
        }

        public static void WriteToCSV(string path, string item)
        {            
                File.AppendAllText(path, item + Environment.NewLine);
        }
                    
        private static List<string> DetectCSVErrors(string[] readCSVArray)
        {
            List<string> invalid = new List<string>();         
            
            for (int i = 1; i < readCSVArray.Length; i++)
            {
                string[] checkLines = readCSVArray[i].Split(';');
                //Products.csv
                if (checkLines.Length >= 5)
                {
                    if (checkLines[0] == "" || checkLines[1] == "")
                    {
                        invalid.Add(readCSVArray[i]);
                    }                    
                }
                //Vouchers.csv
                else
                {                    

                    if (checkLines[0] == "" || checkLines[2] == "")
                    {
                        invalid.Add(readCSVArray[i]);
                    }
                    
                }

                try
                {
                    int checkFormat = int.Parse(checkLines[3]);
                }
                catch (FormatException)
                {
                    invalid.Add(readCSVArray[i]);
                }                
            }

            return invalid;
        }

        private static void WriteErrorLog(List<string> source, string fName)
        {
            string fileName = $"{fName}ErrorLog{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            string errorPath = @"C:\Windows\Temp\" + fileName;
            File.WriteAllLines(errorPath, source);
            errorMessage.Add(errorPath);
        }

        public static string[] ShowCuredProducts()
        {
            var curated = readProducts.Where((c, i) => !invalidProducts.Contains(c) && i > 0).Select(c => c);
            return curated.ToArray();
        }

        public static string[] ShowCuredVouchers()
        {
            var curated = readVouchers.Where((c, i) => !invalidVouchers.Contains(c) && i > 0).Select(c => c);
            return curated.ToArray();
        }

        private static void CopyFolder(string copyFromFolder, string copyToFolder)
        {
            DirectoryInfo fromDir = new DirectoryInfo(copyFromFolder);
            if (!Directory.Exists(copyToFolder))
            {
                Directory.CreateDirectory(copyToFolder);                
            }

            FileInfo[] files = fromDir.GetFiles();
            foreach (FileInfo file in files)
            {               
                string tmp = Path.Combine(copyToFolder, file.Name);
                if (!File.Exists(tmp))
                {
                    file.CopyTo(tmp, false);
                }
            }
        }
    }    
}
