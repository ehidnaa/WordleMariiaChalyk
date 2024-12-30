namespace Wordle.Pages;

public partial class GameSettPage : ContentPage
{
    string _selectedDiff;
    public string SelectedDiff { get => _selectedDiff; set { _selectedDiff = value; OnPropertyChanged (); } }

    UserSetting _sett;
    public UserSetting CurrSetting { get => _sett; set { _sett = value; OnPropertyChanged (); } }

    Store store;
    public GameSettPage ()
    {
        InitializeComponent ();
        store = Service.Get<Store> ();
    }
    protected override void OnAppearing ()
    {
        base.OnAppearing ();
        CurrSetting = null; // full update
        SelectedDiff = store.CurrSetting.DifficultyName;
        CurrSetting = store.CurrSetting;
    }

    private async void cancelClick ( object sender, EventArgs e )
    {
        await Shell.Current.GoToAsync ( "../" );
    }
    private async void saveClick ( object sender, EventArgs e )
    {
        store.CurrSetting.DifficultyName = SelectedDiff;
        store.SaveSettings ();
        await Shell.Current.GoToAsync ( "../" );
    }

    async void ClearClick ( object sender, EventArgs e )
    {        
        string result = await DisplayActionSheet ( "Remove statistics", null, null, FlowDirection.LeftToRight, [ "Yes", "No" ] );
        if ( result == "Yes" )
        {
            CurrSetting.GameList.Clear ();
            store.SaveSettings ();
            CurrSetting = null; // full update
            CurrSetting = store.CurrSetting;
        }
    }
}