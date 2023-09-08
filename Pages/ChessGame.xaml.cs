using Chess_WPF.Code;
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

namespace Chess_WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChessGame.xaml
    /// </summary>
    public partial class ChessGame : Page
    {
        Chess chess;
        public Chess Chess { get { return chess; } set { chess = value; } }
        Label start_label;
        Label player1_check_label;
        Label player2_check_label;
        #nullable enable
        Image?[,] piece_images;
        Coord? start_coords;
        public Coord? Start
        {
            get { return start_coords; }
            set
            {
                if (value != null)
                {
                    start_label.Visibility = Visibility.Visible;
                    Grid.SetRow(start_label, value.Y);
                    Grid.SetColumn(start_label, value.X);
                }
                else
                {
                    start_label.Visibility = Visibility.Hidden;
                }

                start_coords = value;
            }
        }
        #nullable disable
        HashSet<Label> move_variant_labels;
        public ChessGame()
        {
            InitializeComponent();
        }
        private void SetImages()
        {
            #nullable enable
            BitmapImage? image;
            #nullable disable
            //HidePieces();
            piece_images = new Image[chess.board.GetLength(0), chess.board.GetLength(1)];
            for (int row = 0; row < piece_images.GetLength(0); row++)
            {
                for (int col = 0; col < piece_images.GetLength(1); col++)
                {
                    image = PieceImages.GetImage(chess.board[row, col]);
                    if (image != null)
                    {
                        piece_images[row, col] = new Image() { Source = image };
                    }
                }
            }
        }
        public void StartGame(Chess game = null)
        {
            HidePieces();

            if (game == null) chess = new Chess();
            else chess = game;
            move_variant_labels = new HashSet<Label>();


            start_label = new Label();
            start_label.Style = Resources["StartCell"] as Style;
            Board.Children.Add(start_label);
            Start = null;

            player1_check_label = new Label();
            player2_check_label = new Label();

            player1_check_label.Style = Resources["Checked"] as Style;
            player2_check_label.Style = Resources["Checked"] as Style;

            Board.Children.Add(player1_check_label);
            Board.Children.Add(player2_check_label);

            UpdateCheckLabel();

            UpdateCheckLabelVisibility();

            SetImages();
            ShowPieces();
        }
        public void ChangePlayer()
        {
            chess.player = (chess.player == 1 ? 2 : 1);
        }

        public void Click(Coord coords)
        {

            Piece p = chess.board[coords.Y, coords.X];

            if (Start == null)
            {
                ChooseStart(coords);
            }
            else
            {
                //choose another piece
                if ((chess.player == 1 && p != null && p.Side == Piece.Sides.white)
                    ||
                    (chess.player == 2 && p != null && p.Side == Piece.Sides.black)) ChooseStart(coords);
                //else
                else
                    ChooseEnd(coords);
            }
        }
        private void ChooseEnd(Coord coords)
        {
            //incorrect end cell
            if (CheckMove(Start, coords) == false) return;
            PieceMove(Start, coords);

            KingCheck(coords);
            Start = null;
            ChangePlayer();

            GameEndCheck();
        }
        private void ChooseStart(Coord coords)
        {
            //incorrect start cell
            if (chess.CheckStartCoord(coords) == false) return;
            Start = coords;

            //HideMoveVariants();
            DelMoveVariants();
            chess.SetMoveVariants(Start);
            chess.DelCheckMateMoves(Start);
            ShowMoveVariants();
            if (chess.move_variants.Count == 0) start_coords = null;
        }
        private void UpdateCheckLabel()
        {
            Grid.SetRow(player1_check_label, chess.player1_king_coords.Y);
            Grid.SetColumn(player1_check_label, chess.player1_king_coords.X);

            Grid.SetRow(player2_check_label, chess.player2_king_coords.Y);
            Grid.SetColumn(player2_check_label, chess.player2_king_coords.X);
        }
        private void UpdateCheckLabelVisibility()
        {
            if (chess.player1_checked) player1_check_label.Visibility = Visibility.Visible;
            else player1_check_label.Visibility = Visibility.Hidden;

            if (chess.player2_checked) player2_check_label.Visibility = Visibility.Visible;
            else player2_check_label.Visibility = Visibility.Hidden;
        }
        private void PieceMove(Coord start_coords, Coord end_coords)
        {
            int size_x = chess.board.GetLength(0);

            Piece start = chess.board[start_coords.Y, start_coords.X];
            Piece end = chess.board[end_coords.Y, end_coords.X];

            Piece p = chess.board[start_coords.Y, start_coords.X];
            if (p != null)
            {

                //EN PASSANT
                if (p.Type == Piece.Types.pawn)
                {
                    if (Math.Abs(end_coords.X - start_coords.X) == 1 && chess.board[end_coords.Y, end_coords.X] == null) //corner & empty
                    {
                        Board.Children.Remove(piece_images[start_coords.Y, end_coords.X]);
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
                    if (chess.player == 1)
                    {
                        chess.player1_king_coords = end_coords;
                        UpdateCheckLabel();
                    }
                    if (chess.player == 2)
                    {
                        chess.player2_king_coords = end_coords;
                        UpdateCheckLabel();
                    }
                    //long castling
                    if (((chess.player == 1 && chess.player1_castling.Long)
                        ||
                        (chess.player == 2 && chess.player2_castling.Long))
                        && end_coords.X == 2)
                    {
                        Grid.SetColumn(piece_images[end_coords.Y, 0], end_coords.X + 1);

                        chess.board[end_coords.Y, end_coords.X + 1] = chess.board[end_coords.Y, 0];
                        chess.board[end_coords.Y, 0] = null;
                    }

                    //short castling
                    if (((chess.player == 1 && chess.player1_castling.Short)
                        ||
                        (chess.player == 2 && chess.player2_castling.Short))
                        && end_coords.X == size_x - 1 - 1)
                    {
                        Grid.SetColumn(piece_images[end_coords.Y, size_x - 1], end_coords.X - 1);

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

            if (end != null) Board.Children.Remove(piece_images[end_coords.Y, end_coords.X]);

            piece_images[end_coords.Y, end_coords.X] = piece_images[start_coords.Y, start_coords.X];
            piece_images[start_coords.Y, start_coords.X] = null;


            Grid.SetRow(piece_images[end_coords.Y, end_coords.X], end_coords.Y);
            Grid.SetColumn(piece_images[end_coords.Y, end_coords.X], end_coords.X);

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

            Piece p = chess.board[start_coords.Y, start_coords.X];
            if (p == null) return false;

            return true;
        }
        private void KingCheck(Coord coord)
        {
            chess.player1_checked = false;
            chess.player2_checked = false;

            UpdateCheckLabelVisibility();


            if (chess.KingCheck(coord))
            {
                if (chess.player == 1)
                {
                    chess.player2_checked = true;
                }
                if (chess.player == 2)
                {
                    chess.player1_checked = true;
                }
            }

            UpdateCheckLabelVisibility();
        }
        private void GameEndCheck()
        {
            int size_y = chess.board.GetLength(0);
            int size_x = chess.board.GetLength(1);

            //CheckMate / StaleMate check
            for (int row = 0; row < size_y && chess.move_variants.Count == 0; row++)
            {
                for (int col = 0; col < size_x && chess.move_variants.Count == 0; col++)
                {
                    if (chess.board[row, col] == null) continue;
                    if ((chess.board[row, col].Side == Piece.Sides.white && chess.player == 1)
                        ||
                        (chess.board[row, col].Side == Piece.Sides.black && chess.player == 2))
                    {
                        chess.SetMoveVariants(new Coord(col, row));
                        chess.DelCheckMateMoves(new Coord(col, row));
                    }
                }
            }
            //NO MOVES
            if (chess.move_variants.Count == 0)
            {
                if (chess.player == 1 && chess.player1_checked)
                    GameEnd("Player 2 Won! by a checkmate");
                else if (chess.player == 2 && chess.player2_checked)
                    GameEnd("Player 1 Won! by a checkmate");
                else
                    GameEnd("Draw. By a stalemate");
            }
            chess.move_variants.Clear();
        }
        private void ShowPieces()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (piece_images[row, col] != null)
                    {
                        Board.Children.Add(piece_images[row, col]);
                        Grid.SetRow(piece_images[row, col], row);
                        Grid.SetColumn(piece_images[row, col], col);
                    }
                }
            }
        }
        public void HidePieces()
        {
            /*foreach (Image img in piece_images)
            {
                if (Board.Children.Contains(img)) Board.Children.Remove(img);
            }*/
            Board.Children.Clear();
        }
        private void GameEnd(string message)
        {
            MessageBox.Show(message, "Game End", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            MessageBoxResult result = MessageBox.Show("Start a new game?", "New game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.No) Close();
            HidePieces();
            StartGame();

            //InitBoard();
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
                Grid.SetRow(label, coords.Y);
                Grid.SetColumn(label, coords.X);
            }
        }
        public void DelMoveVariants()
        {
            foreach (Label label in move_variant_labels)
            {
                Board.Children.Remove(label);
            }
            move_variant_labels.Clear();
            chess.DelMoveVariants();
        }
    }
}
