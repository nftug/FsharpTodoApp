namespace FsharpTodoApp.Infrastructure.Persistence.Utils

module ExprHelper =
    open Microsoft.FSharp.Linq.RuntimeHelpers
    open System.Linq.Expressions

    let toExpression<'TSource, 'TDest> (q: Quotations.Expr<'TSource -> 'TDest>) =
        LeafExpressionConverter.QuotationToExpression q :?> Expression<System.Func<'TSource, 'TDest>>
