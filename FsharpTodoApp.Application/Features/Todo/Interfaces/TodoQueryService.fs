namespace FsharpTodoApp.Application.Features.Todo.Interfaces

open System.Threading.Tasks
open FsharpTodoApp.Application.Features.Todo.Dtos
open FsharpTodoApp.Domain.Common.ValueObjects

type TodoQueryService =
    { GetTodoById: Actor option -> System.Guid -> Task<Option<TodoDetailsResponseDto>>
      QueryTodos: Actor option -> TodoQueryDto -> Task<TodoPaginatedResponseDto> }
