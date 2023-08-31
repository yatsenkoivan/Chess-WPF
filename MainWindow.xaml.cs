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
using System.Windows.Media.Media3D;
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
            Point clickpos = e.GetPosition(Board);

            double cell_size = Board.RowDefinitions[1].ActualHeight;
            //double cell_size = Math.Min(Width, Height) / 8;


            clickpos.X -= LeftOffset.ActualWidth;
            clickpos.Y -= UpOffset.ActualHeight;
            //Title = $"{clickpos.X} {clickpos.Y} : {cell_size}";

            clickpos.X /= cell_size;
            clickpos.Y /= cell_size;

            Coord coords = new Coord((int)clickpos.X, (int)clickpos.Y);

            if (start_coords == null)
            {

                //incorrect start cell
                if (chess.CheckStartCoord(coords) == false) return;

                start_coords = coords;

                HideMoveVariants();
                chess.SetMoveVariants(start_coords);
                ShowMoveVariants();
                if (chess.move_variants.Count == 0) start_coords = null;
            }
            else
            {
                //incorrect end cell
                if (CheckMove(start_coords, coords) == false) return;
                PieceMove(start_coords, coords);
                start_coords = null;
                ChangePlayer();
            }
        }
        private void PieceMove(Coord start_coords, Coord end_coords)
        {
            int size_x = chess.board.GetLength(1);
            int size_y = chess.board.GetLength(0);

            Piece start = chess.board[start_coords.Y,start_coords.X];
            Piece end = chess.board[end_coords.Y, end_coords.X];

            Piece p = chess.board[start_coords.Y, start_coords.X];
            if (p != null)
            {

                //EN PASSANT
                if (p.Type == Piece.Types.pawn)
                {
                    if (Math.Abs(end_coords.X - start_coords.X) == 1 && chess.board[end_coords.Y, end_coords.X] == null) //corner & empty
                    {
                        Board.Children.Remove(chess.board[start_coords.Y, end_coords.X].img);
                        chess.board[start_coords.Y, end_coords.X] = null;
                    }
                }

                //KING CASTLING
                if (p.Type == Piece.Types.rook)
                {
                    if (start_coords.X == 0)
                    {
                        if (chess.player == 1) chess.player1_castling.Long = false;
                        if (chess.player == 2) chess.player2_castling.Long = false;
                    }
                    if (start_coords.X == size_x - 1)
                    {
                        if (chess.player == 1) chess.player1_castling.Short = false;
                        if (chess.player == 2) chess.player2_castling.Short = false;
                    }
                }
                if (p.Type == Piece.Types.king)
                {
                    //long castling
                    if (((chess.player == 1 && chess.player1_castling.Long)
                        ||
                        (chess.player == 2 && chess.player2_castling.Long))
                        && end_coords.X == 2)
                    {
                        Grid.SetColumn(chess.board[end_coords.Y, 0].img, end_coords.X + 1 + 1);

                        chess.board[end_coords.Y, end_coords.X + 1] = chess.board[end_coords.Y, 0];
                        chess.board[end_coords.Y, 0] = null;
                    }

                    //short castling
                    if (((chess.player == 1 && chess.player1_castling.Short)
                        ||
                        (chess.player == 2 && chess.player2_castling.Short))
                        && end_coords.X == size_x - 1 - 1)
                    {
                        Grid.SetColumn(chess.board[end_coords.Y, size_x - 1].img, end_coords.X - 1 + 1);

                        chess.board[end_coords.Y, end_coords.X - 1] = chess.board[end_coords.Y, size_x - 1];
                        chess.board[end_coords.Y, size_x - 1] = null;
                    }


                    if (chess.player == 1)
                    {
                        chess.player1_castling.Long = false;
                        chess.player1_castling.Short = false;
                    }
                    if (chess.player == 2)
                    {
                        chess.player2_castling.Long = false;
                        chess.player2_castling.Short = false;
                    }
                }
            }

            chess.board[start_coords.Y, start_coords.X] = null;
            chess.board[end_coords.Y, end_coords.X] = start;

            if (end != null) Board.Children.Remove(end.img);

            Grid.SetRow(start.img, end_coords.Y + 1);
            Grid.SetColumn(start.img, end_coords.X + 1);

            HideMoveVariants();
            chess.DelMoveVariants();

            if (chess.player == 1)
            {
                chess.player1_moves.Add(new Move(start_coords, end_coords));
            }
            if (chess.player == 2)
            {
                chess.player2_moves.Add(new Move(start_coords, end_coords));
            }
        }
        private bool CheckMove(Coord start_coords, Coord end_coords)
        {
            if (chess.move_variants.Any(coords => coords.Equals(end_coords)) == false) return false;

            Piece p = chess.board[start_coords.Y,start_coords.X];
            if (p == null) return false;

            return true;
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                HideMoveVariants();
                chess.DelMoveVariants();
                start_coords = null;
            }
        }
    }
}
