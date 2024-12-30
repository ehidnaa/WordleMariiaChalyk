using System.Collections.ObjectModel;

namespace Wordle.Game
{
    // one word on Board
    // PlayPage.xaml bind to collection of Word

    // Each Word has:
    // <DataTemplate x:DataType="game:Word"> line 30 in PlayPage.xaml

    // Each Letter in Word has:
    // <DataTemplate x:DataType="game:Letter"> line 34 in PlayPage.xaml
    public class Word
    {
        public static int WORDSIZE => 5;
        public ObservableCollection<Letter> Letters { get; } = new ();

        public string MyWord => string.Join ( "", Letters.Select ( l => l.Symbol ) ).ToLower ();

        public Word ()
        {
            for ( int i = 0; i < WORDSIZE; i++ )
                Letters.Add ( new Letter () );
        }

        internal void AcceptLetters ( List<Letter> letterList )
        {
            Letters.Clear ();
            for ( int i = 0; i < letterList.Count; i++ )
            {
                Letter letter = letterList [ i ];
                letter.Symbol = letter.Symbol;
                letter.Index = i + 1;
                Letters.Add ( letter );
            }
        }
    }
}
