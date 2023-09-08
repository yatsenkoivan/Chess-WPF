using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chess_WPF.Code;

static class PieceImages
{
    public static readonly BitmapImage pawn_white = new BitmapImage(new Uri(@"..\images/pawn_white.png", UriKind.Relative));

    public static readonly BitmapImage pawn_black = new BitmapImage(new Uri(@"..\images/pawn_black.png", UriKind.Relative));
                                                                              
    public static readonly BitmapImage rook_white = new BitmapImage(new Uri(@"..\images/rook_white.png", UriKind.Relative));
                                                                              
    public static readonly BitmapImage rook_black = new BitmapImage(new Uri(@"..\images/rook_black.png", UriKind.Relative));      
    
    public static readonly BitmapImage knight_white = new BitmapImage(new Uri(@"..\images/knight_white.png", UriKind.Relative));
    
    public static readonly BitmapImage knight_black =new BitmapImage(new Uri(@"..\images/knight_black.png", UriKind.Relative));
    
    public static readonly BitmapImage bishop_white = new BitmapImage(new Uri(@"..\images/bishop_white.png", UriKind.Relative));
    
    public static readonly BitmapImage bishop_black = new BitmapImage(new Uri(@"..\images/bishop_black.png", UriKind.Relative));
    
    public static readonly BitmapImage king_white = new BitmapImage(new Uri(@"..\images/king_white.png", UriKind.Relative));
    
    public static readonly BitmapImage king_black = new BitmapImage(new Uri(@"..\images/king_black.png", UriKind.Relative));
    
    public static readonly BitmapImage queen_white = new BitmapImage(new Uri(@"..\images/queen_white.png", UriKind.Relative));
    
    public static readonly BitmapImage queen_black = new BitmapImage(new Uri(@"..\images/queen_black.png", UriKind.Relative));

    #nullable enable
    public static BitmapImage? GetImage(Piece? piece)
    {
        if (piece == null) return null;
        switch (piece.Type)
        {
            case Piece.Types.pawn:
                return piece.Side == Piece.Sides.white ? pawn_white : pawn_black;
            case Piece.Types.rook:
                return piece.Side == Piece.Sides.white ? rook_white : rook_black;
            case Piece.Types.knight:
                return piece.Side == Piece.Sides.white ? knight_white : knight_black;
            case Piece.Types.bishop:
                return piece.Side == Piece.Sides.white ? bishop_white : bishop_black;
            case Piece.Types.king:
                return piece.Side == Piece.Sides.white ? king_white : king_black;
            case Piece.Types.queen:
                return piece.Side == Piece.Sides.white ? queen_white : queen_black;
        }
        return null;
    }
}

namespace Chess_WPF.Code
{
    [Serializable]
    public class Piece
    {
        public enum Types
        {
            pawn, rook, knight, bishop, king, queen
        }
        public enum Sides
        {
            white, black
        }
        public Types Type { get; set; }
        public Sides Side { get; set; }
        public Piece(Types type, Sides side)
        {
            Type = type;
            Side = side;
        } 
    }
}
