namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Application.Common.Dtos

type TodoDetailsResponseDto =
    { Id: System.Guid
      Title: string
      Description: string option
      DueDate: System.DateTime option
      Status: TodoStatusEnum
      AssigneeUserName: string option
      ReviewerUserName: string option
      CreatedByUserName: string
      CreatedAt: System.DateTime
      UpdatedByUserName: string option
      UpdatedAt: System.DateTime option }

type TodoListItemResponseDto =
    { Id: System.Guid
      Title: string
      DueDate: System.DateTime option
      Status: TodoStatusEnum
      AssigneeUserName: string option
      ReviewerUserName: string option }

type TodoPaginatedResponseDto = PaginatedResponseDto<TodoListItemResponseDto>

module TodoPaginatedResponseDto =
    let create (items: TodoListItemResponseDto list) totalCount page pageSize =
        PaginatedResponseDto.create items totalCount page pageSize
