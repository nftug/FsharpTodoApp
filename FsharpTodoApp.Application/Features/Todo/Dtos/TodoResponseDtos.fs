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
      CreatedAt: DateTime
      CreatedByUserName: string
      UpdatedAt: DateTime option
      UpdatedByUserName: string option }

type TodoListItemResponseDto =
    { Id: Guid
      Title: string
      DueDate: DateTime option
      Status: TodoStatusEnum
      AssigneeUserName: string option
      ReviewerUserName: string option }

type TodoPaginatedResponseDto = PaginatedResponseDto<TodoListItemResponseDto>

module TodoPaginatedResponseDto =
    let create
        (items: TodoListItemResponseDto list)
        (totalCount: int)
        (page: int)
        (pageSize: int)
        : TodoPaginatedResponseDto =
        PaginatedResponseDto.create items totalCount page pageSize
