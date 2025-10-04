namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums

type TodoQueryDto =
    { Search: string option
      Status: TodoStatusEnum option
      Page: int
      PageSize: int }

module TodoQueryDto =
    let ensureValidity (dto: TodoQueryDto) =
        let page = if dto.Page < 1 then 1 else dto.Page

        let pageSize =
            if dto.PageSize < 1 then 10
            elif dto.PageSize > 100 then 100
            else dto.PageSize

        { dto with
            Page = page
            PageSize = pageSize }
