﻿using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Librame.Extensions.Imaging.Processing
{
    using Core.Combiners;
    using Drawing.Services;

    public class ScaleServiceTests
    {
        private IScaleService _service = null;

        public ScaleServiceTests()
        {
            _service = TestServiceProvider.Current.GetRequiredService<IScaleService>();
        }


        [Fact]
        public void DrawScaleTest()
        {
            // 5K 2.21MB
            var imageFile = "microsoft_edge.jpg".AsFilePathCombiner(_service.Dependency.ResourceDirectory);
            var succeed = _service.DrawFile(imageFile);
            Assert.True(succeed);
        }

        [Fact]
        public async void DrawScalesByDirectoryTest()
        {
            // 5K 2.21MB
            var directory = _service.Dependency.ResourceDirectory.CombinePath("pictures");

            // Clear
            await _service.DeleteScalesByDirectoryAsync(directory).ConfigureAwait();

            var count = await _service.DrawFilesByDirectoryAsync(directory).ConfigureAwait();
            Assert.True(count > 0);
        }

    }
}
