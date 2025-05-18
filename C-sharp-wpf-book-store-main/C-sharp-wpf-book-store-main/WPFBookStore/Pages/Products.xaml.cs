using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Path = System.IO.Path;
namespace WPFBookStore.Pages
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        public Products()
        {
            InitializeComponent();
            List<String> Category = new List<string>()
            {
                "---Select Category---",
                "Novel",
                "Poem",
                "Story Book",
                "Translated Book"
            };
            cmbCategory.ItemsSource = Category;
            cmbCategory.Text = "---Select Category---";

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                itemPic.Source = new BitmapImage(new Uri(ofd.FileName));
                var tmpFileName = Guid.NewGuid() + Path.GetExtension(ofd.FileName);
                txtFilePath.Text = tmpFileName;
                var imagePath = System.IO.Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"../..Assets\Images\") + tmpFileName);
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Models.Addbook ab = new Models.Addbook();
                ab.Id = Convert.ToInt32(txtId.Text);
                ab.BookName = txtBook.Text;
                ab.AuthorName = txtAuthor.Text;
                ab.Price = Convert.ToDecimal(txtPrice.Text);
                ab.Quantity = Convert.ToInt32(txtQuantity.Text);
                ab.Category = cmbCategory.Text;
                ab.Image = txtFilePath.Text;
                ab.Description = txtDescription.Text;
                if (rdbInStock.IsChecked == true)
                {
                    ab.IsStock = "In Stock";
                }
                if (rdbOutStock.IsChecked == true)
                {
                    ab.IsStock = "Out Of Stock";
                }
                var newBook = "{'Id':'"+ab.Id+ "','BookName':'" + ab.BookName + "','AuthorName':'" + ab.AuthorName + "','Price':'" + ab.Price + "','Quantity':'" + ab.Quantity + "','Category':'" + ab.Category + "','Image':'" + ab.Image + "','Description':'" + ab.Description + "','IsStock':'" + ab.IsStock + "'}";

                var newBookJson = File.ReadAllText(@"Products.json");
                var jsonObject = JObject.Parse(newBookJson);
                var bookArr = jsonObject.GetValue("Products") as JArray;
                var product = JObject.Parse(newBook);
                bookArr.Add(product);


                jsonObject["Products"] = bookArr;
                string jsonResult = JsonConvert.SerializeObject(jsonObject,Formatting.Indented);
                File.WriteAllText(@"Products.json", jsonResult);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error Occured" + ex.Message.ToString());



            }

        }
    }
}
