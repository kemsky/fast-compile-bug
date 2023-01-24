using System.Linq.Expressions;
using CompileFast.Entities;
using CompileFast.Identity;

namespace CompileFast.Predicates;

public sealed class ActionItemPredicate
{
    public readonly struct AccountManagerPredicate
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<AccountManager>? value)
        {
            return x => x.AccountManagerId == value;
        }
    }
}