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
using System.Drawing;

namespace Librame.Extensions.Drawing.Verification
{
    internal class InternalCaptchaGenerator : ICaptchaGenerator
    {
        private readonly FilePathCombiner _fontFilePath;


        public InternalCaptchaGenerator(DrawingBuilderDependency dependency, ILoggerFactory loggerFactory)
            : base(dependency?.Options, loggerFactory)
        {
            Dependency = dependency;

            _fontFilePath = Options.Captcha.Font.FilePath
                .ChangeBasePathIfEmpty(dependency.ResourceDirectory);
        }


        public IExtensionBuilderDependency Dependency { get; }

        

        public SKEncodedImageFormat CurrentImageFormat
            => Options.ImageFormat.MatchEnum<ImageFormat, SKEncodedImageFormat>();


        public Task<bool> DrawFileAsync(string captcha, string savePath, CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                DrawCore(captcha, data =>
                {
                    using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        data.SaveTo(fs);
                    }

                    Logger.LogInformation($"Captcha image file save as: {savePath}");
                });

                return File.Exists(savePath);
            });
        }


        public Task<bool> DrawStreamAsync(string captcha, Stream target, CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                DrawCore(captcha, data =>
                {
                    data.SaveTo(target);

                    Logger.LogInformation($"Captcha image save as stream");
                });

                return true;
            });
        }


        public Task<byte[]> DrawBytesAsync(string captcha, CancellationToken cancellationToken = default)
        {
            return cancellationToken.RunOrCancelAsync(() =>
            {
                var buffer = default(byte[]);

                DrawCore(captcha, data =>
                {
                    buffer = data.ToArray();
                    Logger.LogDebug($"Captcha image save as byte[]: length={buffer.Length}");
                });

                return buffer;
            });
        }


        public void DrawCore(string captcha, Action<SKData> postAction)
        {
            Logger.LogInformation($"Captcha text: {captcha}");

            var colors = Options.Captcha.Colors;

            var sizeAndPoints = ComputeSizeAndPoints(captcha);
            var imageSize = sizeAndPoints.Size;
            var imageInfo = new SKImageInfo(imageSize.Width, imageSize.Height,
                SKColorType.Bgra8888, SKAlphaType.Premul);

            using (var bmp = new SKBitmap(imageInfo))
            using (var canvas = new SKCanvas(bmp))
            {
                // Clear
                canvas.DrawColor(colors.Background);

                // 绘制噪点
                using (var noisePaint = CreateNoisePaint())
                {
                    var points = CreateNoisePoints(imageSize);
                    canvas.DrawPoints(SKPointMode.Points, points, noisePaint);
                }

                // 绘制验证码
                using (var forePaint = CreatePaint(colors.Fore))
                using (var alternPaint = CreatePaint(colors.Alternate))
                {
                    foreach (var p in sizeAndPoints.Points)
                    {
                        var i = p.Key;
                        var character = p.Value.Key;
                        var point = p.Value.Value;

                        canvas.DrawText(character, point.X, point.Y,
                            i % 2 > 0 ? alternPaint : forePaint);
                    }
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


        private (Size Size, IDictionary<int, KeyValuePair<string, SKPoint>> Points) ComputeSizeAndPoints(string captcha)
        {
            var points = new Dictionary<int, KeyValuePair<string, SKPoint>>();
            var size = new Size();

            using (var foreFont = CreatePaint(Options.Captcha.Colors.Fore))
            {
                var paddingHeight = (int)foreFont.TextSize;
                var paddingWidth = paddingHeight / 2;
                
                var startX = paddingWidth;
                var startY = paddingHeight;

                for (int i = 0; i < captcha.Length; i++)
                {
                    // 当前字符
                    var character = captcha.Substring(i, 1);

                    // 测算字符矩形
                    var rect = new SKRect();
                    foreFont.MeasureText(character, ref rect);

                    // 当前字符宽高
                    var charWidth = (int)rect.Width;
                    var charHeight = (int)rect.Height;
                    
                    var point = new SKPoint();

                    // 随机变换其余字符坐标
                    RandomUtility.Run(r =>
                    {
                        point.X = r.Next(startX, charWidth + startX);
                        point.Y = r.Next(startY, charHeight + startY);
                    });
                    
                    // 附加为字符宽度加当前字符横坐标
                    startX = (int)point.X + charWidth + paddingWidth;

                    points.Add(i, new KeyValuePair<string, SKPoint>(character, point));
                }

                size.Width += startX + paddingWidth;
                size.Height += startY + paddingHeight;
            }

            return (size, points);
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

        private SKPaint CreateNoisePaint()
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = Options.Captcha.Colors.Disturbing;
            paint.StrokeCap = SKStrokeCap.Square;
            paint.StrokeWidth = Options.Captcha.Noise.Width;

            return paint;
        }

        private SKPoint[] CreateNoisePoints(Size imageSize)
        {
            var noises = Options.Captcha.Noise;

            var points = new List<SKPoint>();

            var offset = noises.Width;
            var xCount = imageSize.Width / noises.Space.X + offset;
            var yCount = imageSize.Height / noises.Space.Y + offset;

            for (int i = 0; i < xCount; i++)
            for (int j = 0; j < yCount; j++)
            {
                var point = new SKPoint(i * noises.Space.X, j * noises.Space.Y);
                points.Add(point);
            }

            return points.ToArray();
        }

    }
}
