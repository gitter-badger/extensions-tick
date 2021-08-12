﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Librame.Extensions.Imaging.Processing
{
    using Core.Services;
    using Drawing.Options;

    /// <summary>
    /// 缩放服务接口。
    /// </summary>
    public interface IScaleService : IDrawingService
    {
        /// <summary>
        /// 时钟。
        /// </summary>
        /// <value>
        /// 返回 <see cref="IClockService"/>。
        /// </value>
        IClockService Clock { get; }

        /// <summary>
        /// 水印图画。
        /// </summary>
        /// <value>
        /// 返回 <see cref="IWatermarkService"/>。
        /// </value>
        IWatermarkService Watermark { get; }

        /// <summary>
        /// 图像文件扩展名集合。
        /// </summary>
        IReadOnlyList<string> ImageExtensions { get; }


        /// <summary>
        /// 删除缩放方案集合图片。
        /// </summary>
        /// <param name="imageDirectory">给定的图像目录。</param>
        /// <param name="scales">给定的 <see cref="IEnumerable{ScaleOptions}"/>（可选；默认使用选项配置缩放方案）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含删除张数的异步操作。</returns>
        Task<int> DeleteScalesByDirectoryAsync(string imageDirectory, IEnumerable<ScaleOptions> scales = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// 绘制缩放文件集合。
        /// </summary>
        /// <param name="imageDirectory">给定的图像目录。</param>
        /// <param name="scales">给定的 <see cref="IEnumerable{ScaleOptions}"/>（可选；默认使用选项配置缩放方案）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含处理张数的异步操作。</returns>
        Task<int> DrawFilesByDirectoryAsync(string imageDirectory, IEnumerable<ScaleOptions> scales = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 绘制缩放文件集合。
        /// </summary>
        /// <param name="imagePaths">给定的图像路径集合。</param>
        /// <param name="scales">给定的 <see cref="IEnumerable{ScaleOptions}"/>（可选；默认使用选项配置缩放方案）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含处理张数的异步操作。</returns>
        Task<int> DrawFilesAsync(IEnumerable<string> imagePaths, IEnumerable<ScaleOptions> scales = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 绘制缩放文件。
        /// </summary>
        /// <param name="imagePath">给定的图像路径。</param>
        /// <param name="scales">给定的 <see cref="IEnumerable{ScaleOptions}"/>（可选；默认使用选项配置缩放方案）。</param>
        /// <param name="savePathTemplate">给定的保存路径模板（可选；默认同图像路径）。</param>
        /// <returns>返回一个包含布尔值的异步操作。</returns>
        bool DrawFile(string imagePath, IEnumerable<ScaleOptions> scales = null, string savePathTemplate = null);
    }
}
