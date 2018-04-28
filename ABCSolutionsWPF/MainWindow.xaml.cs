﻿using System;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            FormSubmit formSubmit = new FormSubmit();
            formSubmit.Show();

        }

        private void btLookUp_Click(object sender, RoutedEventArgs e)
        {
            FormLookUp formLookup = new FormLookUp();
            formLookup.Show();
        }
    }
}
