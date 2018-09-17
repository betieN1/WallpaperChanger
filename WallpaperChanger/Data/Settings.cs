namespace WallpaperChanger.Data
{
    public class Settings
    {
        public Settings()
        {
            ContentType = WallpaperContentType.Technology;
            SettingsType = SettingsType.Technology;
        }

        public WallpaperContentType ContentType { get; set; }

        public SettingsType SettingsType { get; set; }

        public int CurrentImageMonth { get; set; }
    }
}
