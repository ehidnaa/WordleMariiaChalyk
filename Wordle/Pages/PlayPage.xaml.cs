using Wordle.Game;

namespace Wordle.Pages;

public partial class PlayPage : ContentPage
{
    // new board for each new game
    Board _board;
    public Board Board { get => _board; set { _board = value; OnPropertyChanged (); } }

    // button Ok. Command can be disabled
    public Command OkComm { get; }

    // current status - Playing Win Lose
    // affect the bindings in PlayPage.xaml
    private bool _isplaying;
    public bool IsPlaying { get => _isplaying; set { _isplaying = value; OnPropertyChanged (); OnPropertyChanged ( nameof ( IsPlayOver ) ); OnPropertyChanged ( nameof ( IsWin ) ); OnPropertyChanged ( nameof ( IsLose ) ); } }
    public bool IsPlayOver => !IsPlaying;
    public bool IsWin { get; set; }
    public bool IsLose { get; set; }

    //selection in WordList (on Easy level)
    public string SelectedItem { set { UserEntry = value; } }

    // entry word in TextBox
    private string _userentry;
    public string UserEntry { get => _userentry; set { if ( value is null ) return; if ( AcceptEntry ( value ) ) _userentry = value; OnPropertyChanged (); } }

    // flag for diasable or enable button OK
    bool _canclickok;
    public bool CanClickOk { get => _canclickok; set { _canclickok = value; OnPropertyChanged (); OkComm.ChangeCanExecute (); } }

    // message from EntryState filled in Board.TryAcceptEntry
    private string _entrymessage;
    public string EntryMessage
    {
        get { return _entrymessage; }
        set { _entrymessage = value; _entrymessage = value; OnPropertyChanged (); }
    }

    // current Level
    private GameDifficulty _currDiff;
    public GameDifficulty CurrDiff { get => _currDiff; set { _currDiff = value; OnPropertyChanged (); } }

    // Timeout (Hard level) - display timer info
    private string _timeout;
    public string Timeout { get => _timeout; set { _timeout = value; OnPropertyChanged (); } }

    // show Secret ONLY FOR Debug !!!
    public bool SHOW_SECRET { get; set; }

    Store store;
    OneGame currentGame;

    //-------------Constructor---------------------
    public PlayPage ()
    {
        OkComm = new Command ( () => OkExe (), () => CanClickOk );

#if DEBUG
        //SHOW_SECRET = true;
#endif
        InitializeComponent ();
        store = Service.Get<Store> ();
        CurrDiff = store.GetDifficulty ( store.CurrSetting.DifficultyName );

        NewGameExe ();
    }
    protected override void OnAppearing ()
    {
        base.OnAppearing ();
        CurrDiff = store.GetDifficulty ( store.CurrSetting.DifficultyName );
    }
    protected override void OnDisappearing ()
    {
        base.OnDisappearing ();
        IsPlaying = false;
    }
    void NewGameExe ()
    {
        CurrDiff = store.GetDifficulty ( store.CurrSetting.DifficultyName );
        // create new Board and select new Secret word
        Board = new Board ();

        // set flags for start
        IsWin = IsLose = false;
        IsPlaying = true;
        currentGame = new OneGame ( Board.Secret );
        GoTimer ();
    }

    // pressed Ok button - analyze word with RESULT
    async void OkExe ( bool timeout = false ) // OK Clicked - full word try accept
    {
        RESULT result = RESULT.none;
        if ( timeout )
            result = RESULT.timeout;
        else
            result = Board.Accept ( UserEntry );

        switch ( result )
        {
            case RESULT.win:
            case RESULT.lose:
            case RESULT.timeout:
                await Task.Delay ( 1500 ); // wait animation ended              
                IsWin = result == RESULT.win;
                IsLose = result == RESULT.lose || result == RESULT.timeout;
                IsPlaying = false;

                currentGame.Result = result;
                currentGame.Steps = Board.currentIndex;
                currentGame.Difficulty = CurrDiff.Name;
                currentGame.WasteTime = ( DateTime.Now - currentGame.StartTime ).TotalMinutes;
                store.SaveGame ( currentGame );

                break;
        }
        UserEntry = "";
    }

    bool AcceptEntry ( string value ) // part of word - try correct letters
    {
        // flags
        EntryState entry = new EntryState () { Word = value };
        // set flags depending on value
        Board.TryAcceptEntry ( entry );
        // check flags:
        if ( entry.CanAccept )
            cv.ScrollTo ( findNearScrollTo ( value ), ScrollToPosition.Center, true );

        if ( entry.Message != null )
            EntryMessage = entry.Message;

        CanClickOk = entry.CanClickOk;

        return entry.CanAccept;
    }

    void NewGameClick ( object sender, EventArgs e )
    {
        NewGameExe ();
    }
    void ExitClick ( object sender, EventArgs e )
    {
        Shell.Current.GoToAsync ( "../" );
    }
    void SettClick ( object sender, EventArgs e )
    {
        Shell.Current.GoToAsync ( "GameSett" );
    }

    void GoTimer ()
    {
        if ( CurrDiff.HasTimeout )
        {
            DateTime start = DateTime.Now;
            TimeSpan limit = TimeSpan.FromMinutes ( CurrDiff.Timeout );

            Task.Run ( async () =>
            {
                while ( IsPlaying )
                {
                    await Task.Delay ( 1000 );
                    TimeSpan waste = DateTime.Now - start;
                    if ( waste > limit )
                    {
                        // dispatch to main thread
                        Dispatcher.Dispatch ( () => OkExe ( true ) );
                        Timeout = "Timeout";
                        return;
                    }
                    Timeout = ( limit - waste ).ToString ( "mm\\:ss" );
                };
            } );
        }
        else Timeout = "";
    }

    // for WordList (Easy level) scroll to nearest
    string findNearScrollTo ( string entry )
    {
        for ( int i = entry.Length - 1; i >= 0; i-- )
        {
            var found = Board.WordList.FirstOrDefault ( w => w.StartsWith ( entry.ToLower () ) );
            if ( found != null )
                return found;
        }
        return null;
    }

}

// Animation apply in: Border(x:Name="br") SemanticProperties.Hint="{Binding Index}"
// Index is order of Letter in Word.Letters
// make delay in order of index
public class LetterLoadedTrigger : TriggerAction<Border>
{
    protected override async void Invoke ( Border br )
    {
        string index = SemanticProperties.GetHint ( br );
        await Task.Delay ( int.Parse ( index ) * 100 );// delay grow with indes
        await br.ScaleTo ( 1, 400 );
        await br.FadeTo ( 1, 600 );
    }
}