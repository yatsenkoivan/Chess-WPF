using Chess_WPF.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Chess_WPF.Code
{
    [Serializable]
    public class Coord : IEquatable<Coord>
    {
        public int X { set; get; }
        public int Y { set; get; }
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public bool Equals(Coord other)
        {
            if (other == null)
                return false;
            return (this.X == other.X && this.Y == other.Y);
        }
    }
    [Serializable]
    public class Castling
    {
        public bool Long;
        public bool Short;
        public Castling()
        {
            Long = true;
            Short = true;
        }
    }
    [Serializable]
    public class Move
    {
        public Coord start;
        public Coord end;
        public Move(Coord start, Coord end)
        {
            this.start = start;
            this.end = end;
        }
    }
    [Serializable]
    public class Chess
    {
        public int player;
        #nullable enable
        public Piece[,]? board;
        #nullable disable
        public HashSet<Coord> move_variants;
        public Castling player1_castling;
        public Castling player2_castling;
        public int start_x;
        public int start_y;
        public List<Move> player1_moves;
        public List<Move> player2_moves;
        public bool player1_checked;
        public bool player2_checked;

        public Coord player1_king_coords;
        public Coord player2_king_coords;

        //50 moves rule
        static public double _50moves = 50;
        public double _50moves_counter;

        public Chess()
        {
            board = new Piece[8, 8];
            player = 1;
            SetBoard();
            start_x = -1;
            start_y = -1;
            move_variants = new HashSet<Coord>(60);
            player1_castling = new Castling();
            player2_castling = new Castling();
            player1_moves = new List<Move>();
            player2_moves = new List<Move>();
            player1_checked = false;
            player2_checked = false;

            player1_king_coords = new Coord(4, 7);
            player2_king_coords = new Coord(4, 0);

            _50moves_counter = _50moves;
        }
        public void SetBoard()
        {
            //PAWNS
            for (int col=0; col<8; col++)
            {
                board[1, col] = new Piece(Piece.Types.pawn, Piece.Sides.black);
                board[6, col] = new Piece(Piece.Types.pawn, Piece.Sides.white);
            }
            //ROOKS
            board[0,0] = new Piece(Piece.Types.rook, Piece.Sides.black);
            board[0,7] = new Piece(Piece.Types.rook, Piece.Sides.black);
            board[7,0] = new Piece(Piece.Types.rook, Piece.Sides.white);
            board[7,7] = new Piece(Piece.Types.rook, Piece.Sides.white);

            //KNIGHTS
            board[0, 1] = new Piece(Piece.Types.knight, Piece.Sides.black);
            board[0, 6] = new Piece(Piece.Types.knight, Piece.Sides.black);
            board[7, 1] = new Piece(Piece.Types.knight, Piece.Sides.white);
            board[7, 6] = new Piece(Piece.Types.knight, Piece.Sides.white);

            //BISHOPS
            board[0, 2] = new Piece(Piece.Types.bishop, Piece.Sides.black);
            board[0, 5] = new Piece(Piece.Types.bishop, Piece.Sides.black);
            board[7, 2] = new Piece(Piece.Types.bishop, Piece.Sides.white);
            board[7, 5] = new Piece(Piece.Types.bishop, Piece.Sides.white);

            //QUEENS
            board[0, 3] = new Piece(Piece.Types.queen, Piece.Sides.black);
            board[7, 3] = new Piece(Piece.Types.queen, Piece.Sides.white);

            //KINGS
            board[0, 4] = new Piece(Piece.Types.king, Piece.Sides.black);
            board[7, 4] = new Piece(Piece.Types.king, Piece.Sides.white);
        }
        public void DelMoveVariants()
        {
            move_variants.Clear();
        }
        public void SetMoveVariants(Coord coords)
        {

            int size_x = board.GetLength(1);
            int size_y = board.GetLength(0);

            Piece p = board[coords.Y,coords.X];
            if (p == null) return;

            move_variants.Clear();
            //PAWN
            if (p.Type == Piece.Types.pawn)
            {

                //WHITE PAWN
                if (p.Side == Piece.Sides.white)
                {
                    //up 1
                    if (coords.Y >= 1 && board[coords.Y - 1,coords.X] == null)
                        move_variants.Add(new Coord(coords.X, coords.Y - 1));
                    //up 2
                    if (coords.Y == size_y - 1 - 1 && board[coords.Y - 2,coords.X] == null && board[coords.Y - 1,coords.X] == null)
                        move_variants.Add(new Coord(coords.X, coords.Y - 2));
                    //left up enemy
                    if (coords.Y >= 1 && coords.X >= 1 && board[coords.Y - 1,coords.X - 1] != null && board[coords.Y - 1,coords.X - 1].Side != p.Side)
                        move_variants.Add(new Coord(coords.X - 1, coords.Y - 1));
                    //right up enemy
                    if (coords.Y >= 1 && coords.X <= size_x - 1 - 1 && board[coords.Y - 1,coords.X + 1] != null && board[coords.Y - 1,coords.X + 1].Side != p.Side)
                        move_variants.Add(new Coord(coords.X + 1, coords.Y - 1));
                    //en passant
                    if (coords.Y == size_y - 1 - 1 - 3)
                    {
                        if (coords.X >= 1 && board[coords.Y,coords.X - 1] != null
                            && board[coords.Y,coords.X - 1].Type == Piece.Types.pawn && board[coords.Y,coords.X - 1].Side == Piece.Sides.black
                            && player2_moves.Last().start.Equals(new Coord(coords.X - 1, coords.Y - 2)) //from
                            && player2_moves.Last().end.Equals(new Coord(coords.X - 1, coords.Y))) //to
                            move_variants.Add(new Coord(coords.X - 1, coords.Y - 1));

                        if (coords.X <= size_x - 1 - 1 && board[coords.Y, coords.X + 1] != null
                            && board[coords.Y, coords.X + 1].Type == Piece.Types.pawn && board[coords.Y, coords.X + 1].Side == Piece.Sides.black
                            && player2_moves.Last().start.Equals(new Coord(coords.X + 1, coords.Y - 2)) //from
                            && player2_moves.Last().end.Equals(new Coord(coords.X + 1, coords.Y))) //to
                            move_variants.Add(new Coord(coords.X + 1, coords.Y - 1));
                    }
                }

                //BLACK PAWN
                if (p.Side == Piece.Sides.black)
                {
                    //down 1
                    if (coords.Y <= size_y - 1 - 1 && board[coords.Y + 1,coords.X] == null)
                        move_variants.Add(new Coord(coords.X, coords.Y + 1));
                    //down 2
                    if (coords.Y == 1 && board[coords.Y + 2,coords.X] == null && board[coords.Y + 1,coords.X] == null)
                        move_variants.Add(new Coord(coords.X, coords.Y + 2));
                    //left up enemy
                    if (coords.Y <= size_y - 1 - 1 && coords.X >= 1 && board[coords.Y + 1,coords.X - 1] != null && board[coords.Y + 1,coords.X - 1].Side != p.Side)
                        move_variants.Add(new Coord(coords.X - 1, coords.Y + 1));
                    //right up enemy
                    if (coords.Y <= size_y - 1 - 1 && coords.X <= size_x - 1 - 1 && board[coords.Y + 1,coords.X + 1] != null && board[coords.Y + 1,coords.X + 1].Side != p.Side)
                        move_variants.Add(new Coord(coords.X + 1, coords.Y + 1));
                    //en passant
                    if (coords.Y == 4)
                    {
                        if (coords.X >= 1 && board[coords.Y, coords.X - 1] != null
                            && board[coords.Y, coords.X - 1].Type == Piece.Types.pawn && board[coords.Y, coords.X - 1].Side == Piece.Sides.white
                            && player1_moves.Last().start.Equals(new Coord(coords.X - 1, coords.Y + 2)) //from
                            && player1_moves.Last().end.Equals(new Coord(coords.X - 1, coords.Y))) //to
                            move_variants.Add(new Coord(coords.X - 1, coords.Y + 1));

                        if (coords.X <= size_x - 1 - 1 && board[coords.Y,coords.X + 1] != null
                            && board[coords.Y,coords.X + 1].Type == Piece.Types.pawn && board[coords.Y,coords.X + 1].Side == Piece.Sides.white
                            && player1_moves.Last().start.Equals(new Coord(coords.X + 1, coords.Y + 2)) //from
                            && player1_moves.Last().end.Equals(new Coord(coords.X + 1, coords.Y))) //to
                            move_variants.Add(new Coord(coords.X + 1, coords.Y + 1));
                    }
                }
            }

            //KNIGHTS
            /*o - from
              x - to  */
            if (p.Type == Piece.Types.knight)
            {
                /*x.
                   .
                   o*/
                if (coords.X >= 1 && coords.Y >= 2 && (board[coords.Y - 2,coords.X - 1] == null || board[coords.Y - 2,coords.X - 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 1, coords.Y - 2));
                /*.x
                  . 
                  o*/
                if (coords.X <= size_x - 1 - 1 && coords.Y >= 2 && (board[coords.Y - 2,coords.X + 1] == null || board[coords.Y - 2,coords.X + 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 1, coords.Y - 2));
                /*. . x
                  o    */
                if (coords.X <= size_x - 1 - 2 && coords.Y >= 1 && (board[coords.Y - 1,coords.X + 2] == null || board[coords.Y - 1,coords.X + 2].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 2, coords.Y - 1));
                /*o
                  . . x*/
                if (coords.X <= size_x - 1 - 2 && coords.Y <= size_y - 1 - 1 && (board[coords.Y + 1,coords.X + 2] == null || board[coords.Y + 1,coords.X + 2].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 2, coords.Y + 1));
                /*o
                  .
                  .x*/
                if (coords.X <= size_x - 1 - 1 && coords.Y <= size_y - 1 - 2 && (board[coords.Y + 2,coords.X + 1] == null || board[coords.Y + 2,coords.X + 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 1, coords.Y + 2));
                /* o
                   .
                  x.*/
                if (coords.X >= 1 && coords.Y <= size_y - 1 - 2 && (board[coords.Y + 2,coords.X - 1] == null || board[coords.Y + 2,coords.X - 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 1, coords.Y + 2));
                /*    o
                  x . .*/
                if (coords.X >= 2 && coords.Y <= size_y - 1 - 1 && (board[coords.Y + 1,coords.X - 2] == null || board[coords.Y + 1,coords.X - 2].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 2, coords.Y + 1));
                /*x . .
                      o*/
                if (coords.X >= 2 && coords.Y >= 1 && (board[coords.Y - 1,coords.X - 2] == null || board[coords.Y - 1,coords.X - 2].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 2, coords.Y - 1));
            }
            //ROOKS (& QUEEN)
            if (p.Type == Piece.Types.rook || p.Type == Piece.Types.queen)
            {
                bool right = true, left = true, up = true, down = true;
                for (int i = 1; i <= size_x && (right || left || up || down); i++)
                {
                    //right
                    if (coords.X + i <= size_x - 1 && right)
                    {
                        if (board[coords.Y,coords.X + i] == null || board[coords.Y,coords.X + i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X + i, coords.Y));
                            //last
                            if (board[coords.Y,coords.X + i] != null)
                                right = false;
                        }
                        else
                        {
                            right = false;
                        }
                    }
                    //left
                    if (coords.X - i >= 0 && left)
                    {
                        if (board[coords.Y,coords.X - i] == null || board[coords.Y,coords.X - i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X - i, coords.Y));
                            //last
                            if (board[coords.Y,coords.X - i] != null)
                                left = false;
                        }
                        else
                        {
                            left = false;
                        }
                    }
                    //down
                    if (coords.Y + i <= size_y - 1 && down)
                    {
                        if (board[coords.Y + i,coords.X] == null || board[coords.Y + i,coords.X].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X, coords.Y + i));
                            //last
                            if (board[coords.Y + i,coords.X] != null)
                                down = false;
                        }
                        else
                        {
                            down = false;
                        }
                    }
                    //up
                    if (coords.Y - i >= 0 && up)
                    {
                        if (board[coords.Y - i,coords.X] == null || board[coords.Y - i,coords.X].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X, coords.Y - i));
                            //last
                            if (board[coords.Y - i,coords.X] != null)
                                up = false;
                        }
                        else
                        {
                            up = false;
                        }
                    }
                }
            }
            //BISHOP (& QUEEN)
            if (p.Type == Piece.Types.bishop || p.Type == Piece.Types.queen)
            {
                bool right_up = true, right_down = true, left_up = true, left_down = true;
                for (int i = 1; i <= size_x && (right_up || right_down || left_up || left_down); i++)
                {
                    //right-up
                    if (coords.X + i <= size_x - 1 && coords.Y - i >= 0 && right_up)
                    {
                        if (board[coords.Y - i,coords.X + i] == null || board[coords.Y - i,coords.X + i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X + i, coords.Y - i));
                            //last
                            if (board[coords.Y - i,coords.X + i] != null)
                                right_up = false;
                        }
                        else
                        {
                            right_up = false;
                        }
                    }
                    //right-down
                    if (coords.X + i <= size_x - 1 && coords.Y + i <= size_y - 1 && right_down)
                    {
                        if (board[coords.Y + i,coords.X + i] == null || board[coords.Y + i,coords.X + i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X + i, coords.Y + i));
                            //last
                            if (board[coords.Y + i,coords.X + i] != null)
                                right_down = false;
                        }
                        else
                        {
                            right_down = false;
                        }
                    }
                    //left-up
                    if (coords.X - i >= 0 && coords.Y - i >= 0 && left_up)
                    {
                        if (board[coords.Y - i,coords.X - i] == null || board[coords.Y - i,coords.X - i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X - i, coords.Y - i));
                            //last
                            if (board[coords.Y - i,coords.X - i] != null)
                                left_up = false;
                        }
                        else
                        {
                            left_up = false;
                        }
                    }
                    //left-down
                    if (coords.X - i >= 0 && coords.Y + i <= size_y - 1 && left_down)
                    {
                        if (board[coords.Y + i,coords.X - i] == null || board[coords.Y + i,coords.X - i].Side != p.Side)
                        {
                            move_variants.Add(new Coord(coords.X - i, coords.Y + i));
                            //last
                            if (board[coords.Y + i,coords.X - i] != null)
                                left_down = false;
                        }
                        else
                        {
                            left_down = false;
                        }
                    }
                }
            }
            //KING
            if (p.Type == Piece.Types.king)
            {
                //right
                if (coords.X + 1 <= size_x - 1 && (board[coords.Y,coords.X + 1] == null || board[coords.Y,coords.X + 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 1, coords.Y));
                //left
                if (coords.X - 1 >= 0 && (board[coords.Y,coords.X - 1] == null || board[coords.Y,coords.X - 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 1, coords.Y));
                //down
                if (coords.Y + 1 <= size_y - 1 && (board[coords.Y + 1,coords.X] == null || board[coords.Y + 1,coords.X].Side != p.Side))
                    move_variants.Add(new Coord(coords.X, coords.Y + 1));
                //up
                if (coords.Y - 1 >= 0 && (board[coords.Y - 1,coords.X] == null || board[coords.Y - 1,coords.X].Side != p.Side))
                    move_variants.Add(new Coord(coords.X, coords.Y - 1));

                //right-down
                if (coords.X + 1 <= size_x - 1 && coords.Y + 1 <= size_y - 1 && (board[coords.Y + 1,coords.X + 1] == null || board[coords.Y + 1,coords.X + 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 1, coords.Y + 1));
                //left-down
                if (coords.X - 1 >= 0 && coords.Y + 1 <= size_y - 1 && (board[coords.Y + 1,coords.X - 1] == null || board[coords.Y + 1,coords.X - 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 1, coords.Y + 1));
                //right-up
                if (coords.X + 1 <= size_x - 1 && coords.Y - 1 >= 0 && (board[coords.Y - 1,coords.X + 1] == null || board[coords.Y - 1,coords.X + 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X + 1, coords.Y - 1));
                //left-up
                if (coords.X - 1 >= 0 && coords.Y - 1 >= 0 && (board[coords.Y - 1,coords.X - 1] == null || board[coords.Y - 1,coords.X - 1].Side != p.Side))
                    move_variants.Add(new Coord(coords.X - 1, coords.Y - 1));

                bool left_castling = true;  //long
                bool right_castling = true; //short
                //check if there are no pieces between king and rook
                for (int i = 1; coords.X - i > 0 && coords.X + i < size_x - 1 && (left_castling || right_castling); i++)
                {
                    if (board[coords.Y,coords.X + i] != null) right_castling = false;
                    if (board[coords.Y,coords.X - i] != null) left_castling = false;
                }
                if (left_castling && ((player == 1 && player1_castling.Long && player1_checked == false) || (player == 2 && player2_castling.Long && player2_checked == false))
                    && board[coords.Y,0] != null && board[coords.Y,0].Type == Piece.Types.rook)
                    move_variants.Add(new Coord(2, coords.Y));
                if (right_castling && ((player == 1 && player1_castling.Short && player1_checked == false) || (player == 2 && player2_castling.Short && player2_checked == false))
                    && board[coords.Y,size_x-1] != null && board[coords.Y,size_x-1].Type == Piece.Types.rook)
                    move_variants.Add(new Coord(size_x - 1 - 1, coords.Y));
            }
        }
        public bool KingCheck(Coord start)
        {
            int size_y = board.GetLength(0);
            int size_x = board.GetLength(1);


            if (start.X < 0 || start.X > size_x - 1 || start.Y < 0 || start.Y > size_y - 1) return false;
            Piece p = board[start.Y, start.X];
            if (p == null)
                return false;

            SetMoveVariants(start); //get all possible moves

            bool flag = false;

            foreach (Coord coords in move_variants)
            {
                if (p.Side == Piece.Sides.white)
                {
                    if (coords.Equals(player2_king_coords))
                    {
                        flag = true;
                    }
                }
                if (p.Side == Piece.Sides.black)
                {
                    if (coords.Equals(player1_king_coords))
                    {
                        flag = false;
                    }
                }
            }
            move_variants.Clear();
            return flag;
            
        }
        public void DelCheckMateMoves(Coord start)
        {
            int size_y = board.GetLength(0);
            int size_x = board.GetLength(1);

            Piece temp;
            HashSet<Coord> new_move_variants = new HashSet<Coord>(move_variants);
            HashSet<Coord> old_move_variants = new HashSet<Coord>(move_variants);
            foreach (Coord coords in old_move_variants)
            {

                //simulate new board
                temp = board[coords.Y,coords.X]; //new cell of the figure (null or enemy)
                board[coords.Y,coords.X] = board[start.Y,start.X];
                board[start.Y,start.X] = null;

                //king moves
                if (board[coords.Y,coords.X] != null && board[coords.Y,coords.X].Type == Piece.Types.king)
                {
                    if (board[coords.Y,coords.X].Side == Piece.Sides.white)
                        player1_king_coords = coords;
                    if (board[coords.Y, coords.X].Side == Piece.Sides.black)
                        player2_king_coords = coords;
                }

                for (int row = 0; row < size_y; row++)
                {
                    for (int col = 0; col < size_x; col++)
                    {
                        if (board[row,col] == null
                            || (board[row,col].Side == Piece.Sides.white && player == 1)
                            || (board[row,col].Side == Piece.Sides.black && player == 2)) continue;

                        if (KingCheck(new Coord(col, row)))
                        {
                            new_move_variants.Remove(coords);
                        }
                    }
                }

                //king moves
                if (board[coords.Y,coords.X] != null && board[coords.Y,coords.X].Type == Piece.Types.king)
                {
                    if (board[coords.Y,coords.X].Side == Piece.Sides.white)
                        player1_king_coords = start;
                    if (board[coords.Y, coords.X].Side == Piece.Sides.black)
                        player2_king_coords = start;
                }

                board[start.Y,start.X] = board[coords.Y,coords.X];
                board[coords.Y,coords.X] = temp;

            }
            move_variants = new HashSet<Coord>(new_move_variants);
        }
        public bool CheckStartCoord(Coord coords)
        {
            #nullable enable
            Piece? piece;
            #nullable disable
            piece = board[coords.Y, coords.X];
            if (piece == null) return false;
            if (piece.Side == Piece.Sides.white && player == 2) return false;
            if (piece.Side == Piece.Sides.black && player == 1) return false;
            return true;
        }
    }
}
