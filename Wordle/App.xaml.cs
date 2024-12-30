
namespace Wordle
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent ();
            MainPage = new AppShell ();
        }

        protected override Window CreateWindow ( IActivationState? activationState )
        {
            var win = base.CreateWindow ( activationState );
            //#if DEBUG
            win.Width = 600;
            win.Height = 600;
            //#endif
            return win;
        }
    }
}
