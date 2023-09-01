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
        HashSet<Label> move_variant_labels;
        public MainWindow()
        {
            InitializeComponent();
            chess = new Chess();
            move_variant_labels = new HashSet<Label>();

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
            chess.player1_check_label.Style = Resources["Checked"] as Style;
            chess.player2_check_label.Style = Resources["Checked"] as Style;

            chess.player1_check_label.Visibility = Visibility.Hidden;
            chess.player2_check_label.Visibility = Visibility.Hidden;

            Board.Children.Add(chess.player1_check_label);
            Board.Children.Add(chess.player2_check_label);

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
            //Title = $"{chess.player1_king_coords.X} {chess.player2_king_coords.Y}";

            clickpos.X /= cell_size;
            clickpos.Y /= cell_size;

            Coord coords = new Coord((int)clickpos.X, (int)clickpos.Y);

            if (start_coords == null)
            {

                //incorrect start cell
                if (chess.CheckStartCoord(coords) == false) return;

                start_coords = coords;

                //HideMoveVariants();
                DelMoveVariants();
                chess.SetMoveVariants(start_coords);
                chess.DelCheckMateMoves(start_coords);
                ShowMoveVariants();
                if (chess.move_variants.Count == 0) start_coords = null;
            }
            else
            {
                //incorrect end cell
                if (CheckMove(start_coords, coords) == false) return;
                PieceMove(start_coords, coords);

                KingCheck(coords);
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
                    if (chess.player == 1) chess.Player1_king = end_coords;
                    if (chess.player == 2) chess.Player2_king = end_coords;
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

            DelMoveVariants();

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
        private void KingCheck(Coord coord)
        {
            chess.player1_checked = false;
            chess.player1_check_label.Visibility = Visibility.Hidden;

            chess.player2_checked = false;
            chess.player2_check_label.Visibility = Visibility.Hidden;


            if (chess.KingCheck(coord))
            {
                if (chess.player == 1)
                {
                    chess.player2_checked = true;
                    chess.player2_check_label.Visibility = Visibility.Visible;
                }
                if (chess.player == 2)
                {
                    chess.player1_checked = true;
                    chess.player1_check_label.Visibility = Visibility.Visible;
                }
            }
        }
        private void ShowMoveVariants()
        {
            Label label;
            foreach (Coord coords in chess.move_variants)
            {
                label = new Label();
                label.Style = Resources["Variant"] as Style;
                move_variant_labels.Add(label);
                Board.Children.Add(label);
                Grid.SetRow(label, coords.Y + 1);
                Grid.SetColumn(label, coords.X + 1);
            }
        }
        private void DelMoveVariants()
        {
            foreach (Label label in move_variant_labels)
            {
                Board.Children.Remove(label);
            }
            move_variant_labels.Clear();
            chess.DelMoveVariants();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                DelMoveVariants();
                start_coords = null;
            }
        }
    }
}
