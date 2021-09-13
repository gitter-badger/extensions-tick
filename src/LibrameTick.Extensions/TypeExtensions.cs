﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Librame.Extensions
{
    /// <summary>
    /// <see cref="Type"/> 静态扩展。
    /// </summary>
    public static class TypeExtensions
    {
        private delegate object MemberGetDelegate<in TSource>(TSource source);

        private static readonly Type _memberGetDelegateTypeDefinition = typeof(MemberGetDelegate<>);
        private static readonly Type _nullableGenericTypeDefinition = typeof(Nullable<>);


        /// <summary>
        /// 是具体类型（即非抽象、接口类型）。
        /// </summary>
        /// <param name="type">给定的类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsConcreteType(this Type type)
            => !type.IsAbstract && !type.IsInterface;

        /// <summary>
        /// 是可空泛类型。
        /// </summary>
        /// <param name="type">给定的类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsNullableType(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == _nullableGenericTypeDefinition;


        /// <summary>
        /// 获取指定类型的程序集名称。
        /// </summary>
        /// <param name="type">给定的类型。</param>
        /// <returns>返回名称字符串。</returns>
        public static string? GetAssemblyName(this Type type)
            => type.Assembly.GetName().Name;


        /// <summary>
        /// 获取基础类型集合。
        /// </summary>
        /// <param name="type">给定的类型。</param>
        /// <returns>返回 <see cref="IEnumerable{Type}"/>。</returns>
        public static IEnumerable<Type> GetBaseTypes([NotNullWhen(true)] this Type? type)
        {
            // 当前基类（Object 为最顶层基类，接口会直接返回 NULL）
            type = type?.BaseType;

            while (type != null)
            {
                yield return type;

                type = type.BaseType;
            }
        }


        /// <summary>
        /// 通过委托获取指定属性的值。
        /// </summary>
        /// <typeparam name="TSource">指定的源类型。</typeparam>
        /// <param name="property">给定的 <see cref="PropertyInfo"/>。</param>
        /// <param name="source">给定的 <typeparamref name="TSource"/>。</param>
        /// <param name="nonPublic">指示是否应返回非公共的get访问器。如果返回非公共访问器，则为 True；否则，假的。</param>
        /// <returns>返回值对象。</returns>
        public static object GetValueByDelegate<TSource>(this PropertyInfo property, TSource source, bool nonPublic = false)
        {
            var method = property.GetGetMethod(nonPublic);
            if (method == null)
                throw new ArgumentException($"There is no get getter for a property '{property.Name}' of the type '{typeof(TSource)}'.");

            var getMemberDelegate = (MemberGetDelegate<TSource>)Delegate
                .CreateDelegate(_memberGetDelegateTypeDefinition.MakeGenericType(typeof(TSource)), method);
            
            return getMemberDelegate.Invoke(source);
        }


        /// <summary>
        /// 尝试获取特性。
        /// </summary>
        /// <typeparam name="TAttribute">指定的特性类型。</typeparam>
        /// <param name="sourceType">给定的源类型。</param>
        /// <param name="attribute">输出取得的特性。</param>
        /// <returns>返回布尔值。</returns>
        public static bool TryGetAttribute<TAttribute>(this Type sourceType,
            [MaybeNullWhen(false)] out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = sourceType.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        /// <summary>
        /// 尝试获取特性。
        /// </summary>
        /// <param name="sourceType">给定的源类型。</param>
        /// <param name="attributeType">给定要取得的特性类型。</param>
        /// <param name="attribute">输出取得的特性。</param>
        /// <returns>返回布尔值。</returns>
        public static bool TryGetAttribute(this Type sourceType, Type attributeType,
            [MaybeNullWhen(false)] out Attribute attribute)
        {
            attribute = sourceType.GetCustomAttribute(attributeType);
            return attribute != null;
        }


        #region IsAssignableType

        /// <summary>
        /// 是否可以从目标类型分配。
        /// </summary>
        /// <remarks>
        /// 详情参考 <see cref="Type.IsAssignableFrom(Type)"/>。
        /// </remarks>
        /// <typeparam name="TTarget">指定的目标类型。</typeparam>
        /// <param name="baseType">给定的当前基础类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsAssignableFromTargetType<TTarget>(this Type baseType)
            => baseType.IsAssignableFromTargetType(typeof(TTarget));

        /// <summary>
        /// 是否可以从目标类型分配。
        /// </summary>
        /// <remarks>
        /// 详情参考 <see cref="Type.IsAssignableFrom(Type)"/>。
        /// </remarks>
        /// <param name="baseType">给定的当前基础类型。</param>
        /// <param name="targetType">给定的目标类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsAssignableFromTargetType(this Type baseType, Type targetType)
        {
            // 对泛型类型定义提供支持
            return baseType.IsGenericTypeDefinition
                ? targetType.IsImplementedType(baseType)
                : baseType.IsAssignableFrom(targetType);
        }


        /// <summary>
        /// 是否可以分配给基础类型。
        /// </summary>
        /// <remarks>
        /// 与 <see cref="IsAssignableFromTargetType(Type, Type)"/> 参数相反。
        /// </remarks>
        /// <typeparam name="TBase">指定的基础类型。</typeparam>
        /// <param name="targetType">给定的当前目标类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsAssignableToBaseType<TBase>(this Type targetType)
            => targetType.IsAssignableToBaseType(typeof(TBase));

        /// <summary>
        /// 是否可以分配给基础类型。
        /// </summary>
        /// <remarks>
        /// 与 <see cref="IsAssignableFromTargetType(Type, Type)"/> 参数相反。
        /// </remarks>
        /// <param name="targetType">给定的当前目标类型。</param>
        /// <param name="baseType">给定的基础类型。</param>
        /// <returns>返回布尔值。</returns>
        public static bool IsAssignableToBaseType(this Type targetType, Type baseType)
            => baseType.IsAssignableFromTargetType(targetType);

        #endregion


        #region NotAssignableType

        /// <summary>
        /// 当前基础类型不能从目标类型分配。
        /// </summary>
        /// <typeparam name="TTarget">指定的目标类型。</typeparam>
        /// <param name="baseType">给定的当前基础类型。</param>
        /// <returns>返回目标类型。</returns>
        public static Type NotAssignableFromTargetType<TTarget>(this Type baseType)
            => baseType.NotAssignableFromTargetType(typeof(TTarget));

        /// <summary>
        /// 当前基础类型不能从目标类型分配。
        /// </summary>
        /// <param name="baseType">给定的当前基础类型。</param>
        /// <param name="targetType">给定的目标类型。</param>
        /// <returns>返回目标类型。</returns>
        public static Type NotAssignableFromTargetType(this Type baseType, Type targetType)
        {
            if (!baseType.IsAssignableFromTargetType(targetType))
                throw new NotImplementedException($"The current base type '{baseType}' cannot be assigned from the target type '{targetType}'.");

            return targetType;
        }


        /// <summary>
        /// 当前目标类型不能分配给基础类型。
        /// </summary>
        /// <typeparam name="TBase">指定的基础类型。</typeparam>
        /// <param name="targetType">给定的当前目标类型。</param>
        /// <returns>返回基础类型。</returns>
        public static Type NotAssignableToBaseType<TBase>(this Type targetType)
            => targetType.NotAssignableToBaseType(typeof(TBase));

        /// <summary>
        /// 当前目标类型不能分配给基础类型。
        /// </summary>
        /// <param name="targetType">给定的当前目标类型。</param>
        /// <param name="baseType">给定的基础类型。</param>
        /// <returns>返回基础类型。</returns>
        public static Type NotAssignableToBaseType(this Type targetType, Type baseType)
        {
            if (!targetType.IsAssignableToBaseType(baseType))
                throw new NotImplementedException($"The current target type '{targetType}' cannot be assigned to the base type '{baseType}'.");

            return baseType;
        }

        #endregion


        #region IsImplementedType

        /// <summary>
        /// 是否已实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <typeparam name="T">指定的已实现类型（支持基础类型、接口、泛型类型定义等）。</typeparam>
        /// <param name="currentType">给定的当前类型。</param>
        /// <returns>返回是否已实现的布尔值。</returns>
        public static bool IsImplementedType<T>(this Type currentType)
            => currentType.IsImplementedType(typeof(T), out _);

        /// <summary>
        /// 是否已实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <typeparam name="T">指定的已实现类型（支持基础类型、接口、泛型类型定义等）。</typeparam>
        /// <param name="currentType">给定的当前类型。</param>
        /// <param name="resultType">输出此结果类型（当基础类型为泛型定义时，可用于得到泛型参数等操作）。</param>
        /// <returns>返回是否已实现的布尔值。</returns>
        public static bool IsImplementedType<T>(this Type currentType, [MaybeNullWhen(false)] out Type resultType)
            => currentType.IsImplementedType(typeof(T), out resultType);

        /// <summary>
        /// 是否已实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <param name="currentType">给定的当前类型。</param>
        /// <param name="implementedType">给定的已实现类型（支持基础类型、接口、泛型类型定义等）。</param>
        /// <returns>返回是否已实现的布尔值。</returns>
        public static bool IsImplementedType(this Type currentType, Type implementedType)
            => currentType.IsImplementedType(implementedType, out _);

        /// <summary>
        /// 是否已实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <param name="currentType">给定的当前类型。</param>
        /// <param name="implementedType">给定的已实现类型（支持基础类型、接口、泛型类型定义等）。</param>
        /// <param name="resultType">输出此结果类型（当基础类型为泛型定义时，可用于得到泛型参数等操作）。</param>
        /// <returns>返回是否已实现的布尔值。</returns>
        public static bool IsImplementedType(this Type currentType, Type implementedType,
            [MaybeNullWhen(false)] out Type resultType)
        {
            var allImplementedTypes = implementedType.IsInterface
                ? currentType.GetInterfaces()
                : currentType.GetBaseTypes(); // Extensions

            // 如果已实现类型是泛型定义
            if (implementedType.IsGenericTypeDefinition)
            {
                resultType = allImplementedTypes.Where(type => type.IsGenericType)
                    .FirstOrDefault(type => type.GetGenericTypeDefinition() == implementedType);
            }
            else
            {
                resultType = allImplementedTypes.FirstOrDefault(type => type == implementedType);
            }

            return resultType != null;
        }

        #endregion


        #region NotImplementedType

        /// <summary>
        /// 未实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <typeparam name="T">指定的已实现类型（支持基础类型、接口、泛型类型定义等）。</typeparam>
        /// <param name="currentType">给定的当前类型。</param>
        /// <returns>返回实现的结果类型（当基础类型为泛型定义时，可用于得到泛型参数等操作）。</returns>
        public static Type NotImplementedType<T>(this Type currentType)
            => NotImplementedType(currentType, typeof(T));

        /// <summary>
        /// 未实现指定类型（支持基础类型、接口、泛型类型定义等）。
        /// </summary>
        /// <param name="currentType">给定的当前类型。</param>
        /// <param name="implementedType">给定的已实现类型（支持基础类型、接口、泛型类型定义等）。</param>
        /// <returns>返回实现的结果类型（当基础类型为泛型定义时，可用于得到泛型参数等操作）。</returns>
        public static Type NotImplementedType(this Type currentType, Type implementedType)
        {
            if (!currentType.IsImplementedType(implementedType, out var resultType))
                throw new NotImplementedException($"The current type '{currentType}' does not implement the specified type '{implementedType}'.");

            return resultType;
        }

        #endregion

    }
}
