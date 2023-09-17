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
        public ChessGame selectedGame;
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
                AddSave(save);
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

            string saveName = (Saves.SelectedItem as ComboBoxItem).Content.ToString();

            //cancel overwriting
            if (SaveOverWriteCheck(saveName) == false) return;
            try
            {
                FileStream fs = new FileStream(saveFolder+"/"+saveName, FileMode.OpenOrCreate);
                bf.Serialize(fs, current);
                MessageBox.Show("Successfully saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                fs.Close();
                Saves_SelectionChanged(Saves, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private Chess LoadGame()
        {
            BinaryFormatter bf = new BinaryFormatter();
            SaveFolderCheck();


            Chess game;

            try
            {
                string saveName = (Saves.SelectedItem as ComboBoxItem).Content.ToString();
                using (FileStream fs = new FileStream(saveFolder + "/" + saveName, FileMode.Open))
                {
                    game = bf.Deserialize(fs) as Chess;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return game;
        }
        private void Load(object sender, RoutedEventArgs e)
        {
            try
            {
                current = LoadGame();
                if (current == null) throw new Exception("Failed to load save");
                MessageBox.Show("Successfully loaded.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void DisableButtons()
        {
            SaveButton.IsEnabled = false;
            LoadButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
        }
        private void EnableButtons()
        {
            SaveButton.IsEnabled = true;
            LoadButton.IsEnabled = true;
            RemoveButton.IsEnabled = true;
        }

        private bool SaveEmpty(string name)
        {
            try
            {
                FileStream fs = new FileStream(saveFolder + "/" + name, FileMode.Open);
                long len = fs.Length;
                fs.Close();
                if (len == 0) return true;
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
        private void AddSave(string name)
        {
            Saves.Items.Add(new ComboBoxItem() { Content = name, Background = Saves.Background });
        }
        private void Create(object sender, RoutedEventArgs e)
        {
            CreateSave createSave_page = new CreateSave();
            createSave_page.ShowDialog();
            if (createSave_page.DialogResult == true)
            {
                string saveName = createSave_page.SaveName;
                AddSave(saveName);
            }
        }
        private void DelFrame()
        {
            GameFrame.Content = null;
        }
        private void SetFrame()
        {
            selectedGame = new ChessGame();
            Chess game = LoadGame();
            if (game != null)
                selectedGame.StartGame(game);
            if (selectedGame == null) GameFrame.Source = null;
            else GameFrame.Content = selectedGame;
        }
        private void Saves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableButtons();
            ComboBox comboBox = sender as ComboBox;
            if ((comboBox.SelectedItem as ComboBoxItem) == null)
            {
                DisableButtons();
                DelFrame();
                return;
            }
            if (SaveEmpty((comboBox.SelectedItem as ComboBoxItem).Content.ToString()))
            {
                LoadButton.IsEnabled = false;
            }
            SetFrame();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            File.Delete(saveFolder + "/" + (Saves.SelectedItem as ComboBoxItem).Content.ToString());
            MessageBox.Show("Successfully removed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Saves.Items.Remove(Saves.SelectedItem);
        }
    }
}
