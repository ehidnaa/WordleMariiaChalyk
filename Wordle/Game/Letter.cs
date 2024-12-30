namespace Wordle.Game
{
    public enum LETTERSTATE { none, gray, orange, green }
    // one letter in word

    // State - key property for coloring. 
    // In PlayPage.xaml - Border x:Name="br" has DataTriggers binding to this State

    // Each Letter in Word has:
    // <DataTemplate x:DataType="game:Letter"> line 34 in PlayPage.xaml

    public class Letter
    {      
        public char Symbol { get; set;}
        public LETTERSTATE State { get; set; }
        public int Index { get; set; }
    }
}
