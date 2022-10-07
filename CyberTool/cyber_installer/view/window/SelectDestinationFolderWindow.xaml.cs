﻿using cyber_base.implement.views.cyber_window;
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

namespace cyber_installer.view.window
{
    /// <summary>
    /// Interaction logic for SelectDestinationFolderWindow.xaml
    /// </summary>
    public partial class SelectDestinationFolderWindow : CyberWindow
    {

        public SelectDestinationFolderWindow()
        {
            InitializeComponent();
        }

        public string show()
        {
            base.ShowDialog();
            return PART_DestinationFolderTextBox.Text;
        }
    }
}
