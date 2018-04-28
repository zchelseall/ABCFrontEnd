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
using System.Windows.Shapes;

namespace ABCSolutionsWPF
{
    /// <summary>
    /// Interaction logic for FormPrivateKey.xaml
    /// </summary>
    public partial class FormPrivateKey : Window
    {
        public FormPrivateKey(string ID, string key)
        {
            InitializeComponent();
            init(ID, key);
        }

        private void init(string ID, string key)
        {
            this.tbID.Text = ID;
            this.tbKey.Text = key;
        }
    }
}
