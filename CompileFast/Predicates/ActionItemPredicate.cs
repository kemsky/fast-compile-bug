using System.Linq.Expressions;
using CompileFast.Entities;
using CompileFast.Identity;
using CompileFast.Specification;

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

        Spec<ActionItem> TasksSpec();

        SpecFactory<ActionItem, Id<T>> TasksSpecFactory();

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

        public Spec<ActionItem> TasksSpec()
        {
            return new ActionItemSpecBuilder().TasksSpec(this);
        }

        public SpecFactory<ActionItem, Id<AccountManager>> TasksSpecFactory()
        {
            return new ActionItemSpecBuilder().TasksSpecFactory(this);
        }
    }

    public readonly struct AttorneyPredicate : IActionItemPredicate<Attorney>
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<Attorney>? value)
        {
            return x => x.AttorneyId == value;
        }

        public Spec<ActionItem> TasksSpec()
        {
            return new ActionItemSpecBuilder().TasksSpec(this);
        }

        public SpecFactory<ActionItem, Id<Attorney>> TasksSpecFactory()
        {
            return new ActionItemSpecBuilder().TasksSpecFactory(this);
        }
    }

    public readonly struct VipPredicate : IActionItemPredicate<Vip>
    {
        public Expression<Func<ActionItem, bool>> Predicate(Id<Vip>? value)
        {
            return x => x.VipId == value;
        }

        public Spec<ActionItem> TasksSpec()
        {
            return new ActionItemSpecBuilder().TasksSpec(this);
        }

        public SpecFactory<ActionItem, Id<Vip>> TasksSpecFactory()
        {
            return new ActionItemSpecBuilder().TasksSpecFactory(this);
        }
    }
}