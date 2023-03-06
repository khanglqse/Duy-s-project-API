using DuyProject.API.Configurations;
using DuyProject.API.ViewModels;
using System.Linq.Expressions;

namespace DuyProject.API.Helpers;

public static class QueryExtension
{
    public static IQueryable<T> ToPaging<T>(this IQueryable<T> query, PaginationRequest model)
    {
        if (model.PageNumber <= 0)
        {
            model.PageNumber = 1;
        }

        if (model.PageSize <= 0)
        {
            model.PageSize = AppSettings.DefaultPageSize;
        }
        return query.ToOrder(model).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize);
    }

    public static IQueryable<T> ToOrder<T>(this IQueryable<T> query, PaginationRequest model, bool anotherLevel = false)
    {
        if (!string.IsNullOrEmpty(model.OrderBy))
        {
            try
            {
                ParameterExpression? param = Expression.Parameter(typeof(T), string.Empty);
                MemberExpression? property = Expression.PropertyOrField(param, model.OrderBy);
                LambdaExpression? sort = Expression.Lambda(property, param);

                MethodCallExpression? call = Expression.Call(
                    typeof(Queryable),
                    (!anotherLevel ? "OrderBy" : "ThenBy") +
                    (!model.OrderAsc ? "Descending" : string.Empty),
                    new[] { typeof(T), property.Type },
                    query.Expression,
                    Expression.Quote(sort));

                return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(call);
            }
            catch
            {
                // ignored
            }
        }
        return query;
    }
}