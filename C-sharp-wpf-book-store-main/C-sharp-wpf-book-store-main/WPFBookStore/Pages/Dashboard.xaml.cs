using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WPFBookStore.Pages
{
    /// <summary>
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        public static int id;
        public static string bookName="";
        public static string authorName="";
        public static decimal price = decimal.Zero; 
        public static int quantity = 0;
        public static string category = "";
        public static string image = "";
        public static string description = "";



        private void GetItemList()
        {
            var json = File.ReadAllText(@"Products.json");
            var jObject = JObject.Parse(json);
            if (jObject != null)
            {
                JArray products = (JArray)jObject["Products"];
                if (products != null)
                {
                    List<Models.Addbook> abook = new List<Models.Addbook>()
                    {

                    };

                    foreach (var ibooks in products)
                    {
                        abook.Add(new Models.Addbook() { Id = Convert.ToInt32(ibooks["Id"]), 
                            BookName = ibooks["BookName"].ToString(), 
                            AuthorName = ibooks["AuthorName"].ToString(), 
                            Price = Convert.ToDecimal(ibooks["Price"]), 
                            Quantity = Convert.ToInt32(ibooks["Quantity"]), 
                            Category = ibooks["Category"].ToString(), 
                            Image = ibooks["Image"].ToString(), 
                            Description = ibooks["Description"].ToString() 
                        
                        });
                    }
                   
                   lvProduct.ItemsSource = abook;
                }
            }
        }



        private void btnView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            var itemsJson = File.ReadAllText(@"Products.json");
            try
            {
                var jobject = JObject.Parse(itemsJson);
                JArray bookArr = (JArray)jobject["Products"];
                Button button = sender as Button;
                Models.Addbook addbookId = button.CommandParameter as Models.Addbook;
                int bid = addbookId.Id;
                var bookToDelete = bookArr.FirstOrDefault(obj => obj["Id"].Value<int>() == bid);

                MessageBox.Show("Are you Sure to delet the Products");
                bookArr.Remove(bookToDelete);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jobject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(@"Products.json", output);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                MessageBox.Show("Data Deleted Successfully--!");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetItemList();
        }
    }
}
