﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Collections;
using Librame.Extensions.Data.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Librame.Extensions.Data.Accessing
{
    /// <summary>
    /// 定义抽象 <see cref="AbstractAccessor"/> 的泛型实现。
    /// </summary>
    /// <typeparam name="TAccessor">指定实现 <see cref="AbstractAccessor"/> 的访问器类型。</typeparam>
    public abstract class AbstractAccessor<TAccessor> : AbstractAccessor
        where TAccessor : AbstractAccessor
    {
        /// <summary>
        /// 使用指定的数据库上下文选项构造一个 <see cref="AbstractAccessor{TAccessor}"/> 实例。
        /// </summary>
        /// <remarks>
        /// 备注：如果需要注册多个 <see cref="DbContext"/> 扩展，参数必须使用泛型 <see cref="DbContextOptions{TAccessor}"/> 形式，
        /// 不能使用非泛型 <see cref="DbContextOptions"/> 形式，因为 <paramref name="options"/> 参数也会注册到容器中以供使用。
        /// </remarks>
        /// <param name="options">给定的 <see cref="DbContextOptions{TAccessor}"/>。</param>
        protected AbstractAccessor(DbContextOptions<TAccessor> options)
            : base(options)
        {
        }


        /// <summary>
        /// 访问器类型。
        /// </summary>
        public override Type AccessorType
            => typeof(TAccessor);
    }


    /// <summary>
    /// 定义抽象 <see cref="DbContext"/> 与 <see cref="IAccessor"/> 的实现。
    /// </summary>
    public abstract class AbstractAccessor : DbContext, IAccessor
    {
        /// <summary>
        /// 使用指定的数据库上下文选项构造一个 <see cref="AbstractAccessor"/> 实例。
        /// </summary>
        /// <param name="options">给定的 <see cref="DbContextOptions"/>。</param>
        protected AbstractAccessor(DbContextOptions options)
            : base(options)
        {
            OptionsExtension = options.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault();

            // 当启用分库功能时，需在切换到分库后尝试创建数据库
            ChangedAction = accessor => accessor.TryCreateDatabase();
        }


        /// <summary>
        /// 核心选项扩展。
        /// </summary>
        protected CoreOptionsExtension? OptionsExtension { get; init; }

        /// <summary>
        /// 数据扩展选项。
        /// </summary>
        protected DataExtensionOptions DataOptions
            => GetRequiredScopeService<DataExtensionOptions>();

        /// <summary>
        /// 关系连接。
        /// </summary>
        protected IRelationalConnection? RelationalConnection
            => GetScopeService<IRelationalConnection>();


        /// <summary>
        /// 访问器标识。
        /// </summary>
        public virtual string AccessorId
            => ContextId.ToString();

        /// <summary>
        /// 访问器类型。
        /// </summary>
        public virtual Type AccessorType
            => GetType();


        #region GetScopeService

        /// <summary>
        /// 获取范围服务。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型</typeparam>
        /// <param name="serviceType">给定的服务类型（可选；默认使用指定的泛型服务类型）。</param>
        /// <returns>返回 <typeparamref name="TService"/>。</returns>
        public virtual TService GetRequiredScopeService<TService>(Type? serviceType = null)
            => (TService)GetRequiredScopeService(serviceType ?? typeof(TService));

        /// <summary>
        /// 获取范围服务。
        /// </summary>
        /// <param name="serviceType">给定的服务类型。</param>
        /// <returns>返回对象。</returns>
        public virtual object GetRequiredScopeService(Type serviceType)
        {
            var service = GetScopeService(serviceType);
            if (service == null)
                throw new ArgumentException($"The scope instance of the service type '{serviceType}' is null.");
            
            return service;
        }


        /// <summary>
        /// 获取范围服务。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型</typeparam>
        /// <param name="serviceType">给定的服务类型（可选；默认使用指定的泛型服务类型）。</param>
        /// <returns>返回 <typeparamref name="TService"/>。</returns>
        public virtual TService? GetScopeService<TService>(Type? serviceType = null)
            => (TService?)GetScopeService(serviceType ?? typeof(TService));

        /// <summary>
        /// 获取范围服务。
        /// </summary>
        /// <param name="serviceType">给定的服务类型。</param>
        /// <returns>返回对象。</returns>
        public virtual object? GetScopeService(Type serviceType)
        {
            // this.GetService<T>() 即 InfrastructureExtensions.GetService<T>() 这种方法可能会抛出 internalServiceProvider 不是范围服务，
            // 而内部使用的 internalServiceProvider.GetService<IDbContextOptions>()?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
            // 方式获取到的实例可能为空，而直接通过 DbContext 构造函数的 DbContextOptions 参数获取到的服务实例则正常

            // OptionsExtension?.ApplicationServiceProvider 此处是范围服务
            return ((IInfrastructure<IServiceProvider>)this).Instance.GetService(serviceType)
                ?? OptionsExtension?.ApplicationServiceProvider?.GetService(serviceType);
        }

        #endregion


        #region Exists

        /// <summary>
        /// 在本地缓存或数据库中是否存在指定断定方法的实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="predicate">给定的断定方法表达式。</param>
        /// <param name="checkLocal">是否检查本地缓存（可选；默认启用检查）。</param>
        /// <returns>返回布尔值。</returns>
        public virtual bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate,
            bool checkLocal = true)
            where TEntity : class
            => base.Set<TEntity>().LocalOrDbAny(predicate, checkLocal);

        /// <summary>
        /// 异步在本地缓存或数据库中是否存在指定断定方法的实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="predicate">给定的断定方法表达式。</param>
        /// <param name="checkLocal">是否检查本地缓存（可选；默认启用检查）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含布尔值的异步操作。</returns>
        public virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            bool checkLocal = true, CancellationToken cancellationToken = default)
            where TEntity : class
            => base.Set<TEntity>().LocalOrDbAnyAsync(predicate, checkLocal, cancellationToken);

        #endregion


        #region Find

        /// <summary>
        /// 查找带有规约的实体集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="specification">给定的 <see cref="ISpecification{TEntity}"/>（可选）。</param>
        /// <returns>返回 <see cref="IList{TEntity}"/>。</returns>
        public virtual IList<TEntity> FindListWithSpecification<TEntity>(ISpecification<TEntity>? specification = null)
            where TEntity : class
            => SpecificationEvaluator.EvaluateList(GetQueryable<TEntity>(), specification);

        /// <summary>
        /// 异步查找带有规约的实体集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="specification">给定的 <see cref="ISpecification{TEntity}"/>（可选）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含 <see cref="IList{TEntity}"/> 的异步操作。</returns>
        public virtual Task<IList<TEntity>> FindListWithSpecificationAsync<TEntity>(ISpecification<TEntity>? specification = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
            => SpecificationEvaluator.EvaluateListAsync(GetQueryable<TEntity>(), specification, cancellationToken);


        /// <summary>
        /// 查找实体分页集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="pageAction">给定的分页动作。</param>
        /// <returns>返回 <see cref="IPagingList{TEntity}"/>。</returns>
        public virtual IPagingList<TEntity> FindPagingList<TEntity>(Action<IPagingList<TEntity>> pageAction)
            where TEntity : class
            => GetQueryable<TEntity>().AsPaging(pageAction);

        /// <summary>
        /// 异步查找实体分页集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="pageAction">给定的分页动作。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含 <see cref="IPagingList{TEntity}"/> 的异步操作。</returns>
        public virtual Task<IPagingList<TEntity>> FindPagingListAsync<TEntity>(Action<IPagingList<TEntity>> pageAction,
            CancellationToken cancellationToken = default)
            where TEntity : class
            => GetQueryable<TEntity>().AsPagingAsync(pageAction, cancellationToken);


        /// <summary>
        /// 查找带有规约的实体分页集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="pageAction">给定的分页动作。</param>
        /// <param name="specification">给定的 <see cref="ISpecification{TEntity}"/>（可选）。</param>
        /// <returns>返回 <see cref="IPagingList{TEntity}"/>。</returns>
        public virtual IPagingList<TEntity> FindPagingListWithSpecification<TEntity>(Action<IPagingList<TEntity>> pageAction,
            ISpecification<TEntity>? specification = null)
            where TEntity : class
            => SpecificationEvaluator.EvaluatePagingList(GetQueryable<TEntity>(), pageAction, specification);

        /// <summary>
        /// 异步查找带有规约的实体分页集合。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="pageAction">给定的分页动作。</param>
        /// <param name="specification">给定的 <see cref="ISpecification{TEntity}"/>（可选）。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含 <see cref="IPagingList{TEntity}"/> 的异步操作。</returns>
        public virtual Task<IPagingList<TEntity>> FindPagingListWithSpecificationAsync<TEntity>(Action<IPagingList<TEntity>> pageAction,
            ISpecification<TEntity>? specification = null, CancellationToken cancellationToken = default)
            where TEntity : class
            => SpecificationEvaluator.EvaluatePagingListAsync(GetQueryable<TEntity>(), pageAction, specification, cancellationToken);

        #endregion


        #region GetQueryable

        /// <summary>
        /// 获取指定实体的可查询接口。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <returns>返回 <see cref="IQueryable{TEntity}"/>。</returns>
        public virtual IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class
            => base.Set<TEntity>();

        /// <summary>
        /// 获取指定实体的可查询接口。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="name">要使用的共享类型实体类型的名称。</param>
        /// <returns>返回 <see cref="IQueryable{TEntity}"/>。</returns>
        public virtual IQueryable<TEntity> GetQueryable<TEntity>(string name)
            where TEntity : class
            => base.Set<TEntity>(name);

        #endregion


        #region Add

        /// <summary>
        /// 添加不存在指定断定方法的实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="entity">给定要添加的实体。</param>
        /// <param name="predicate">给定的断定方法表达式。</param>
        /// <param name="checkLocal">是否检查本地缓存（可选；默认启用检查）。</param>
        /// <returns>返回 <typeparamref name="TEntity"/>。</returns>
        public virtual TEntity AddIfNotExists<TEntity>(TEntity entity,
            Expression<Func<TEntity, bool>> predicate, bool checkLocal = true)
            where TEntity : class
        {
            if (!Exists(predicate, checkLocal))
                base.Add(entity);

            return entity;
        }

        /// <summary>
        /// 添加实体对象。
        /// </summary>
        /// <param name="entity">给定要添加的实体对象。</param>
        /// <returns>返回实体对象。</returns>
        public virtual new object Add(object entity)
        {
            base.Add(entity);
            return entity;
        }

        /// <summary>
        /// 添加实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="entity">给定要添加的实体。</param>
        /// <returns>返回 <typeparamref name="TEntity"/>。</returns>
        public virtual new TEntity Add<TEntity>(TEntity entity)
            where TEntity : class
        {
            base.Add(entity);
            return entity;
        }

        #endregion


        #region Attach

        /// <summary>
        /// 附加实体对象。
        /// </summary>
        /// <param name="entity">给定要附加的实体对象。</param>
        /// <returns>返回实体对象。</returns>
        public virtual new object Attach(object entity)
        {
            base.Attach(entity);
            return entity;
        }

        /// <summary>
        /// 附加实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="entity">给定要附加的实体。</param>
        /// <returns>返回 <typeparamref name="TEntity"/>。</returns>
        public virtual new TEntity Attach<TEntity>(TEntity entity)
            where TEntity : class
        {
            base.Attach(entity);
            return entity;
        }

        #endregion


        #region Remove

        /// <summary>
        /// 移除实体对象。
        /// </summary>
        /// <param name="entity">给定要移除的实体对象。</param>
        /// <returns>返回实体对象。</returns>
        public virtual new object Remove(object entity)
        {
            base.Remove(entity);
            return entity;
        }

        /// <summary>
        /// 移除实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体。</typeparam>
        /// <param name="entity">给定要移除的实体。</param>
        /// <returns>返回 <typeparamref name="TEntity"/>。</returns>
        public virtual new TEntity Remove<TEntity>(TEntity entity)
            where TEntity : class
        {
            base.Remove(entity);
            return entity;
        }

        #endregion


        #region Update

        /// <summary>
        /// 更新实体对象。
        /// </summary>
        /// <param name="entity">给定要更新的实体对象。</param>
        /// <returns>返回实体对象。</returns>
        public virtual new object Update(object entity)
        {
            base.Update(entity);
            return entity;
        }

        /// <summary>
        /// 更新实体。
        /// </summary>
        /// <typeparam name="TEntity">指定的实体类型。</typeparam>
        /// <param name="entity">给定要更新的实体。</param>
        /// <returns>返回 <typeparamref name="TEntity"/>。</returns>
        public virtual new TEntity Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            base.Update(entity);
            return entity;
        }

        #endregion


        #region IConnectable<IAccessor>

        /// <summary>
        /// 当前连接字符串。
        /// </summary>
        public virtual string? CurrentConnectionString
            => RelationalConnection?.ConnectionString;

        /// <summary>
        /// 改变时动作（传入的参数为当前 <see cref="IAccessor"/>）。
        /// </summary>
        public Action<IAccessor>? ChangingAction { get; set; }

        /// <summary>
        /// 改变后动作（传入的参数为当前 <see cref="IAccessor"/>）。
        /// </summary>
        public Action<IAccessor>? ChangedAction { get; set; }


        /// <summary>
        /// 改变数据库连接。
        /// </summary>
        /// <param name="newConnectionString">给定的新数据库连接字符串。</param>
        /// <returns>返回 <see cref="IAccessor"/>。</returns>
        public virtual IAccessor ChangeConnection(string newConnectionString)
        {
            var connection = RelationalConnection?.DbConnection;
            if (connection != null)
            {
                ChangingAction?.Invoke(this);

                connection.ConnectionString = newConnectionString;

                ChangedAction?.Invoke(this);
            }

            return this;
        }


        /// <summary>
        /// 尝试创建数据库（已集成是否需要先删除数据库功能）。
        /// </summary>
        /// <returns>返回布尔值。</returns>
        public virtual bool TryCreateDatabase()
        {
            if (DataOptions.Access.EnsureDatabaseDeleted)
                Database.EnsureDeleted();

            if (DataOptions.Access.EnsureDatabaseCreated)
                return Database.EnsureCreated();

            return false;
        }

        /// <summary>
        /// 异步尝试创建数据库（已集成是否需要先删除数据库功能）。
        /// </summary>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含布尔值的异步操作。</returns>
        public virtual async Task<bool> TryCreateDatabaseAsync(
            CancellationToken cancellationToken = default)
        {
            if (DataOptions.Access.EnsureDatabaseDeleted)
                await Database.EnsureDeletedAsync(cancellationToken);

            if (DataOptions.Access.EnsureDatabaseCreated)
                await Database.EnsureCreatedAsync(cancellationToken);

            return false;
        }

        #endregion


        #region ISortable

        /// <summary>
        /// 排序优先级（数值越小越优先）。
        /// </summary>
        public virtual float Priority
            => 1;


        /// <summary>
        /// 与指定的 <see cref="ISortable"/> 比较大小。
        /// </summary>
        /// <param name="other">给定的 <see cref="ISortable"/>。</param>
        /// <returns>返回整数。</returns>
        public virtual int CompareTo(ISortable? other)
            => Priority.CompareTo(other?.Priority ?? 0);

        #endregion

    }

}
