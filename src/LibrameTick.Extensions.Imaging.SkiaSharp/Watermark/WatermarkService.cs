﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Librame.Extensions.Imaging.Watermark
{
    using Core.Builders;
    using Core.Combiners;
    using Core.Services;
    using Drawing.Builders;
    using Drawing.Resources;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class WatermarkService : AbstractExtensionBuilderService<DrawingBuilderOptions>, IWatermarkService
    {
        private readonly FilePathCombiner _fontFilePath;
        private readonly FilePathCombiner _watermarkImagePath;


        public WatermarkService(DrawingBuilderDependency dependency, ILoggerFactory loggerFactory)
            : base(dependency?.Options, loggerFactory)
        {
            Dependency = dependency;

            _watermarkImagePath = Options.Watermark.ImagePath
                .ChangeBasePathIfEmpty(dependency.ResourceDirectory);

            _fontFilePath = Options.Watermark.Font.FilePath
                .ChangeBasePathIfEmpty(dependency.ResourceDirectory);
        }


        public IExtensionBuilderDependency Dependency { get; }

        public SKEncodedImageFormat CurrentImageFormat
            => Options.ImageFormat.MatchEnum<ImageFormat, SKEncodedImageFormat>();


        public Task<bool> DrawFileAsync(string imagePath, string savePath,
            WatermarkMode mode = WatermarkMode.Text, CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                var result = false;

                DrawCore(imagePath, mode, data =>
                {
                    using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        data.SaveTo(fs);
                    }

                    Logger.LogDebug($"Watermark image file save as: {savePath}");
                    result = true;
                });

                return result;
            });
        }


        public Task<bool> DrawStreamAsync(string imagePath, Stream target,
            WatermarkMode mode = WatermarkMode.Text, CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                var result = false;

                DrawCore(imagePath, mode, data =>
                {
                    data.SaveTo(target);

                    Logger.LogDebug($"Watermark image file save as stream");
                    result = true;
                });

                return result;
            });
        }


        public Task<byte[]> DrawBytesAsync(string imagePath, WatermarkMode mode = WatermarkMode.Text,
            CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                var buffer = default(byte[]);

                DrawCore(imagePath, mode, data =>
                {
                    buffer = data.ToArray();
                    Logger.LogDebug($"Watermark image file save as byte[]: length={buffer.Length}");
                });

                return buffer;
            });
        }


        public void DrawCore(string imagePath, WatermarkMode mode, Action<SKData> postAction)
        {
            Logger.LogDebug($"Watermark image file: {imagePath}");
            Logger.LogDebug($"Watermark mode: {mode.AsEnumName()}");

            using (var bmp = SKBitmap.Decode(imagePath))
            {
                using (var canvas = new SKCanvas(bmp))
                {
                    var imageSize = new Size(bmp.Width, bmp.Height);

                    DrawCore(canvas, imageSize, mode);
                }

                using (var img = SKImage.FromBitmap(bmp))
                using (var data = img.Encode(CurrentImageFormat, Options.Quality))
                {
                    if (data.IsNull())
                        throw new InvalidOperationException(InternalResource.InvalidOperationExceptionUnsupportedImageFormat);

                    postAction.Invoke(data);
                }
            }
        }


        internal void DrawCore(SKCanvas canvas, Size imageSize, WatermarkMode mode)
        {
            var coordinate = ImageHelper.CalculateCoordinate(imageSize,
                Options.Watermark.Location, Options.Watermark.IsRandom);
            
            switch (mode)
            {
                case WatermarkMode.Text:
                    {
                        using (var foreFont = CreatePaint(Options.Watermark.Colors.Fore))
                        using (var alternFont = CreatePaint(Options.Watermark.Colors.Alternate))
                        {
                            var text = Options.Watermark.Text;

                            for (int i = 0; i < text.Length; i++)
                            {
                                // 当前字符
                                var character = text.Substring(i, 1);

                                // 测算水印文本内容矩形尺寸
                                var rect = new SKRect();
                                foreFont.MeasureText(character, ref rect);

                                // 绘制文本水印
                                canvas.DrawText(character, coordinate.X + (int)rect.Width, coordinate.Y,
                                    i % 2 > 0 ? alternFont : foreFont);

                                // 递增字符宽度
                                coordinate.X += (int)rect.Width;
                            }
                        }
                    }
                    break;

                case WatermarkMode.Image:
                    {
                        using (var watermark = SKBitmap.Decode(_watermarkImagePath))
                        {
                            // 绘制图像水印
                            canvas.DrawBitmap(watermark, coordinate.X, coordinate.Y);
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        

        private SKPaint CreatePaint(SKColor color)
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = color;
            // paint.StrokeCap = SKStrokeCap.Round;
            paint.Typeface = SKTypeface.FromFile(_fontFilePath);
            paint.TextSize = Options.Watermark.Font.Size;

            return paint;
        }

    }
}
