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

namespace StateDiagramApp.View
{
    /// <summary>
    /// NodeControl.xaml の相互作用ロジック
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public NodeControl()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            
        }

        public static readonly DependencyProperty SelectMarkProperty =
                               DependencyProperty.Register("SelectMark", typeof(bool), typeof(NodeControl), 
                                                            new PropertyMetadata(false, OnSelectMarkPropertyChanged));
        public bool SelectMark
        {
            get { return (bool)GetValue(SelectMarkProperty); }
            set { SetValue(SelectMarkProperty, value); }
        }

        private static void OnSelectMarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NodeControl;
            control.SelectMark = (bool)e.NewValue;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateNode();
        }

        private void UpdateNode()
        {
            if (SelectMark)
            {
                Mark.Stroke = Brushes.Red;
            }
            else 
            {
                Mark.Stroke = Brushes.Transparent;
            }
        }
    }
}
