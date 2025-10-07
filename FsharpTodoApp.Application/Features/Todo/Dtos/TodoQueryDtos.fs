namespace FsharpTodoApp.Application.Features.Todo.Dtos

open FsharpTodoApp.Domain.Features.Todo.Enums
open System.Collections.Generic

type TodoQueryDto =
    { Search: string option
      Status: TodoStatusEnum option
      Page: int
      PageSize: int }

module TodoQueryDto =
    let ensureValidity (dto: TodoQueryDto) : TodoQueryDto =
        let page = if dto.Page < 1 then 1 else dto.Page

        let pageSize =
            if dto.PageSize < 1 then 10
            elif dto.PageSize > 100 then 100
            else dto.PageSize

        { dto with
            Page = page
            PageSize = pageSize }

    let bindQueryParams (queryParams: IDictionary<string, string>) : TodoQueryDto =
        let tryGetValue key =
            match queryParams.TryGetValue key with
            | true, value when not (System.String.IsNullOrWhiteSpace value) -> Some value
            | _ -> None

        let search = tryGetValue "search"

        let status =
            tryGetValue "status"
            |> Option.bind (fun s ->
                match System.Enum.TryParse<TodoStatusEnum>(s, true) with
                | true, enumValue -> Some enumValue
                | _ -> None)

        let page =
            tryGetValue "page"
            |> Option.bind (fun s ->
                match System.Int32.TryParse(s) with
                | true, intValue -> Some intValue
                | _ -> None)
            |> Option.defaultValue 1

        let pageSize =
            tryGetValue "pageSize"
            |> Option.bind (fun s ->
                match System.Int32.TryParse(s) with
                | true, intValue -> Some intValue
                | _ -> None)
            |> Option.defaultValue 10

        { Search = search
          Status = status
          Page = page
          PageSize = pageSize }
