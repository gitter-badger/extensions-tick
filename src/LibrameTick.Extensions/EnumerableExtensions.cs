﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions;

/// <summary>
/// <see cref="IEnumerable"/> 静态扩展。
/// </summary>
public static class EnumerableExtensions
{

    #region AsEnumerable

    /// <summary>
    /// 转换为可枚举集合。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="item">给定的类型实例。</param>
    /// <returns>返回 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> AsEnumerable<T>(T item)
    {
        yield return item;
    }

    /// <summary>
    /// 转换为可枚举集合。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="task">给定的异步操作。</param>
    /// <returns>返回 <see cref="IAsyncEnumerable{T}"/>。</returns>
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(Task<T> task)
    {
        yield return await task;
    }

    /// <summary>
    /// 转换为异步可枚举集合。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的可枚举异步操作。</param>
    /// <returns>返回 <see cref="IAsyncEnumerable{T}"/>。</returns>
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<Task<T>> enumerable)
    {
        foreach (var task in enumerable)
            yield return await task;
    }

    #endregion


    #region AsReadOnlyList

    /// <summary>
    /// 转换为只读列表集合。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <returns>返回 <see cref="ReadOnlyCollection{T}"/>。</returns>
    public static ReadOnlyCollection<T> AsReadOnlyList<T>(this IEnumerable<T> enumerable)
        => enumerable.ToList().AsReadOnly();

    /// <summary>
    /// 转换为只读列表集合。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="list">给定的 <see cref="IList{T}"/>。</param>
    /// <returns>返回 <see cref="ReadOnlyCollection{T}"/>。</returns>
    public static ReadOnlyCollection<T> AsReadOnlyList<T>(this IList<T> list)
        => new ReadOnlyCollection<T>(list);

    #endregion


    #region ForEach

    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <returns>返回一个异步操作。</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> items, Action<T> action)
    {
        await foreach (var item in items)
        {
            action(item);
        }
    }

    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <param name="breakFunc">给定跳出遍历的动作。</param>
    /// <returns>返回一个异步操作。</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> items, Action<T> action,
        Func<T, bool> breakFunc)
    {
        await foreach (var item in items)
        {
            action(item);

            if (breakFunc(item))
                break;
        }
    }


    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <returns>返回一个异步操作。</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> items, Action<T, int> action)
    {
        var i = 0;
        await foreach (var item in items)
        {
            action(item, i);
            i++;
        }
    }

    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <param name="breakFunc">给定跳出遍历的动作。</param>
    /// <returns>返回一个异步操作。</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> items,
        Action<T, int> action, Func<T, int, bool> breakFunc)
    {
        var i = 0;
        await foreach (var item in items)
        {
            action(item, i);

            if (breakFunc(item, i))
                break;

            i++;
        }
    }


    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
            action(item);
    }

    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <param name="breakFunc">给定跳出遍历的动作。</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action,
        Func<T, bool> breakFunc)
    {
        foreach (var item in items)
        {
            action(item);

            if (breakFunc(item))
                break;
        }
    }


    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
    {
        var i = 0;
        foreach (var item in items)
        {
            action(item, i);
            i++;
        }
    }

    /// <summary>
    /// 遍历元素集合（元素集合为空或空集合则返回，不抛异常）。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="items">给定的元素集合。</param>
    /// <param name="action">给定的遍历动作。</param>
    /// <param name="breakFunc">给定跳出遍历的动作。</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action,
        Func<T, int, bool> breakFunc)
    {
        var i = 0;
        foreach (var item in items)
        {
            action(item, i);

            if (breakFunc(item, i))
                break;

            i++;
        }
    }

    #endregion


    #region Trim

    /// <summary>
    /// 修剪集合初始和末尾指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="firstAndLast">要修剪的初始和末尾项。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> Trim<T>(this IEnumerable<T> enumerable,
        T? firstAndLast, bool isLoops = true)
        where T : IEquatable<T>
        => enumerable.Trim(f => f.Equals(firstAndLast), isLoops);

    /// <summary>
    /// 修剪集合初始指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="first">要修剪的初始项。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> TrimFirst<T>(this IEnumerable<T> enumerable,
        T? first, bool isLoops = true)
        where T : IEquatable<T>
        => enumerable.TrimFirst(f => f.Equals(first), isLoops);

    /// <summary>
    /// 修剪集合末尾指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="last">要修剪的末尾项。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> TrimLast<T>(this IEnumerable<T> enumerable,
        T? last, bool isLoops = true)
        where T : IEquatable<T>
        => enumerable.TrimLast(f => f.Equals(last), isLoops);


    /// <summary>
    /// 修剪集合初始和末尾指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="isFirstAndLastFunc">断定是初始和末尾项的方法。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> Trim<T>(this IEnumerable<T> enumerable,
        Func<T, bool> isFirstAndLastFunc, bool isLoops = true)
        => enumerable.TrimFirst(isFirstAndLastFunc, isLoops).TrimLast(isFirstAndLastFunc, isLoops);

    /// <summary>
    /// 修剪集合初始指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="isFirstFunc">断定是初始项的方法。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> TrimFirst<T>(this IEnumerable<T> enumerable,
        Func<T, bool> isFirstFunc, bool isLoops = true)
    {
        var count = enumerable.Count();
        if (count > 0 && isFirstFunc(enumerable.First()))
        {
            // 修剪初始项
            count--;

            if (count < 1)
                return Enumerable.Empty<T>();

            // 单次修剪一个（即跳过首个）元素
            enumerable = enumerable.Skip(1);

            if (isLoops) // 链式修剪
                enumerable = enumerable.TrimFirst(isFirstFunc, isLoops);
        }

        return enumerable;
    }

    /// <summary>
    /// 修剪集合末尾指定项。
    /// </summary>
    /// <typeparam name="T">指定的类型。</typeparam>
    /// <param name="enumerable">给定的 <see cref="IEnumerable{T}"/>。</param>
    /// <param name="isLastFunc">断定是末尾项的方法。</param>
    /// <param name="isLoops">是否循环查找（可选；默认启用）。</param>
    /// <returns>返回修剪后的 <see cref="IEnumerable{T}"/>。</returns>
    public static IEnumerable<T> TrimLast<T>(this IEnumerable<T> enumerable,
        Func<T, bool> isLastFunc, bool isLoops = true)
    {
        var count = enumerable.Count();
        if (count > 0 && isLastFunc(enumerable.Last()))
        {
            // 修剪末尾项
            count--;

            if (count < 1)
                return Enumerable.Empty<T>();

            // 单次修剪一个（即取得前 COUNT-1）元素
            enumerable = enumerable.Take(count);

            if (isLoops) // 链式修剪
                enumerable = enumerable.TrimLast(isLastFunc, isLoops);
        }

        return enumerable;
    }

    #endregion

}
