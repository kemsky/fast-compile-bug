using System.Linq.Expressions;
using FastExpressionCompiler;

namespace CompileFast.Specification;

public readonly struct Spec<TSource>
{
    public Expression<Func<TSource, bool>> Expression => LazyExpression.Value;

    public Func<TSource, bool> Delegate => LazyDelegate.Value;

    private Lazy<Func<TSource, bool>> LazyDelegate { get; }

    private Lazy<Expression<Func<TSource, bool>>> LazyExpression { get; }

    public Spec(Expression<Func<TSource, bool>> expression)
    {
        LazyExpression = new Lazy<Expression<Func<TSource, bool>>>(() => expression);

        var lazyExpression = LazyExpression;

        // todo: CompileFast() does not work, file bug report to https://github.com/dadhi/FastExpressionCompiler
        LazyDelegate = new Lazy<Func<TSource, bool>>(() => lazyExpression.Value.CompileFast());
    }

    internal Spec(Expression<Func<TSource, bool>> expressionFunc, Func<TSource, bool> delegateFunc)
    {
        LazyExpression = new Lazy<Expression<Func<TSource, bool>>>(() => expressionFunc);
        LazyDelegate = new Lazy<Func<TSource, bool>>(() => delegateFunc);
    }

    private Spec(Spec<TSource> spec)
    {
        LazyExpression = new Lazy<Expression<Func<TSource, bool>>>(() => spec.Expression.Not());
        LazyDelegate = new Lazy<Func<TSource, bool>>(() => x => !spec.Delegate(x));
    }

    private Spec(bool or, Spec<TSource> spec1, Spec<TSource> spec2)
    {
        if (or)
        {
            LazyExpression = new Lazy<Expression<Func<TSource, bool>>>(() => spec1.Expression.Or(spec2.Expression));
            LazyDelegate = new Lazy<Func<TSource, bool>>(() => x => spec1.Delegate(x) || spec2.Delegate(x));
        }
        else
        {
            LazyExpression = new Lazy<Expression<Func<TSource, bool>>>(() => spec1.Expression.And(spec2.Expression));
            LazyDelegate = new Lazy<Func<TSource, bool>>(() => x => spec1.Delegate(x) && spec2.Delegate(x));
        }
    }

    public static Spec<TSource> operator &(Spec<TSource> spec1, Spec<TSource> spec2)
    {
        return new Spec<TSource>(or: false, spec1, spec2);
    }

    public static Spec<TSource> operator |(Spec<TSource> spec1, Spec<TSource> spec2)
    {
        return new Spec<TSource>(or: true, spec1, spec2);
    }

    public static Spec<TSource> operator !(Spec<TSource> spec)
    {
        return new Spec<TSource>(spec);
    }

    public static bool operator false(Spec<TSource> f) => false;

    public static bool operator true(Spec<TSource> f) => false;

    public Spec<TProjection> From<TProjection>(Expression<Func<TProjection, TSource>> expression)
    {
        return new Spec<TProjection>(Expression.From(expression));
    }

    public static implicit operator Func<TSource, bool>(Spec<TSource> f)
    {
        return f.Delegate;
    }

    public static implicit operator Expression<Func<TSource, bool>>(Spec<TSource> f)
    {
        return f.Expression;
    }
}

public readonly struct Spec<TSource, TResult>
{
    public Expression<Func<TSource, TResult>> Expression => LazyExpression.Value;


    public Func<TSource, TResult> Delegate => LazyDelegate.Value;


    private Lazy<Func<TSource, TResult>> LazyDelegate { get; }


    private Lazy<Expression<Func<TSource, TResult>>> LazyExpression { get; }

    public Spec(Expression<Func<TSource, TResult>> expression)
    {
        LazyExpression = new Lazy<Expression<Func<TSource, TResult>>>(() => expression);

        var lazyExpression = LazyExpression;

        LazyDelegate = new Lazy<Func<TSource, TResult>>(() => lazyExpression.Value.CompileFast());
    }

    internal Spec(Expression<Func<TSource, TResult>> expressionFunc, Func<TSource, TResult> delegateFunc)
    {
        LazyExpression = new Lazy<Expression<Func<TSource, TResult>>>(() => expressionFunc);
        LazyDelegate = new Lazy<Func<TSource, TResult>>(() => delegateFunc);
    }

    public Spec<TProjection, TResult> From<TProjection>(Expression<Func<TProjection, TSource>> expression)
    {
        return new Spec<TProjection, TResult>(Expression.From(expression));
    }

    public static implicit operator Func<TSource, TResult>(Spec<TSource, TResult> f)
    {
        return f.Delegate;
    }

    public static implicit operator Expression<Func<TSource, TResult>>(Spec<TSource, TResult> f)
    {
        return f.Expression;
    }
}