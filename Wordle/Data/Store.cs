using System.Text.Json;

namespace Wordle.Data
{
    // Singleton. Create once in MauiProgram.CreateMauiApp
    // Used on all paged
    // Get instanse from Service.Get<Store>()
    public class Store
    {
        public UserSetting CurrSetting { get; set; }

        public string DataDir { get; set; }
        public string UsersDir { get; set; }
        public List<string> WordList { get; set; }

        string wordListFile = "words.txt";

        string HttpWords = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";

        public Store ()
        {
            DataDir = FileSystem.Current.AppDataDirectory;
            UsersDir = Path.Combine ( DataDir, "users" );
            try
            {
                if ( !Directory.Exists ( UsersDir ) )
                    Directory.CreateDirectory ( UsersDir );
            }
            catch { }
        }

        public async Task<string> TryLoadWords ()
        {
            string wordpath = Path.Combine ( DataDir, wordListFile );
            if ( !File.Exists ( wordpath ) )
            {
                try
                {
                    HttpClient http = new ();
                    var response = await http.GetAsync ( HttpWords );
                    response.EnsureSuccessStatusCode ();
                    var words = await response.Content.ReadAsStringAsync ();
                    File.WriteAllText ( wordpath, words.ToLower () );
                }
                catch ( Exception ex ) { return ex.Message; }
            }
            try
            {
                WordList = File.ReadAllLines ( wordpath ).ToList ();
            }
            catch ( Exception ex ) { return ex.Message; }
            return null;
        }
        public string LoadSettings ( string name )
        {
            string fname = AllNames ().FirstOrDefault ( n => string.Compare ( name, n, true ) == 0 );
            UserSetting loadSett = null;
            try
            {
                string json = File.ReadAllText ( Path.Combine ( UsersDir, $"{name}.json" ) );
                loadSett = JsonSerializer.Deserialize<UserSetting> ( json );
            }
            catch ( Exception ex ) { }

            if ( loadSett == null )
            {
                if ( CheckName ( name ) )
                {
                    loadSett = new UserSetting () { UserName = name };
                    SaveSettings ();
                }
                else return "The name must contain only letters and numbers";
            }
            CurrSetting = loadSett;
            return null; //no error
        }
        public string SaveSettings ()
        {
            if ( CurrSetting == null ) return "Settings not loaded";

            try
            {
                string filename = Path.Combine ( UsersDir, $"{CurrSetting.UserName}.json" );
                using Stream fs = File.Create ( filename );
                JsonSerializer.Serialize ( fs, CurrSetting, new JsonSerializerOptions () { WriteIndented = true } );
                return null;
            }
            catch ( Exception ex ) { return ex.Message; }
        }

        // simple check for invalid characters
        internal bool CheckName ( string name )
        {
            return name.All ( c => char.IsLetterOrDigit ( c ) || c == ' ' || c == '_' );
        }

        // all user names 
        internal List<string> AllNames ()
        {
            return Directory.GetFiles ( UsersDir ).Select ( f => Path.GetFileNameWithoutExtension ( f ) ).ToList ();
        }

        // change user name from SettPage.NameClick
        internal string ChangeName ( string oldname, string newName )
        {
            if ( newName?.Length > 1 && CheckName ( newName ) )
            {
                if ( string.Compare ( oldname, newName, true ) == 0 )
                    return null; // no error

                if ( AllNames ().Any ( n => string.Compare ( n, newName, true ) == 0 ) )
                    return "This name already exists, choose another name";
            }
            else return "The name must contain only letters and numbers";
            return null;
        }

        internal void SaveGame ( OneGame game )
        {
            CurrSetting.GameList.Insert ( 0, game );
            SaveSettings ();
        }

        // predefined list of difficulties
        List<GameDifficulty> DiffList ()
        {
            return new ()
                {
                    new()
                    {
                        Name="Esay",
                        ShowWordList = true
                    },
                    new()
                    {
                        Name="Normal"
                    },
                    new()
                    {
                        Name="Hard",
                        Timeout=10
                    }
                };
        }

        // get difficulty by Name
        internal GameDifficulty GetDifficulty ( string diffName )
        {
            var diff = DiffList ().FirstOrDefault ( df => df.Name == diffName );
            if ( diff == null )
                diff = DiffList ().First ();
            return diff;
        }
    }
}
