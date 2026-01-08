// CounterCounter/Models/AppSettings.cs
namespace CounterCounter.Models
{
    public class AppSettings
    {
        public string? LastOpenedFilePath { get; set; }

        public AppSettings()
        {
            LastOpenedFilePath = null;
        }
    }
}