namespace FsharpTodoApp.Application.Features.Todo.Queries

open FsharpTodoApp.Domain.Common.ValueObjects
open System.Threading.Tasks
open FsharpTodoApp.Application.Features.Todo.Dtos

type QueryTodos =
    { Handle: (Actor option * TodoQueryDto -> Task<TodoPaginatedResponseDto>) }

module QueryTodos =
    open FsharpTodoApp.Application.Features.Todo.Interfaces

    let private handle queryService (actor, queryDto) = queryService.QueryTodos actor queryDto

    let create (queryService: TodoQueryService) : QueryTodos = { Handle = handle queryService }
