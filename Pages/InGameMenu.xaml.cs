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
        public static string saveFolder = "saves";
        public InGameMenu(Chess current)
        {
            this.current = current;

            InitializeComponent();

            DisableButtons();
            SetSaves();
        }

        public Chess Current { get; }

        private void SetSaves()
        {
            SaveFolderCheck();
            var saves = Directory.GetFiles(saveFolder).Select(System.IO.Path.GetFileName);

            foreach (string save in saves)
            {
                Saves.Items.Add(save);
            }
        }
        private void SaveFolderCheck()
        {
            if (Directory.Exists(saveFolder) == false) Directory.CreateDirectory(saveFolder);
        }
        private bool SaveOverWriteCheck(string saveName)
        {
            if (SaveEmpty(saveName)) return true;
            MessageBoxResult result = MessageBox.Show($"Save '{saveName}' is not empty. Overwrite?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) return true;
            return false;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            SaveFolderCheck();

            string saveName = Saves.SelectedItem.ToString();

            //cancel overwriting
            if (SaveOverWriteCheck(saveName) == false) return;
            FileStream fs = new FileStream(saveFolder+"/"+saveName, FileMode.OpenOrCreate);
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
            SaveFolderCheck();

            string saveName = Saves.SelectedItem.ToString();

            FileStream fs = new FileStream(saveFolder + "/" + saveName, FileMode.Open);
            
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
        private void DisableButtons()
        {
            SaveButton.IsEnabled = false;
            LoadButton.IsEnabled = false;
        }
        private void EnableButtons()
        {
            SaveButton.IsEnabled = true;
            LoadButton.IsEnabled = true;
        }

        private bool SaveEmpty(string name)
        {
            FileStream fs = new FileStream(saveFolder + "/" + name, FileMode.Open);
            long len = fs.Length;
            fs.Close();
            if (len == 0) return true;
            return false;
        }
        private void Add(object sender, RoutedEventArgs e)
        {
            CreateSave createSave_page = new CreateSave();
            createSave_page.ShowDialog();
            if (createSave_page.DialogResult == true)
            {
                string saveName = createSave_page.SaveName;
                Saves.Items.Add(saveName);
                Saves.SelectedItem = saveName;
            }
        }
        private void Saves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableButtons();
            if (SaveEmpty((sender as ComboBox).SelectedItem.ToString()))
            {
                LoadButton.IsEnabled = false;
            }
        }
    }
}
