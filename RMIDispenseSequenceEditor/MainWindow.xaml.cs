using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Newtonsoft.Json;
using RMIDispenseSequenceEditor.Models;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispenseProtocolEditorViewModel ViewModel { get; set; }

        
        public MainWindow()
        {
            InitializeComponent();
         //   var simulator = new SimulateDispense()
            ViewModel = new DispenseProtocolEditorViewModel();
            DataContext = ViewModel;
        }

    }
}