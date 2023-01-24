using CompileFast.Entities;
using CompileFast.Identity;
using CompileFast.Specification;

namespace CompileFast.Predicates;

public class ActionItemSpecBuilder
{
    private static readonly Dictionary<Type, ActionItemPredicate.IActionItemNullPredicate> NegativeExpressions = new Dictionary<Type, ActionItemPredicate.IActionItemNullPredicate>();

#pragma warning disable CA1065
    static ActionItemSpecBuilder()
    {
        var expressions = new HashSet<string>();

        foreach (var type in typeof(ActionItemPredicate).GetNestedTypes())
        {
            if (type.IsValueType)
            {
                var predicate = ((ActionItemPredicate.IActionItemNullPredicate)Activator.CreateInstance(type));

                var expression = predicate.PredicateWithNull();

                var expressionKey = expression.ToString();

                if (!expressions.Add(expressionKey))
                {
                    throw new Exception($"Duplicate predicate expression: {expressionKey}");
                }

                NegativeExpressions[type] = predicate;
            }
            else if (!type.IsInterface)
            {
                throw new Exception($"Predicate type must be a struct: {type.FullName}");
            }
        }
    }
#pragma warning restore CA1065

    public Spec<ActionItem> TasksSpec<TOwner>(ActionItemPredicate.IActionItemPredicate<TOwner> predicate) where TOwner : IActionItemOwner
    {
        var type = predicate.GetType();

        return NegativeExpressions.Where(x => x.Key != type).Select(x => x.Value).Select(x => new Spec<ActionItem>(x.PredicateWithNull())).Aggregate((spec1, spec2) => spec1 && spec2);
    }

    public SpecFactory<ActionItem, Id<TOwner>> TasksSpecFactory<TOwner>(ActionItemPredicate.IActionItemPredicate<TOwner> predicate) where TOwner : IActionItemOwner
    {
        return new SpecFactory<ActionItem, Id<TOwner>>(
            id => predicate.Predicate(id),
            id => predicate.Predicate(id).Compile() // todo: CompileFast() does not work, file bug report to https://github.com/dadhi/FastExpressionCompiler
        );
    }
}