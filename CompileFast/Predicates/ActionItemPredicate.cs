using System.Linq.Expressions;
using CompileFast.Entities;
using CompileFast.Identity;

namespace CompileFast.Predicates;

public sealed class ActionItemPredicate
{
    public interface IActionItemNullPredicate
    {
        public Expression<Func<ActionItem, bool>> PredicateWithNull();
    }

    public interface IActionItemPredicate<T> : IActionItemNullPredicate where T : IActionItemOwner
    {
        Expression<Func<ActionItem, bool>> Predicate(Id<T>? value);

#pragma warning disable CA1033
        Expression<Func<ActionItem, bool>> IActionItemNullPredicate.PredicateWithNull()
        {
            return Predicate(null);
        }
#pragma warning restore CA1033
    }

    public readonly struct AccountManagerPredicate : IActionItemPredicate<AccountManager>
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<AccountManager>? value)
        {
            return x => x.AccountManagerId == value;
        }
    }

    public readonly struct AttorneyPredicate : IActionItemPredicate<Attorney>
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<Attorney>? value)
        {
            return x => x.AttorneyId == value;
        }
    }

    public readonly struct VipPredicate : IActionItemPredicate<Vip>
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<Vip>? value)
        {
            return x => x.VipId == value;
        }
    }
}