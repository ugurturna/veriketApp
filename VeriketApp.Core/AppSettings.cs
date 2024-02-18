namespace VeriketApp.Core
{
    public class AppSettings
    {
        public string ServiceName { get; set; }
        public string LogFileName { get; set; }
        public int LogPeriodMinute { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string BaseFilePath { get => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); }
    }
}
