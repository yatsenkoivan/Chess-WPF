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
using System.IO;

namespace Chess_WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateSave.xaml
    /// </summary>
    public partial class CreateSave : Window
    {
        public CreateSave()
        {
            InitializeComponent();
        }

        public string SaveName
        {
            get { return saveName.Text; }
        }

        private void saveName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (saveName.Text.Length > 0) createButton.IsEnabled = true;
            else createButton.IsEnabled = false;
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            if (saveName.Text == "")
            {
                MessageBox.Show("File name cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //save already exists
            if (File.Exists(InGameMenu.saveFolder + "/" + SaveName) == true)
            {
                MessageBox.Show($"Save '{SaveName}' already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FileStream fs = File.Create(InGameMenu.saveFolder + "/" + SaveName);
            fs.Close();
            MessageBox.Show($"Save '{SaveName}' successfully created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
