﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using SkiaSharp;

namespace Librame.Extensions.Imaging
{
    using Core.Serializers;

    /// <summary>
    /// SK 颜色选项。
    /// </summary>
    public class SKColorOptions
    {
        /// <summary>
        /// 前景色。
        /// </summary>
        public SerializableString<SKColor> Fore { get; set; }
            = new SerializableString<SKColor>(SKColorThemes.ForeColor);

        /// <summary>
        /// 交替色。
        /// </summary>
        public SerializableString<SKColor> Alternate { get; set; }
            = new SerializableString<SKColor>(SKColorThemes.ForeColor);

        /// <summary>
        /// 背景色。
        /// </summary>
        public SerializableString<SKColor> Background { get; set; }
            = new SerializableString<SKColor>(SKColorThemes.ForeColor);

        /// <summary>
        /// 干扰色。
        /// </summary>
        public SerializableString<SKColor> Disturbing { get; set; }
            = new SerializableString<SKColor>(SKColorThemes.ForeColor);
    }
}
