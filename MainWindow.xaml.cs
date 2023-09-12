using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chess_WPF.Code;
using Chess_WPF.Pages;

namespace Chess_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessGame Game
        {
            get { return GameFrame.Content as ChessGame; }
        }
        Chess chess
        {
            get { return Game.Chess; }
            set { Game.Chess = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            ChessGame game = new ChessGame();
            game.StartGame();
            GameFrame.Content = game;
        }

        private void UpdateTitle()
        {
            Title = $"Chess | Player {chess.player}";
        }
        private void Press(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                Game.DelMoveVariants();
                Game.Start = null;
            }
            if (e.Key == Key.Escape)
            {
                OpenMenu();
            }
        }
        private void OpenMenu()
        {
            InGameMenu menu = new InGameMenu(chess);
            menu.ShowDialog();

            Game.StartGame(menu.current);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double size = Math.Min(ActualWidth, ActualHeight);
            if (size == ActualHeight)
            {
                LeftOffset.Width = (ActualWidth - size) / 2;
                RightOffset.Width = (ActualWidth - size) / 2;
                TopOffset.Height = 0;
                BottomOffset.Height = 0;
            }
            else
            {
                TopOffset.Height = (ActualHeight - size) / 2;
                BottomOffset.Height = (ActualHeight - size) / 2;
                LeftOffset.Width = 0;
                RightOffset.Width = 0;
            }
        }

        private void GameFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(GameFrame);

            double cell_size = (GameFrame.Content as ChessGame).Board.RowDefinitions[0].ActualHeight;

            Coord coords = new Coord((int)(pos.X / cell_size), (int)(pos.Y / cell_size));

            //edge fix
            coords.X = Math.Min(7, coords.X);
            coords.Y = Math.Min(7, coords.Y);

            //MOVED
            Game.Click(coords);
            UpdateTitle();


            //Close game
            if (Game.Content == null) Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTitle();
        }
    }
}
