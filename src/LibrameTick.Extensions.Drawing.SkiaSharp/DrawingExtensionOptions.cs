﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Core;
using Librame.Extensions.Drawing.Verification;

namespace Librame.Extensions.Drawing;

/// <summary>
/// 图画扩展选项。
/// </summary>
public class DrawingExtensionOptions : AbstractExtensionOptions<DrawingExtensionOptions>
{
    /// <summary>
    /// 构造一个 <see cref="DrawingExtensionOptions"/>。
    /// </summary>
    public DrawingExtensionOptions()
    {
        Colors = ColorOptions.CreateLightOptions(Notifier);
        Captcha = new(Notifier);
        Scale = new(Notifier);
        Watermark = new(Notifier);
    }


    /// <summary>
    /// 色彩选项。
    /// </summary>
    public ColorOptions Colors { get; init; }

    /// <summary>
    /// 验证码选项。
    /// </summary>
    public CaptchaOptions Captcha { get; init; }

    /// <summary>
    /// 缩放选项。
    /// </summary>
    public ScaleOptions Scale { get; init; }

    /// <summary>
    /// 水印选项。
    /// </summary>
    public WatermarkOptions Watermark { get; init; }


    /// <summary>
    /// 图像目录（主要用于存储图像）。
    /// </summary>
    public string ImageDirectory
    {
        get => Notifier.GetOrAdd(nameof(ImageDirectory), Directories.ResourceDirectory.CombinePath("images"));
        set => Notifier.AddOrUpdate(nameof(ImageDirectory), value);
    }

    /// <summary>
    /// 图像格式（默认为“jpeg”）。
    /// </summary>
    public string ImageFormat
    {
        get => Notifier.GetOrAdd(nameof(ImageFormat), "jpeg");
        set => Notifier.AddOrUpdate(nameof(ImageFormat), value);
    }

    /// <summary>
    /// 编码品质（取值范围：1-100；默认为 80）。
    /// </summary>
    public int EncodeQuality
    {
        get => Notifier.GetOrAdd(nameof(EncodeQuality), 80);
        set => Notifier.AddOrUpdate(nameof(EncodeQuality), value);
    }

    /// <summary>
    /// 调整尺寸品质。
    /// </summary>
    public SKFilterQuality ResizeQuality
    {
        get => Notifier.GetOrAdd(nameof(ResizeQuality), SKFilterQuality.Medium);
        set => Notifier.AddOrUpdate(nameof(ResizeQuality), value);
    }

    /// <summary>
    /// 支持的图像扩展名列表。
    /// </summary>
    public List<string> SupportImageExtensions { get; init; } = new()
    {
        ".bmp",
        ".jpg",
        ".jpeg",
        ".png"
    };

}
