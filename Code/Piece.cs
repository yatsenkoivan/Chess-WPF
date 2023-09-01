using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

static class PieceImages
{
    public static readonly BitmapImage pawn_white = new BitmapImage(new Uri(@"images/pawn_white.png", UriKind.Relative));

    public static readonly BitmapImage pawn_black = new BitmapImage(new Uri(@"images/pawn_black.png", UriKind.Relative));
    
    public static readonly BitmapImage rook_white = new BitmapImage(new Uri(@"images/rook_white.png", UriKind.Relative));
    
    public static readonly BitmapImage rook_black = new BitmapImage(new Uri(@"images/rook_black.png", UriKind.Relative));      
    
    public static readonly BitmapImage knight_white = new BitmapImage(new Uri(@"images/knight_white.png", UriKind.Relative));
    
    public static readonly BitmapImage knight_black =new BitmapImage(new Uri(@"images/knight_black.png", UriKind.Relative));
    
    public static readonly BitmapImage bishop_white = new BitmapImage(new Uri(@"images/bishop_white.png", UriKind.Relative));
    
    public static readonly BitmapImage bishop_black = new BitmapImage(new Uri(@"images/bishop_black.png", UriKind.Relative));
    
    public static readonly BitmapImage king_white = new BitmapImage(new Uri(@"images/king_white.png", UriKind.Relative));
    
    public static readonly BitmapImage king_black = new BitmapImage(new Uri(@"images/king_black.png", UriKind.Relative));
    
    public static readonly BitmapImage queen_white = new BitmapImage(new Uri(@"images/queen_white.png", UriKind.Relative));
    
    public static readonly BitmapImage queen_black = new BitmapImage(new Uri(@"images/queen_black.png", UriKind.Relative));
}

namespace Chess_WPF.Code
{
    internal class Piece
    {
        public enum Types
        {
            pawn, rook, knight, bishop, king, queen
        }
        public enum Sides
        {
            white, black
        }
        #nullable enable
        public Image? img;
        #nullable disable
        public Types Type { get; set; }
        public Sides Side { get; set; }

        public Piece(Types type, Sides side)
        {
            Type = type;
            Side = side;
            img = new Image();
            UpdateImage();
        }
        public void UpdateImage()
        {
            switch (Type)
            {
                case Types.pawn:
                    if (Side == Sides.white) img.Source = PieceImages.pawn_white;
                    if (Side == Sides.black) img.Source = PieceImages.pawn_black;
                    break;
                case Types.rook:
                    if (Side == Sides.white) img.Source = PieceImages.rook_white;
                    if (Side == Sides.black) img.Source = PieceImages.rook_black;
                    break;
                case Types.knight:
                    if (Side == Sides.white) img.Source = PieceImages.knight_white;
                    if (Side == Sides.black) img.Source = PieceImages.knight_black;
                    break;
                case Types.bishop:
                    if (Side == Sides.white) img.Source = PieceImages.bishop_white;
                    if (Side == Sides.black) img.Source = PieceImages.bishop_black;
                    break;
                case Types.king:
                    if (Side == Sides.white) img.Source = PieceImages.king_white;
                    if (Side == Sides.black) img.Source = PieceImages.king_black;
                    break;
                case Types.queen:
                    if (Side == Sides.white) img.Source = PieceImages.queen_white;
                    if (Side == Sides.black) img.Source = PieceImages.queen_black;
                    break;
            }
        }   
    }
}
