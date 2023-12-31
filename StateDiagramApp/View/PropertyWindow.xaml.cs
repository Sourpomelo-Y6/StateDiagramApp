﻿using StateDiagramApp.ViewModel;
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

namespace StateDiagramApp.View
{
    /// <summary>
    /// PropertyWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PropertyWindow : Window
    {
        public PropertyWindow(NodeViewModel node)
        {
            InitializeComponent();

            DataContext = node;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
