
using Assets.Scripts.Data.Literals;

public class OptionsModel
{
    public const string PreferencesInfoFile = "/optionsPreferences.json";
    public enum LeftRightEnum {Left, Right };
    public float MusicPref { get; set; }
    public float SoundEffectsPref { get; set; }
    public LeftRightEnum SelectTroopPref { get; set; }
    public LeftRightEnum MoveAttackPref { get; set; }
    public string Language { get; set; }
    
    public string toString()
    {
        return $"Preferences: Music Volume = {MusicPref}, Sound Effects Volume = {SoundEffectsPref}, Select Troop = {SelectTroopPref}, Move/Attack = {MoveAttackPref}"; 
    }
}
