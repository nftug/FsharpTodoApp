namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Application.Common.Dtos
open System

type TodoDetailsResponseDto =
    { Id: Guid
      Title: string
      Description: string option
      DueDate: DateTime option
      Status: TodoStatusEnum
      AssigneeUserName: string option
      ReviewerUserName: string option
      CreatedByUserName: string
      CreatedAt: DateTime
      UpdatedByUserName: string option
      UpdatedAt: DateTime option }

type TodoListItemResponseDto =
    { Id: Guid
      Title: string
      DueDate: DateTime option
      Status: TodoStatusEnum
      AssigneeUserName: string option
      ReviewerUserName: string option }

type TodoPaginatedResponseDto = TodoPaginatedResponseDto of PaginatedResponseDto<TodoListItemResponseDto>

module TodoPaginatedResponseDto =
    let create
        (items: TodoListItemResponseDto list)
        (totalCount: int)
        (page: int)
        (pageSize: int)
        : TodoPaginatedResponseDto =
        TodoPaginatedResponseDto(PaginatedResponseDto.create items totalCount page pageSize)
