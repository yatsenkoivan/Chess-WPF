using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chess_WPF.Code;

namespace Chess_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Chess chess;
        #nullable enable
        Coord? start_coords;
        #nullable disable
        public MainWindow()
        {
            InitializeComponent();
            chess = new Chess();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chess.board[row, col] != null)
                    {
                        Board.Children.Add(chess.board[row, col].img);
                        Grid.SetRow(chess.board[row, col].img, row + 1);
                        Grid.SetColumn(chess.board[row, col].img, col + 1);
                    }
                }
            }
            UpdateTitle();
        }
        private void ChangePlayer()
        {
            chess.player = (chess.player == 1 ? 2 : 1);
            UpdateTitle();
        }
        private void UpdateTitle()
        {
            Title = $"Chess | Player {chess.player}";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double board_size = Math.Min(this.ActualWidth, this.ActualHeight);

            double offset_x = Math.Max((this.ActualWidth - board_size) / 2, 0);
            double offset_y = Math.Max((this.ActualHeight - board_size) / 2, 0);

            GridLength horizonal = new GridLength(offset_x);
            GridLength vertical = new GridLength(offset_y);
            LeftOffset.Width = horizonal;
            RightOffset.Width = horizonal;
            UpOffset.Height = vertical;
            DownOffset.Height = vertical;
        }
        private void Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double cell_size = (Board.Children[0] as Label).ActualWidth;

            Point clickpos = e.GetPosition(sender as IInputElement);

            clickpos.X -= LeftOffset.ActualWidth;
            clickpos.Y -= UpOffset.ActualHeight;
            //Title = $"{clickpos.X} {clickpos.Y} : {cell_size}";

            clickpos.X /= cell_size;
            clickpos.Y /= cell_size;

            Coord coords = new Coord((int)clickpos.X, (int)clickpos.Y);

            if (start_coords == null)
            {
                start_coords = coords;

                //incorrect start cell
                if (chess.CheckStartCoord(start_coords) == false) return;

                HideMoveVariants();
                chess.SetMoveVariants(start_coords);
                ShowMoveVariants();
            }
            else
            {
                //TODO

                start_coords = null;
                ChangePlayer();
            }
        }
        private void HideMoveVariants()
        {
            Label label;
            foreach (Coord coords in chess.move_variants)
            {
                label = Board.Children[coords.Y * chess.board.GetLength(0) + coords.X] as Label;
                if (label.Style == Resources["WhiteCellMove"] as Style)
                {
                    label.Style = Resources["WhiteCell"] as Style;
                }
                else
                {
                    label.Style = Resources["BlackCell"] as Style;
                }
            }
        }
        private void ShowMoveVariants()
        {
            Label label;
            foreach (Coord coords in chess.move_variants)
            {
                label = Board.Children[coords.Y * chess.board.GetLength(0) + coords.X] as Label;
                if (label.Style == Resources["WhiteCell"] as Style)
                {
                    label.Style = Resources["WhiteCellMove"] as Style;
                }
                else
                {
                    label.Style = Resources["BlackCellMove"] as Style;
                }
            }
        }
    }
}
