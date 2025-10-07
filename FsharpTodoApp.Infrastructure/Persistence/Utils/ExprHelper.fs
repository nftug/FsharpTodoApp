namespace FsharpTodoApp.Infrastructure.Persistence.Repositories

module ExprHelper =
    open Microsoft.FSharp.Linq.RuntimeHelpers
    open System.Linq.Expressions
    open System

    let toExpression<'TSource, 'TDest> (q: Quotations.Expr<'TSource -> 'TDest>) : Expression<Func<'TSource, 'TDest>> =
        LeafExpressionConverter.QuotationToExpression q :?> Expression<Func<'TSource, 'TDest>>
