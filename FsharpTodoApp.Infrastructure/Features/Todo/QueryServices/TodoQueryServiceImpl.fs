namespace FsharpTodoApp.Infrastructure.Features.Todo.QueryServices

open System.Linq
open Microsoft.EntityFrameworkCore
open FsToolkit.ErrorHandling
open FsharpTodoApp.Application.Features.Todo.Dtos
open FsharpTodoApp.Application.Features.Todo.Interfaces
open FsharpTodoApp.Persistence
open FsharpTodoApp.Persistence.DataModels
open FsharpTodoApp.Infrastructure.Persistence.Repositories
open FsharpTodoApp.Infrastructure.Persistence.Utils

module TodoQueryServiceImpl =
    let private getTodoById (ctx: AppDbContext) _ id =
        ctx.Todos
            .Where(fun x -> x.PublicId = id)
            .Select(fun e ->
                { Id = e.PublicId
                  Title = e.Title
                  Description = e.Description |> Option.ofObj
                  DueDate = e.DueDate |> Option.ofNullable
                  Status = e.Status
                  AssigneeUserName = e.Assignee |> Option.ofObj
                  ReviewerUserName = e.Reviewer |> Option.ofObj
                  CreatedAt = e.CreatedAt
                  CreatedByUserName = e.CreatedBy
                  UpdatedAt = e.UpdatedAt |> Option.ofNullable
                  UpdatedByUserName = e.UpdatedBy |> Option.ofObj })
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private queryTodos (ctx: AppDbContext) _ (query: TodoQueryDto) =
        task {
            let mutable queryable =
                ctx.Todos.Where(fun x -> query.Status = None || x.Status = query.Status.Value)

            queryable <-
                match query.Search with
                | None -> queryable
                | Some search ->
                    queryable.Where(
                        QueryExpressionHelper.buildCaseInsensitiveContains
                            ctx
                            [ ExprHelper.toExpression <@ fun (t: TodoDataModel) -> t.Title @>
                              ExprHelper.toExpression <@ fun (t: TodoDataModel) -> t.Description @> ]
                            search
                    )

            let! totalCount = queryable.CountAsync()

            let! results =
                queryable
                    .OrderByDescending(fun x -> x.CreatedAt)
                    .Skip(query.PageSize * (query.Page - 1))
                    .Take(query.PageSize)
                    .Select(fun e ->
                        { Id = e.PublicId
                          Title = e.Title
                          DueDate = e.DueDate |> Option.ofNullable
                          Status = e.Status
                          AssigneeUserName = e.Assignee |> Option.ofObj
                          ReviewerUserName = e.Reviewer |> Option.ofObj })
                    .ToListAsync()
                |> Task.map Seq.toList

            return TodoPaginatedResponseDto.create results totalCount query.Page query.PageSize
        }

    let create (ctx: AppDbContext) : TodoQueryService =
        { GetTodoById = getTodoById ctx
          QueryTodos = queryTodos ctx }
