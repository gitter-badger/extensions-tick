using Librame.Extensions.Core;
using Librame.Extensions.Data.Storing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Librame.Extensions.Data.Accessing
{
    public class TestAccessorTests
    {

        [Fact]
        public void AllTest()
        {
            var services = new ServiceCollection();

            services.AddDbContext<TestMySqlAccessor>(opts =>
            {
                opts.UseMySql(MySqlConnectionStringHelper.Validate("server=localhost;port=3306;database=librame_extensions;user=root;password=123456;", out var version), version,
                    a => a.MigrationsAssembly(typeof(User).Assembly.FullName));

                opts.UseAccessor(b => b.WithInteraction(AccessorInteraction.Write).WithPriority(1));
            });
            
            services.AddDbContextPool<TestSqlServerAccessor>(opts =>
            {
                opts.UseSqlServer("server=.;database=librame_extensions;integrated security=true;",
                    a => a.MigrationsAssembly(typeof(User).Assembly.FullName));

                opts.UseAccessor(b => b.WithInteraction(AccessorInteraction.Write).WithPooling().WithPriority(2));
            });

            services.AddDbContext<TestSqliteAccessor>(opts =>
            {
                opts.UseSqlite("Data Source=librame_extensions.db",
                    a => a.MigrationsAssembly(typeof(User).Assembly.FullName));

                opts.UseAccessor(b => b.WithInteraction(AccessorInteraction.Read));
            });

            services.AddLibrame()
                .AddData(opts =>
                {
                    // ����ʱÿ���������½����ݿ�
                    opts.Access.EnsureDatabaseDeleted = true;

                    // ÿ���޸�ѡ��ʱ�Զ�����Ϊ JSON �ļ�
                    opts.PropertyChangedAction = (o, e) => o.SaveOptionsAsJson();
                })
                .AddSeeder<InternalTestAccessorSeeder>()
                .AddMigrator<InternalTestAccessorMigrator>()
                .AddInitializer<InternalTestAccessorInitializer<TestMySqlAccessor>>()
                .AddInitializer<InternalTestAccessorInitializer<TestSqlServerAccessor>>()
                .AddInitializer<InternalTestAccessorInitializer<TestSqliteAccessor>>()
                .SaveOptionsAsJson(); // �״α���ѡ��Ϊ JSON �ļ�

            var provider = services.BuildServiceProvider();

            provider.UseAccessorInitializer();

            var store = provider.GetRequiredService<IStore<User>>();
            Assert.NotNull(store);

            var pagingUsers = store.FindPagingList(p => p.PageByIndex(index: 1, size: 5));
            Assert.NotEmpty(pagingUsers);

            // Update
            foreach (var user in pagingUsers)
            {
                user.Name = $"Update {user.Name}";
            }

            // �����д�������
            store.Update(pagingUsers);

            // Add
            var addUsers = new User[10];

            for (var i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Name = $"Add Name {i + 1}",
                    Passwd = "123456"
                };

                user.Id = store.IdGeneratorFactory.GetNewId<long>();
                user.PopulateCreation(0, DateTimeOffset.UtcNow);

                addUsers[i] = user;
            }

            // �����д�������
            store.Add(addUsers);

            store.SaveChanges();

            // �޸ģ���ȡ��������Sqlite�������ޱ仯
            var users = store.FindList(p => p.Name!.StartsWith("Update"));
            Assert.Empty(users);

            // �޸ģ�ǿ�ƴ�д���������ѯ��MySQL/SQL Server��
            users = store.FindList(p => p.Name!.StartsWith("Update"), fromWriteAccessor: true);
            Assert.NotEmpty(users);

            // ��������ȡ��������Sqlite�������ޱ仯
            users = store.FindList(p => p.Name!.StartsWith("Add"));
            Assert.Empty(users);

            // ������ǿ�ƴ�д���������ѯ��MySQL/SQL Server��
            users = store.FindList(p => p.Name!.StartsWith("Add"), fromWriteAccessor: true);
            Assert.NotEmpty(users);
        }

    }
}
