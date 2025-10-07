namespace FsharpTodoApp.Infrastructure.Persistence.Utils

open System
open System.Linq
open System.Linq.Expressions
open Microsoft.EntityFrameworkCore

[<RequireQualifiedAccess>]
type DatabaseProvider =
    | PostgreSql
    | Sqlite
    | Other

[<RequireQualifiedAccess>]
type LogicalOperator =
    | And
    | Or

module QueryExpressionHelper =
    open Microsoft.EntityFrameworkCore.Query

    let detectProvider (context: DbContext) : DatabaseProvider =
        let providerName =
            try
                context.Database.ProviderName
            with _ ->
                String.Empty

        if String.IsNullOrWhiteSpace providerName then
            DatabaseProvider.Other
        else
            let normalized = providerName.ToLowerInvariant()

            if normalized.Contains "npgsql" then
                DatabaseProvider.PostgreSql
            elif normalized.Contains "sqlite" then
                DatabaseProvider.Sqlite
            else
                DatabaseProvider.Other

    let private requireMethod methodInfo errorMessage =
        match methodInfo with
        | null -> invalidOp errorMessage
        | info -> info

    let private likeMethod =
        requireMethod
            (typeof<DbFunctionsExtensions>
                .GetMethod("Like", [| typeof<DbFunctions>; typeof<string>; typeof<string> |]))
            "Unable to locate EF.Functions.Like(DbFunctions, string, string)."

    let private iLikeMethod =
        // Try to find ILike; if not present we will fallback to LIKE at runtime
        typeof<DbFunctionsExtensions>
            .GetMethod("ILike", [| typeof<DbFunctions>; typeof<string>; typeof<string> |])

    let private toLowerMethod =
        requireMethod
            (typeof<string>.GetMethod("ToLower", Type.EmptyTypes))
            "Unable to locate string.ToLower method info."

    let private collateMethod =
        // EF Core exposes DbFunctionsExtensions.Collate(DbFunctions, string, string) in newer versions
        typeof<DbFunctionsExtensions>
            .GetMethod("Collate", [| typeof<DbFunctions>; typeof<string>; typeof<string> |])

    let private efFunctions () =
        Expression.Property(null, typeof<EF>, nameof EF.Functions)

    let private combineBodies combiner head tail = tail |> List.fold combiner head

    let private createLikeBody (context: DbContext) (propertyExpression: Expression) (trimmed: string) =
        let pattern = "%" + trimmed + "%"

        let provider = detectProvider context

        let likeCall =
            match provider with
            | DatabaseProvider.PostgreSql ->
                // Use ILike directly on Postgres (case-insensitive) - no LOWER. If ILike method missing, fallback to LIKE
                if isNull iLikeMethod then
                    Expression.Call(likeMethod, efFunctions (), propertyExpression, Expression.Constant(pattern))
                else
                    Expression.Call(iLikeMethod, efFunctions (), propertyExpression, Expression.Constant(pattern))
            | DatabaseProvider.Sqlite ->
                // Prefer EF.Functions.Collate(column, "NOCASE") when available to keep indexability.
                if not (isNull collateMethod) then
                    // Call Collate: DbFunctionsExtensions.Collate(EF.Functions, column, "NOCASE")
                    Expression.Call(collateMethod, efFunctions (), propertyExpression, Expression.Constant("NOCASE"))
                    |> fun collated ->
                        Expression.Call(likeMethod, efFunctions (), collated, Expression.Constant(pattern))
                else
                    // Fallback to ToLower comparison if Collate not available
                    let loweredProperty = Expression.Call(propertyExpression, toLowerMethod)
                    let loweredPattern = pattern.ToLowerInvariant()

                    Expression.Call(likeMethod, efFunctions (), loweredProperty, Expression.Constant(loweredPattern))
            | DatabaseProvider.Other ->
                // Default: use LIKE as-is (no LOWER)
                Expression.Call(likeMethod, efFunctions (), propertyExpression, Expression.Constant(pattern))

        let notNull =
            Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof<string>))

        Expression.AndAlso(notNull, likeCall)

    let buildCaseInsensitiveContains<'TEntity>
        (context: DbContext)
        (propertySelectors: Expression<Func<'TEntity, string>> list)
        (searchTerm: string)
        : Expression<Func<'TEntity, bool>> =
        let selectors =
            propertySelectors
            |> List.choose (fun selector -> if isNull (box selector) then None else Some selector)

        match selectors with
        | [] -> invalidArg (nameof propertySelectors) "At least one property selector must be provided."
        | firstSelector :: _ ->
            let parameter =
                Expression.Parameter(typeof<'TEntity>, firstSelector.Parameters.[0].Name)

            if String.IsNullOrWhiteSpace searchTerm then
                Expression.Lambda<Func<'TEntity, bool>>(Expression.Constant true, parameter)
            else
                let trimmed = searchTerm.Trim()

                let bodies =
                    selectors
                    |> List.map (fun selector ->
                        if selector.ReturnType <> typeof<string> then
                            invalidArg (nameof propertySelectors) "All selectors must target string properties."

                        let propertyExpression =
                            ReplacingExpressionVisitor.Replace(selector.Parameters.[0], parameter, selector.Body)

                        createLikeBody context propertyExpression trimmed)

                match bodies with
                | [] -> Expression.Lambda<Func<'TEntity, bool>>(Expression.Constant true, parameter)
                | firstBody :: tail ->
                    let combinedBody =
                        combineBodies (fun left right -> Expression.OrElse(left, right)) firstBody tail

                    Expression.Lambda<Func<'TEntity, bool>>(combinedBody, parameter)

    let combinePredicates<'TEntity>
        (logicalOperator: LogicalOperator)
        (predicates: seq<Expression<Func<'TEntity, bool>>>)
        : Expression<Func<'TEntity, bool>> option =
        let validPredicates =
            predicates
            |> Seq.choose (fun predicate -> if isNull (box predicate) then None else Some predicate)
            |> Seq.toList

        match validPredicates with
        | [] -> None
        | head :: tail ->
            let parameter = head.Parameters.[0]

            let combine =
                match logicalOperator with
                | LogicalOperator.And -> fun left right -> Expression.AndAlso(left, right) :> Expression
                | LogicalOperator.Or -> fun left right -> Expression.OrElse(left, right) :> Expression

            let bodies =
                tail
                |> List.map (fun predicate ->
                    ReplacingExpressionVisitor.Replace(predicate.Parameters.[0], parameter, predicate.Body))

            let combinedBody = combineBodies combine head.Body bodies
            Some(Expression.Lambda<Func<'TEntity, bool>>(combinedBody, parameter))

    let applyCombined<'TEntity>
        (source: IQueryable<'TEntity>)
        (logicalOperator: LogicalOperator)
        (predicates: seq<Expression<Func<'TEntity, bool>>>)
        : IQueryable<'TEntity> =
        match combinePredicates logicalOperator predicates with
        | Some predicate -> source.Where predicate
        | None -> source

    let applyPredicate<'TEntity> (source: IQueryable<'TEntity>) (predicate: Expression<Func<'TEntity, bool>>) =
        if isNull (box predicate) then
            source
        else
            source.Where predicate
