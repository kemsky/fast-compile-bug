// See https://aka.ms/new-console-template for more information

using CompileFast.Entities;
using CompileFast.Predicates;
using FastExpressionCompiler;

var compileFast = new ActionItemPredicate.AccountManagerPredicate().Predicate(null).CompileFast();

compileFast(new ActionItem());