using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wordle.Game;

namespace Wordle.Data
{
    // created in PlayPage.NewGameExe    
    // Gameover in PlayPage.OkExe - store to UserSetting.GameList
    public class OneGame
    {
        public string Word { get; set; }
        public string Date { get; set; }
        public double WasteTime { get; set; }
        public int Steps { get; set; }
        public string Difficulty { get; set; } = "Easy";

        [JsonConverter ( typeof ( JsonStringEnumConverter ) )]
        public RESULT Result { get; set; }

        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public string WasteString => TimeSpan.FromMinutes ( WasteTime ).ToString ( "m\\:ss" );

        public OneGame () { }
        public OneGame ( string secret )
        {
            Word = secret;
            StartTime = DateTime.Now;
            Date = StartTime.ToString ( "yyyy-MM-dd HH:mm" );
        }
    }

}
