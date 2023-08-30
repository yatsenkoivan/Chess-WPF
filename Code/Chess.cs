using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chess_WPF.Code
{
    internal class Chess
    {
        public int player;
        #nullable enable
        public Piece[,]? board;
        #nullable disable
        public Chess()
        {
            board = new Piece[8, 8];
            player = 1;
            SetBoard();
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

            //KINGS
            board[0, 3] = new Piece(Piece.Types.king, Piece.Sides.black);
            board[7, 3] = new Piece(Piece.Types.king, Piece.Sides.white);

            //QUEENS
            board[0, 4] = new Piece(Piece.Types.queen, Piece.Sides.black);
            board[7, 4] = new Piece(Piece.Types.queen, Piece.Sides.white);
        }
    }
}
