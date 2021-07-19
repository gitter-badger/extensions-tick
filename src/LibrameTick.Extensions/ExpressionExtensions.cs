﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Librame.Extensions
{
    /// <summary>
    /// <see cref="Expression"/> 静态扩展。
    /// </summary>
    public static class ExpressionExtensions
    {

        /// <summary>
        /// 作为属性表达式的名称。
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// <paramref name="propertyExpression"/> not supported.
        /// </exception>
        /// <typeparam name="T">指定的类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyExpression">给定的属性表达式。</param>
        /// <returns>返回字符串。</returns>
        public static string AsPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            propertyExpression.NotNull(nameof(propertyExpression));

            return propertyExpression.Body switch
            {
                // 一元运算符
                UnaryExpression unary => ((MemberExpression)unary.Operand).Member.Name,
                // 访问的字段或属性
                MemberExpression member => member.Member.Name,
                // 参数表达式
                ParameterExpression parameter => parameter.Type.Name,
                // 默认
                _ => throw new NotSupportedException("Not supported property expression.")
            };
        }

        /// <summary>
        /// 如果指定类型实例的属性值。
        /// </summary>
        /// <typeparam name="T">指定的类型。</typeparam>
        /// <typeparam name="TValue">指定的属性值类型。</typeparam>
        /// <param name="propertyExpression">给定的属性表达式。</param>
        /// <param name="source">给定要获取属性值的类型实例。</param>
        /// <returns>返回属性值。</returns>
        public static TValue? AsPropertyValue<T, TValue>(this Expression<Func<T, TValue>> propertyExpression, T source)
        {
            source.NotNull(nameof(source));

            var name = propertyExpression.AsPropertyName();

            try
            {
                var pi = typeof(T).GetRuntimeProperty(name);
                return (TValue?)pi?.GetValue(source);
            }
            catch (AmbiguousMatchException)
            {
                return default;
            }
        }


        #region AsPropertyExpression
        
        /// <summary>
        /// 转换为单个属性键的 Lambda 表达式（例：p => p.PropertyName）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定的属性名。</param>
        /// <returns>返回 lambda 表达式。</returns>
        public static Expression<Func<T, TProperty>> AsPropertyExpression<T, TProperty>(this string propertyName)
        {
            // 建立变量
            var p = Expression.Parameter(typeof(T), "p");

            // 建立属性
            var property = Expression.Property(p, propertyName);

            // p => p.PropertyName
            return Expression.Lambda<Func<T, TProperty>>(property, p);
        }


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName > compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsGreaterThanPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.GreaterThan(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName > compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsGreaterThanPropertyExpression<T>(this string propertyName, Type propertyType,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.GreaterThan(p, c));


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName >= compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsGreaterThanOrEqualPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.GreaterThanOrEqual(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName >= compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsGreaterThanOrEqualPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.GreaterThanOrEqual(p, c));


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName 〈 compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsLessThanPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.LessThan(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName 〈 compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsLessThanPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.LessThan(p, c));


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName 〈= compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsLessThanOrEqualPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.LessThanOrEqual(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName 〈= compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsLessThanOrEqualPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.LessThanOrEqual(p, c));


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName != compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsNotEqualPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.NotEqual(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName != compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsNotEqualPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.NotEqual(p, c));


        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName == compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TProperty">指定的属性类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsEqualPropertyExpression<T, TProperty>(this string propertyName,
            object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(typeof(TProperty), value,
                (p, c) => Expression.Equal(p, c));

        /// <summary>
        /// 转换为比较的单个属性值等于的 Lambda 表达式（例：p => p.PropertyName == compareValue）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定用于对比的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsEqualPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value)
            => propertyName.AsPropertyExpression<T, BinaryExpression>(propertyType, value,
                (p, c) => Expression.Equal(p, c));


        /// <summary>
        /// 转换为使用单个属性值进行比较的 Lambda 表达式（例：p => p.PropertyName.CompareTo(value)）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <typeparam name="TExpression">指定的表达式类型。</typeparam>
        /// <param name="propertyName">给定的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <param name="compareToFactory">给定的对比方法。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsPropertyExpression<T, TExpression>(this string propertyName,
            Type propertyType, object? value, Func<MemberExpression, ConstantExpression, TExpression> compareToFactory)
            where TExpression : Expression
        {
            compareToFactory.NotNull(nameof(compareToFactory));

            var p = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(p, propertyName);
            var constant = Expression.Constant(value, propertyType);

            // 调用方法（如：Expression.Equal(property, constant);）
            var body = compareToFactory.Invoke(property, constant);

            // p => p.PropertyName == value
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        /// <summary>
        /// 转换为使用单个属性值进行比较的 Lambda 表达式（例：p => p.PropertyName.CallMethodName(value)）。
        /// </summary>
        /// <typeparam name="T">指定的实体类型。</typeparam>
        /// <param name="propertyName">给定的属性名。</param>
        /// <param name="propertyType">给定的属性类型。</param>
        /// <param name="value">给定的参考值。</param>
        /// <param name="callMethodName">给定要调用的方法名。</param>
        /// <returns>返回 Lambda 表达式。</returns>
        public static Expression<Func<T, bool>> AsPropertyExpression<T>(this string propertyName,
            Type propertyType, object? value, string callMethodName)
        {
            var type = typeof(T);

            var p = Expression.Parameter(type, "p");
            var property = Expression.Property(p, propertyName);
            var constant = Expression.Constant(value, propertyType);

            var propertyInfo = type.GetRuntimeProperty(propertyName);
            if (propertyInfo.IsNull())
                throw new ArgumentException($"The property '{type}' with the specified name '{propertyName}' was not found.");

            var method = propertyInfo.PropertyType.GetRuntimeMethod(callMethodName,
                new Type[] { propertyType });
            if (method.IsNull())
                throw new ArgumentException($"The method '{type}' with the specified name '{propertyName}' was not found.");

            var body = Expression.Call(property, method, constant);

            // p => p.PropertyName.CallMethodName(value)
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        #endregion

    }
}