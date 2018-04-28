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
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;

namespace ABCSolutionsWPF
{
    /// <summary>
    /// Interaction logic for FormLookUp.xaml
    /// </summary>
    public partial class FormLookUp : Window
    {
        public List<Grades> grades;
        public List<Dictionary<string, string>> schools;
        public Dictionary<string, string> dictSchools;
        public BlockChainClient client;

        public FormLookUp()
        {
            InitializeComponent();
            init();
        }
        
        private void init()
        {
            grades = new List<Grades>();
            dgGrades.ItemsSource = grades;

            dictSchools = new Dictionary<string, string>
            {
                {"Tsinghua University","i9d365d46dc450c068dc90d4d7e90d8397e4301ff"},
                {"University of Waterloo","i2404d1fc87e790eb390f8a077554538c78e05c49"}
            };

            this.client = BlockChainClient.GetInstance();
            this.cbSchools.ItemsSource = dictSchools;
            
            this.tbName.Visibility = Visibility.Hidden;
            this.lbName.Visibility = Visibility.Hidden;
            this.tbStudID.Visibility = Visibility.Hidden;
            this.lbStudID.Visibility = Visibility.Hidden;
            this.tbTerm.Visibility = Visibility.Hidden;
            this.lbTerm.Visibility = Visibility.Hidden;
            this.cbSchools.Visibility = Visibility.Hidden;
            this.lbSchools.Visibility = Visibility.Hidden;
            this.dgGrades.Visibility = Visibility.Hidden;

        }
        
        private void CallAPI()
        {

        }
        public class Credentials
        {
            public string Name { get; set; }
            public string School { get; set; }
            public string StudID { get; set; }
            public string Term { get; set; }
            public List<Grades> transcript { get; set; }
        }

        public class Grades
        {
            public string CourseID { get; set; }
            public string CourseName { get; set; }
            public string Credit { get; set; }
            public string Mark { get; set; }
  
            public Grades()
            {
            }
        }
        

        private void btLookUp_Click(object sender, RoutedEventArgs e)
        {
            string key = this.tbKey.Text;
            string ID = this.tbID.Text;

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("请填入您的成绩单编号和私钥");
                return;
            }

            this.tbName.Visibility = Visibility.Visible;
            this.lbName.Visibility = Visibility.Visible;
            this.tbStudID.Visibility = Visibility.Visible;
            this.lbStudID.Visibility = Visibility.Visible;
            this.tbTerm.Visibility = Visibility.Visible;
            this.lbTerm.Visibility = Visibility.Visible;
            this.cbSchools.Visibility = Visibility.Visible;
            this.lbSchools.Visibility = Visibility.Visible;
            this.dgGrades.Visibility = Visibility.Visible;

            string response = this.client.queryTranscript("", ID);
            byte[] text = Crypto.Decode(response, key);

        }
    }
}
