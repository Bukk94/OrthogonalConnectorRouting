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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ConnectorVM();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(Canvas);
            this.MousePositionTxt.Text = $"{{ {position.X} : {position.Y} }}";
        }
    }
}
