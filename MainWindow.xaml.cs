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
using Chess_WPF.Code;

namespace Chess_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Chess chess;
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
    }
}
