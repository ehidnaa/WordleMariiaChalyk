using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wordle.Data
{
    //Description difficulty level
    // predefined levels in Store.DiffList
    // key for store and select - property Name
    public class GameDifficulty
    {
        public string Name { get; set; }
        public bool ShowWordList { get; set; }

        [JsonIgnore]
        public bool HasTimeout => Timeout > 0;
        public int Timeout { get; set; }
    }
}
