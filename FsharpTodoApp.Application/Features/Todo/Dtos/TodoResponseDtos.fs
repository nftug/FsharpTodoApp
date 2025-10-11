namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Application.Common.Dtos
open System

type TodoDetailsResponseDto(id, audit, title, description, dueDate, status, assigneeUserName, reviewerUserName) =
    member val Id: Guid = id with get, set
    member val Title: string = title with get, set
    member val Description: string option = description with get, set
    member val DueDate: DateTime option = dueDate with get, set
    member val Status: TodoStatusEnum = status with get, set
    member val AssigneeUserName: string option = assigneeUserName with get, set
    member val ReviewerUserName: string option = reviewerUserName with get, set
    member val Audit: AuditDto = audit with get, set

type TodoListItemResponseDto(id, audit, title, dueDate, status, assigneeUserName, reviewerUserName) =
    member val Id: Guid = id with get, set
    member val Title: string = title with get, set
    member val DueDate: DateTime option = dueDate with get, set
    member val Status: TodoStatusEnum = status with get, set
    member val AssigneeUserName: string option = assigneeUserName with get, set
    member val ReviewerUserName: string option = reviewerUserName with get, set
    member val Audit: AuditDto = audit with get, set

type TodoPaginatedResponseDto(items, totalCount, page, pageSize) =
    inherit PaginatedResponseDto<TodoListItemResponseDto>(items, totalCount, page, pageSize)
