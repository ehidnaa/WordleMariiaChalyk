using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wordle.Game;

namespace Wordle.Data
{
    // current user setting - used on all pages
    public class UserSetting
    {
        public string UserName { get; set; }
        public string LastVisit { get; set; }

        [JsonConverter ( typeof ( JsonStringEnumConverter ) )]
        public AppTheme Theme { get; set; }
        public string DifficultyName { get; set; } = "Easy";
        public List<OneGame> GameList { get; set; } = [];
    }

}
