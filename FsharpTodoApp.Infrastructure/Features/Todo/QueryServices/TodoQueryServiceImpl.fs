namespace FsharpTodoApp.Infrastructure.Features.Todo.QueryServices

open System.Linq
open Microsoft.EntityFrameworkCore
open FsToolkit.ErrorHandling
open FsharpTodoApp.Application.Features.Todo.Dtos
open FsharpTodoApp.Application.Features.Todo.Interfaces
open FsharpTodoApp.Persistence
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Infrastructure.Features.Todo.DataModels

module TodoQueryServiceImpl =
    let private getTodoById (ctx: AppDbContext) (actor: Actor option) id =
        (TodoDataModelHelper.applyFilter ctx.Todos actor)
            .Where(fun x -> x.PublicId = id)
            .Select(fun e ->
                new TodoDetailsResponseDto(
                    e.PublicId,
                    e.Title,
                    Option.ofObj e.Description,
                    Option.ofNullable e.DueDate,
                    e.Status,
                    Option.ofObj e.Assignee,
                    Option.ofObj e.Reviewer,
                    e.CreatedAt,
                    e.CreatedBy,
                    Option.ofNullable e.UpdatedAt,
                    Option.ofObj e.UpdatedBy
                ))
            .SingleOrDefaultAsync()
        |> Task.map Option.ofObj

    let private queryTodos (ctx: AppDbContext) (actor: Actor option) (query: TodoQueryDto) =
        task {
            let queryable =
                (TodoDataModelHelper.applyFilter ctx.Todos actor)
                    .Where(fun x -> query.Status = None || x.Status = query.Status.Value)

            let queryable =
                match query.Search with
                | None -> queryable
                | Some search ->
                    let lowerSearch = search.ToLower()

                    queryable.Where(fun t ->
                        t.Title.ToLower().Contains lowerSearch
                        || t.Description.ToLower().Contains lowerSearch)

            let! totalCount = queryable.CountAsync()

            let! results =
                queryable
                    .OrderByDescending(fun x -> x.CreatedAt)
                    .Skip(query.PageSize * (query.Page - 1))
                    .Take(query.PageSize)
                    .Select(fun e ->
                        new TodoListItemResponseDto(
                            e.PublicId,
                            e.Title,
                            Option.ofNullable e.DueDate,
                            e.Status,
                            Option.ofObj e.Assignee,
                            Option.ofObj e.Reviewer
                        ))
                    .ToListAsync()
                |> Task.map Seq.toList

            return TodoPaginatedResponseDto.create results totalCount query.Page query.PageSize
        }

    let create (ctx: AppDbContext) : TodoQueryService =
        { GetTodoById = getTodoById ctx
          QueryTodos = queryTodos ctx }
