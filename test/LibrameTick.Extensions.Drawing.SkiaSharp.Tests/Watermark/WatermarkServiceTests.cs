﻿using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Librame.Extensions.Drawing.Watermarking
{
    using Core.Combiners;
    using Drawing.Services;

    public class WatermarkServiceTests
    {
        private IWatermarkGenerator _service = null;

        public WatermarkServiceTests()
        {
            _service = TestServiceProvider.Current.GetRequiredService<IWatermarkGenerator>();
        }
        

        [Fact]
        public async void DrawWatermarkTest()
        {
            // 5K 2.21MB
            var imageFile = "microsoft_edge.jpg".AsFilePathCombiner(_service.Dependency.ResourceDirectory);
            var saveFile = imageFile.WithFileName("microsoft_edge-watermark.png");
            
            var succeed = await _service.DrawFileAsync(imageFile, saveFile).ConfigureAwait();
            Assert.True(succeed);
        }

    }
}
