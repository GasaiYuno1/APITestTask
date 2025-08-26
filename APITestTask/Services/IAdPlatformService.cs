namespace APITestTask.Services
{
    public interface IAdPlatformService
    {
        Task LoadAdPlatformsFromFileAsync(string filePath);
        Task LoadAdPlatformsFromContentAsync(string content);
        List<string> FindAdPlatformsForLocation(string? location);
    }
}
