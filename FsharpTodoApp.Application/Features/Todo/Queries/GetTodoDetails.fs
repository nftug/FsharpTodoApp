namespace FsharpTodoApp.Application.Features.Todo.Queries

open FsharpTodoApp.Domain.Common.ValueObjects
open System.Threading.Tasks
open FsharpTodoApp.Application.Features.Todo.Dtos
open FsToolkit.ErrorHandling
open FsharpTodoApp.Domain.Common.Errors

type GetTodoDetails =
    { Handle: (Actor option * System.Guid -> Task<Result<TodoDetailsResponseDto, AppError>>) }

module GetTodoDetails =
    open FsharpTodoApp.Application.Features.Todo.Interfaces

    let private handle queryService (actor, id) =
        queryService.GetTodoById actor id |> TaskResult.requireSome NotFoundError

    let create (queryService: TodoQueryService) : GetTodoDetails = { Handle = handle queryService }
