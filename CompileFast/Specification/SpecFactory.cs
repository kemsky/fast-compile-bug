using System.Linq.Expressions;

namespace CompileFast.Specification;

public readonly struct SpecFactory<TSource>
{
    public Spec<TSource> For() => new Spec<TSource>(_expressionFactory(), _delegateFactory());


    private readonly Func<Expression<Func<TSource, bool>>> _expressionFactory;


    private readonly Func<Func<TSource, bool>> _delegateFactory;

    public SpecFactory(Func<Expression<Func<TSource, bool>>> expressionFactory, Func<Func<TSource, bool>> delegateFactory)
    {
        _expressionFactory = expressionFactory;
        _delegateFactory = delegateFactory;
    }
}

public readonly struct SpecFactory<TSource, TParam>
{
    public Spec<TSource> For(TParam param) => new Spec<TSource>(_expressionFactory(param), _delegateFactory(param));


    private readonly Func<TParam, Expression<Func<TSource, bool>>> _expressionFactory;


    private readonly Func<TParam, Func<TSource, bool>> _delegateFactory;

    public SpecFactory(Func<TParam, Expression<Func<TSource, bool>>> expressionFactory, Func<TParam, Func<TSource, bool>> delegateFactory)
    {
        _expressionFactory = expressionFactory;
        _delegateFactory = delegateFactory;
    }
}

public readonly struct SpecFactory<TSource, TParam1, TParam2>
{
    public Spec<TSource> For(TParam1 param1, TParam2 param2) => new Spec<TSource>(_expressionFactory(param1, param2), _delegateFactory(param1, param2));


    private readonly Func<TParam1, TParam2, Expression<Func<TSource, bool>>> _expressionFactory;


    private readonly Func<TParam1, TParam2, Func<TSource, bool>> _delegateFactory;

    public SpecFactory(Func<TParam1, TParam2, Expression<Func<TSource, bool>>> expressionFactory, Func<TParam1, TParam2, Func<TSource, bool>> delegateFactory)
    {
        _expressionFactory = expressionFactory;
        _delegateFactory = delegateFactory;
    }
}