// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;
using CompileFast.Identity;
using FastExpressionCompiler;

Expression<Func<ActionItem, bool>> Predicate(Id<AccountManager>? value)
{
    return x => x.AccountManagerId == value;
}

var compileFast = Predicate(null).CompileFast();

compileFast(new ActionItem());

public class AccountManager
{
}

public class ActionItem
{
    public long? AccountManagerId { get; set; }
}