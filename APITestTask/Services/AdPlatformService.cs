namespace APITestTask.Services
{
    public class AdPlatformService : IAdPlatformService
    {
        private readonly Dictionary<string, HashSet<string>> _locationIndex = new();
        private readonly object _lock = new object();

        public async Task LoadAdPlatformsFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var content = await File.ReadAllTextAsync(filePath);
            await LoadAdPlatformsFromContentAsync(content);
        }

        public Task LoadAdPlatformsFromContentAsync(string content)
        {
            var newLocationIndex = new Dictionary<string, HashSet<string>>();

            try
            {
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                        continue;

                    var parts = trimmedLine.Split(':', 2);
                    if (parts.Length != 2)
                        continue;

                    var platformName = parts[0].Trim();
                    var locationsStr = parts[1].Trim();

                    if (string.IsNullOrEmpty(platformName) || string.IsNullOrEmpty(locationsStr))
                        continue;

                    var locations = locationsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(loc => loc.Trim())
                        .Where(loc => !string.IsNullOrEmpty(loc))
                        .ToList();

                    if (locations.Any())
                    {
                        foreach (var location in locations)
                        {
                            if (!newLocationIndex.ContainsKey(location))
                                newLocationIndex[location] = new HashSet<string>();
                            
                            newLocationIndex[location].Add(platformName);
                        }
                    }
                }

                lock (_lock)
                {
                    _locationIndex.Clear();
                    foreach (var kvp in newLocationIndex)
                        _locationIndex[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error parsing content: {ex.Message}", ex);
            }

            return Task.CompletedTask;
        }

        public List<string> FindAdPlatformsForLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return new List<string>();

            var result = new HashSet<string>();

            lock (_lock)
            {
                var locationParts = location.Split('/');
                
                for (int i = locationParts.Length; i > 0; i--)
                {
                    var currentLocation = string.Join("/", locationParts.Take(i));
                    
                    if (_locationIndex.TryGetValue(currentLocation, out var platforms))
                    {
                        foreach (var platform in platforms)
                            result.Add(platform);
                    }
                }
            }

            return result.ToList();
        }
    }
}
