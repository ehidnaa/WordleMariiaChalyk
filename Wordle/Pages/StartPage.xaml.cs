namespace Wordle.Pages
{
    public partial class StartPage : ContentPage
    {
        string _name;
        public string UserName
        {
            get => _name;
            set
            {
                _name = value;
                OkComm.ChangeCanExecute ();
                OnPropertyChanged ();
            }
        }

        public Command OkComm { get; set; }

        Store Store;

        public StartPage ()
        {
            OkComm = new Command ( OkExe, () => UserName?.Count () > 1 );
            InitializeComponent ();
            Store = Service.Get<Store> ();
            LoadWords ();
        }

        async void LoadWords ()
        {
            string err = await Store.TryLoadWords ();
            if ( err != null )
                await DisplayAlert ( "Words", "Can't load words from HTTP", "OK" );
        }
        protected override void OnAppearing ()
        {
            base.OnAppearing ();
            if ( Store.CurrSetting != null )
                UserName = Store.CurrSetting.UserName;
        }
        async void OkExe ()
        {
            if ( Store.CheckName ( UserName ) )
            {
                string error = Store.LoadSettings ( UserName );
                if ( error == null )
                {
                    Application.Current.UserAppTheme = Store.CurrSetting.Theme;
                    await Shell.Current.GoToAsync ( "Menu" );
                    return;
                }
            }
            await DisplayAlert ( "Name", "The name must contain only letters and numbers", "Close" );
        }
    }

}
