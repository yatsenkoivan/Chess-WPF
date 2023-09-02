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
using Chess_WPF.Code;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Chess_WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для InGameMenu.xaml
    /// </summary>
    public partial class InGameMenu : Window
    {
        public Chess current;
        static string savePath = "save";
        public InGameMenu(Chess current)
        {
            this.current = current;
            InitializeComponent();
        }

        public Chess Current { get; }

        private void Save(object sender, RoutedEventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
            try
            {
                bf.Serialize(fs, current);
                MessageBox.Show("Successfully saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                fs.Close();
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
            try
            {
                current = bf.Deserialize(fs) as Chess;
                MessageBox.Show("Successfully loaded", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
