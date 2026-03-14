namespace VocabTrainer.Api.Settings
{
    public class StorageSettings
    {
        public const string SectionName = "Storage";
        public string Provider { get; set; } = "Local";
        public string LocalPath { get; set; } = "wwwroot";
    }
}