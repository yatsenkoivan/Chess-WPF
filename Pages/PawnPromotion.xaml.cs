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
using System.Windows.Shapes;
using Chess_WPF.Code;

namespace Chess_WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для PawnPromotion.xaml
    /// </summary>
    public partial class PawnPromotion : Window
    {
        public PawnPromotion()
        {
            InitializeComponent();
            QueenButton.IsChecked = true;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        public Piece.Types GetPieceType()
        {
            if (QueenButton.IsChecked == true) return Piece.Types.queen;
            if (RookButton.IsChecked == true) return Piece.Types.rook;
            if (BishopButton.IsChecked == true) return Piece.Types.bishop;
            if (KnightButton.IsChecked == true) return Piece.Types.knight;
            return Piece.Types.queen;
        }
    }
}
