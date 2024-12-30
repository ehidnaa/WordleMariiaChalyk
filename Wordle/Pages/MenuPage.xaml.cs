namespace Wordle.Pages;
public partial class MenuPage : ContentPage
{
    string _username;
    public string UserName { get => _username; set { _username = value; OnPropertyChanged (); } }
    public string LastVisit { get; set; }

    Store store;

    public MenuPage ()
    {
        store = Service.Get<Store> ();
        UserName = store.CurrSetting.UserName;

        if ( store.CurrSetting.LastVisit == null )
            LastVisit = "Your first visit";
        else
            LastVisit = "Last visited: " + store.CurrSetting.LastVisit;

        store.CurrSetting.LastVisit = DateTime.Now.ToString ( "yyyy-MM-dd HH:mm" );
        store.SaveSettings ();

        InitializeComponent ();

#if DEBUG
        //        Dispatcher.DispatchAsync ( PlayExe );
#endif
    }
    protected override void OnAppearing ()
    {
        base.OnAppearing ();
        UserName = store.CurrSetting.UserName;
    }

    private void PlayClick ( object sender, EventArgs e )
    {
        Shell.Current.GoToAsync ( "Play" );
    }

    private void GameSettClick ( object sender, EventArgs e )
    {
        Shell.Current.GoToAsync ( "GameSett" );
    }

    private void ExitClick ( object sender, EventArgs e )
    {
        App.Current.Quit ();
    }

    private void SettClick ( object sender, EventArgs e )
    {
        Shell.Current.GoToAsync ( "Settings" );
    }
}