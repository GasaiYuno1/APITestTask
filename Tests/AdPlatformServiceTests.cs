using Xunit;
using APITestTask.Services;

namespace APITestTask.Tests
{
    public class AdPlatformServiceTests
    {
        private readonly IAdPlatformService _service;

        public AdPlatformServiceTests()
        {
            _service = new AdPlatformService();
        }

        [Fact]
        public async Task LoadAdPlatformsFromContent_ValidContent_LoadsCorrectly()
        {
            var content = @"Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
Крутая реклама:/ru/svrd";

            await _service.LoadAdPlatformsFromContentAsync(content);

            var ruPlatforms = _service.FindAdPlatformsForLocation("/ru");
            Assert.Single(ruPlatforms);
            Assert.Contains("Яндекс.Директ", ruPlatforms);
        }

        [Fact]
        public async Task FindAdPlatformsForLocation_MoscowLocation_ReturnsCorrectPlatforms()
        {
            var content = @"Яндекс.Директ:/ru
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl";

            await _service.LoadAdPlatformsFromContentAsync(content);

            var platforms = _service.FindAdPlatformsForLocation("/ru/msk");

            Assert.Equal(2, platforms.Count);
            Assert.Contains("Яндекс.Директ", platforms);
            Assert.Contains("Газета уральских москвичей", platforms);
        }

        [Fact]
        public async Task FindAdPlatformsForLocation_SverdlovskLocation_ReturnsCorrectPlatforms()
        {
            var content = @"Яндекс.Директ:/ru
Крутая реклама:/ru/svrd";

            await _service.LoadAdPlatformsFromContentAsync(content);

            var platforms = _service.FindAdPlatformsForLocation("/ru/svrd");

            Assert.Equal(2, platforms.Count);
            Assert.Contains("Яндекс.Директ", platforms);
            Assert.Contains("Крутая реклама", platforms);
        }

        [Fact]
        public async Task FindAdPlatformsForLocation_RevdaLocation_ReturnsCorrectPlatforms()
        {
            var content = @"Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
Крутая реклама:/ru/svrd";

            await _service.LoadAdPlatformsFromContentAsync(content);

            var platforms = _service.FindAdPlatformsForLocation("/ru/svrd/revda");

            Assert.Equal(3, platforms.Count);
            Assert.Contains("Яндекс.Директ", platforms);
            Assert.Contains("Ревдинский рабочий", platforms);
            Assert.Contains("Крутая реклама", platforms);
        }

        [Fact]
        public async Task FindAdPlatformsForLocation_NonExistentLocation_ReturnsEmptyList()
        {
            var content = @"Яндекс.Директ:/ru";
            await _service.LoadAdPlatformsFromContentAsync(content);

            var platforms = _service.FindAdPlatformsForLocation("/us/ny");

            Assert.Empty(platforms);
        }

        [Fact]
        public async Task LoadAdPlatformsFromContent_EmptyContent_DoesNotThrow()
        {
            await _service.LoadAdPlatformsFromContentAsync("");
            var platforms = _service.FindAdPlatformsForLocation("/ru");
            Assert.Empty(platforms);
        }

        [Fact]
        public async Task LoadAdPlatformsFromContent_InvalidFormat_SkipsInvalidLines()
        {
            var content = @"Яндекс.Директ:/ru
                            InvalidLine
                            Another Invalid Line Without Colon
Valid Platform:/ru/test";

            await _service.LoadAdPlatformsFromContentAsync(content);

            var ruPlatforms = _service.FindAdPlatformsForLocation("/ru");
            Assert.Single(ruPlatforms);
            Assert.Contains("Яндекс.Директ", ruPlatforms);

            var testPlatforms = _service.FindAdPlatformsForLocation("/ru/test");
            Assert.Equal(2, testPlatforms.Count);
            Assert.Contains("Яндекс.Директ", testPlatforms);
            Assert.Contains("Valid Platform", testPlatforms);
        }

        [Fact]
        public void FindAdPlatformsForLocation_NullOrEmptyLocation_ReturnsEmptyList()
        {
            var platforms1 = _service.FindAdPlatformsForLocation(null);
            var platforms2 = _service.FindAdPlatformsForLocation("");
            var platforms3 = _service.FindAdPlatformsForLocation("   ");

            Assert.Empty(platforms1);
            Assert.Empty(platforms2);
            Assert.Empty(platforms3);
        }
    }
}
