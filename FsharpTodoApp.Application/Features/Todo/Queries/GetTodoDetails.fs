namespace FsharpTodoApp.Application.Features.Todo.Queries

open FsharpTodoApp.Domain.Common.ValueObjects
open System.Threading.Tasks
open FsharpTodoApp.Application.Features.Todo.Dtos

type GetTodoDetails =
    { Handle: (Actor option * System.Guid -> Task<Option<TodoDetailsResponseDto>>) }

module GetTodoDetails =
    open FsharpTodoApp.Application.Features.Todo.Interfaces

    let private handle (queryService: TodoQueryService) (actor, id) = queryService.GetTodoById actor id

    let create (queryService: TodoQueryService) = { Handle = handle queryService }
