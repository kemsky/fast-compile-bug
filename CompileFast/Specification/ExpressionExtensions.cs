using System.Linq.Expressions;
using System.Reflection;

namespace CompileFast.Specification;

/// <summary>
/// Enables the efficient, dynamic composition of query predicates.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>()
    {
        return param => true;
    }

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>()
    {
        return param => false;
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.OrElse);
    }

    /// <summary>
    /// Negates the predicate.
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters
            .Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterReBinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }


    public static Expression<Func<TDestination, TReturn>> From<TSource, TDestination, TReturn>(
        this Expression<Func<TSource, TReturn>> source,
        Expression<Func<TDestination, TSource>> mapFrom
    )
    {
        return Expression.Lambda<Func<TDestination, TReturn>>(Expression.Invoke(source, mapFrom.Body), mapFrom.Parameters);
    }

    public static TExpression BindToLocal<TExpression, TParam1>(
        this TExpression expression,
        Expression<Func<TParam1>> property1,
        ParameterInfo parameter1
    ) where TExpression : Expression
    {
        var replacement1 = property1.Body;

        return (TExpression)new RebindArgument(parameter1, replacement1).Visit(expression);
    }


    public static Expression<Func<TSource, TResultTo>> BindToExpression<TSource, TResultFrom, TResultTo>(
        this Expression<Func<TSource, TResultFrom>> expression,
        Expression<Func<TResultFrom, TResultTo>> transformer
    )
    {
        var body = new RebindParameter(transformer.Parameters[0], expression.Body).Visit(transformer.Body);

        return Expression.Lambda<Func<TSource, TResultTo>>(body, expression.Parameters);
    }

    public static TExpression Rebind<TExpression, TParam1, TParam2>(
        this TExpression expression,
        Expression<Func<TParam1>> property1,
        Expression<Func<TParam2>> property2,
        ParameterInfo parameter1,
        ParameterInfo parameter2
    ) where TExpression : Expression
    {
        var replacement1 = property1.Body;
        var replacement2 = property2.Body;

        return (TExpression)new RebindArgument(parameter2, replacement2).Visit(new RebindArgument(parameter1, replacement1).Visit(expression));
    }

    private class RebindArgument : ExpressionVisitor
    {
        private readonly ParameterInfo _parameter;
        private readonly Expression _replace;

        public RebindArgument(ParameterInfo parameter, Expression replace)
        {
            _parameter = parameter;
            _replace = replace;
        }

        public override Expression Visit(Expression node)
        {
            if (node is MemberExpression memberExpression && memberExpression.Member.Name == _parameter.Name && memberExpression.Member.DeclaringType?.DeclaringType == _parameter.Member.DeclaringType?.DeclaringType)
            {
                return _replace;
            }
            else
            {
                return base.Visit(node);
            }
        }
    }

    private class ParameterReBinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterReBinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterReBinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }

    private class RebindParameter : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly Expression _replace;

        public RebindParameter(ParameterExpression parameter, Expression replace)
        {
            _parameter = parameter;
            _replace = replace;
        }

        public override Expression Visit(Expression node)
        {
            if (node is ParameterExpression parameterExpression && parameterExpression == _parameter)
            {
                return _replace;
            }
            else
            {
                return base.Visit(node);
            }
        }
    }
}