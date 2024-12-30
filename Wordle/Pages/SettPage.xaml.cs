global using Wordle.Data;

namespace Wordle.Pages;

public partial class SettPage : ContentPage
{
    string _username;
    public string UserName { get => _username; set { _username = value; OnPropertyChanged (); } }

    private AppTheme _selectedTheme;
    public AppTheme SelectedTheme
    {
        get => _selectedTheme;
        set { _selectedTheme = value; OnPropertyChanged (); Application.Current.UserAppTheme = value; }
    }

    Store store;
    UserSetting Setting => store.CurrSetting;
    public SettPage ()
    {
        store = Service.Get<Store> ();
        InitializeComponent ();
    }
    protected override void OnAppearing ()
    {
        base.OnAppearing ();
        UserName = Setting.UserName;
        SelectedTheme = Setting.Theme;
    }
    protected override bool OnBackButtonPressed ()
    {
        ExitClick ( null, null );
        return base.OnBackButtonPressed ();
    }
    private void ExitClick ( object sender, EventArgs e )
    {
        Application.Current.UserAppTheme = Setting.Theme;
        Shell.Current.GoToAsync ( "../" );
    }

    private void SaveClick ( object sender, EventArgs e )
    {
        Setting.UserName = UserName;
        Setting.Theme = SelectedTheme;
        store.SaveSettings ();
        Shell.Current.GoToAsync ( "../" );
    }

    private async void NameClick ( object sender, EventArgs e )
    {
        string name = await DisplayPromptAsync ( "User Name", "New Name", initialValue: UserName );

        if ( name != null )
        {
            string error = store.ChangeName ( UserName, name );
            if ( error != null )
                await DisplayAlert ( "Name", error, "Close" );
            else
                UserName = name;
        }
    }
}