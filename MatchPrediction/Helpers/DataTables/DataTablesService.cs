using MatchPrediction.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace MatchPrediction.Helpers.DataTables
{
    public class DataTablesService<T>
    {
        private readonly ILogger<T> _logger;
        private readonly MatchPredictionContext _db;
        
        private IQueryable<T> SearchSingleString<T>(IQueryable<T> query, string value)
        {
            List<Expression> expressions = new List<Expression>();
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)))
            {
                MemberExpression member_expression = Expression.PropertyOrField(parameter, prop.Name);
                ConstantExpression value_expression = Expression.Constant(value, typeof(string));
                MethodCallExpression contains_expression = Expression.Call(member_expression, contains_method, value_expression);
                expressions.Add(contains_expression);
            }

            if (expressions.Count == 0)
                return query;

            Expression or_expression = expressions[0];

            for (int i = 1; i < expressions.Count; i++)
            {
                or_expression = Expression.OrElse(or_expression, expressions[i]);
            }

            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(
                or_expression, parameter);

            return query.Where(expression);
        }
        public DataTablesService(
                ILogger<T> logger,
                MatchPredictionContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IQueryable<T> Paginate<T>(IQueryable<T> q, int start, int pageSize)
        {
            return q.Skip(start).Take(pageSize);
        }

        public IQueryable<T> Search<T>(IQueryable<T> query, string searchString)
        {
            if (searchString == null || searchString.Length < 2)
                return query;

            var values_to_search = searchString.Split(',', ' ');
            foreach (var value in values_to_search)
            {
                query = SearchSingleString(query, value);
            }

            return query;
        }

        public async Task<DataTableAjaxResponseModel<T>> GenerateDataTableResponse<T>(DataTableAjaxPostModel model, IQueryable<T> query)
        {
            var total = await query.CountAsync();

            query = Search(query, model.search.value);
            var filtered_total = await query.CountAsync();

            query = Paginate(query, model.start, model.length);

            var result = await query.ToListAsync();
            
            return new DataTableAjaxResponseModel<T> {
                draw = model.draw,
                recordsTotal = total,
                recordsFiltered = filtered_total,
                data = result
            };
        }
    }
}
