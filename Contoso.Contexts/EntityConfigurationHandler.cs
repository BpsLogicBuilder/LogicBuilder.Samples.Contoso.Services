using LogicBuilder.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Contoso.Contexts
{
    public class EntityConfigurationHandler(DbContext context)
    {

        #region Properties
        protected DbContext Context { get; private set; } = context;
        #endregion Properties

        #region Methods
        public virtual void Configure(ModelBuilder modelBuilder)
        {
            foreach (PropertyInfo property in this.Context.GetType().GetProperties())
            {
                if (property.PropertyType.Name != "DbSet`1")
                    continue;

                Type modelType = property.PropertyType.GetGenericArguments()[0];
                if (!typeof(BaseData).IsAssignableFrom(modelType))
                    continue;

                modelBuilder.Entity(modelType).Ignore(nameof(BaseData.EntityState));
            }

            Type interfaceType = typeof(Configuations.ITableConfiguration);
            interfaceType.Assembly.GetTypes().Where(p => interfaceType.IsAssignableFrom(p)
                                && !p.IsAbstract
                                && !p.IsGenericTypeDefinition
                                && !p.IsInterface).ToList().ForEach(t =>
                                {
                                    MethodInfo mi = t.GetMethod(nameof(Configuations.ITableConfiguration.Configure))!;//ITableConfiguration implements Configure
                                    mi.Invoke(Activator.CreateInstance(t), [modelBuilder]);
                                });
        }
        #endregion Methods
    }
}
