using System.Collections.ObjectModel;

namespace Wordle.Game
{
    public enum RESULT { none, accept, win, lose, timeout }

    // Displays a Word list

    public class Board
    {
        public static int BOARDSIZE = 6; //can change count of words
        public static Random Rnd = new Random ();

        public ObservableCollection<Word> Words { get; } = new ();

        // get list of string (word) already filled by user
        public List<string> MyWords => Words.Select ( w => w.MyWord ).ToList ();

        // List of Words
        public List<string> WordList { get; set; }
        public string Secret { get; set; }
        internal int currentIndex { get; set; }

        public Board ()
        {
            Store store = Service.Get<Store> ();
            WordList = store.WordList.Select ( s => s.ToLower () ).ToList ();

            Secret = WordList.ElementAt ( Rnd.Next ( WordList.Count ) );

            for ( int i = 0; i < BOARDSIZE; i++ )
                Words.Add ( new Word () );
        }

        // word part analysis and return status flags
        internal void TryAcceptEntry ( EntryState entry )
        {
            string text = entry.Word?.ToLower ().Trim ();

            // case contain non letter - return without accept value
            if ( text?.All ( c => char.IsLetter ( c ) ) == false )
                return;

            if ( text?.Length < Word.WORDSIZE )
            {
                entry.Message = "enter a 5-letter word";
                entry.CanAccept = true;
            }
            else if ( text?.Length == Word.WORDSIZE )
            {
                if ( WordList.Contains ( text ) )
                {
                    if ( MyWords.Contains ( text ) )
                        entry.Message = "word already exists";
                    else
                    {
                        entry.Message = "Click OK";
                        entry.CanClickOk = true;
                    }
                }
                else
                    entry.Message = "No such word";

                entry.CanAccept = true;
            }
        }

        // full word logic: Win, Lose, continue
        internal RESULT Accept ( string text )
        {
            string word = text.Trim ().ToLower ();
            if ( word.Length == Word.WORDSIZE )
                if ( word.All ( c => char.IsLetter ( c ) ) )
                    if ( WordList.Contains ( word ) )
                        if ( !MyWords.Contains ( word ) )
                        {
                            //creating all letters for a word with status:
                            // gray orange green
                            List<Letter> letterList = [];
                            for ( int i = 0; i < word.Length; i++ )
                            {
                                char symbol = word [ i ];
                                Letter letter = new Letter () { Symbol = symbol };
                                if ( Secret [ i ] == symbol )
                                    letter.State = LETTERSTATE.green;
                                else if ( Secret.Contains ( symbol ) )
                                    letter.State = LETTERSTATE.orange;
                                else
                                    letter.State = LETTERSTATE.gray;
                                letterList.Add ( letter );
                            }
                            Words [ currentIndex++ ].AcceptLetters ( letterList );

                            if ( word == Secret )
                                return RESULT.win;

                            if ( currentIndex == BOARDSIZE )
                                return RESULT.lose;

                            return RESULT.accept;
                        }
            return RESULT.none;
        }
    }
    //--------------------------------------
    //status flags
    class EntryState
    {
        public string Word;
        public bool CanAccept;
        public string Message;
        internal bool CanClickOk;
    }
}
