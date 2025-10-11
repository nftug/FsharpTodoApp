namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums
open FsharpTodoApp.Application.Common.Dtos
open System

type TodoDetailsResponseDto
    (
        id,
        title,
        description,
        dueDate,
        status,
        assigneeUserName,
        reviewerUserName,
        createdAt,
        createdBy,
        updatedAt,
        updatedBy
    ) =
    member val Id: Guid = id with get, set
    member val Title: string = title with get, set
    member val Description: string option = description with get, set
    member val DueDate: DateTime option = dueDate with get, set
    member val Status: TodoStatusEnum = status with get, set
    member val AssigneeUserName: string option = assigneeUserName with get, set
    member val ReviewerUserName: string option = reviewerUserName with get, set
    member val CreatedAt: DateTime = createdAt with get, set
    member val CreatedBy: string = createdBy with get, set
    member val UpdatedAt: DateTime option = updatedAt with get, set
    member val UpdatedBy: string option = updatedBy with get, set

type TodoListItemResponseDto(id, title, dueDate, status, assigneeUserName, reviewerUserName) =
    member val Id: Guid = id with get, set
    member val Title: string = title with get, set
    member val DueDate: DateTime option = dueDate with get, set
    member val Status: TodoStatusEnum = status with get, set
    member val AssigneeUserName: string option = assigneeUserName with get, set
    member val ReviewerUserName: string option = reviewerUserName with get, set

type TodoPaginatedResponseDto = PaginatedResponseDto<TodoListItemResponseDto>

module TodoPaginatedResponseDto =
    let create
        (items: TodoListItemResponseDto list)
        (totalCount: int)
        (page: int)
        (pageSize: int)
        : TodoPaginatedResponseDto =
        PaginatedResponseDto.create items totalCount page pageSize
