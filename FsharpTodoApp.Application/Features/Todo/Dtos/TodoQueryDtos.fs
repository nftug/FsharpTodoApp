namespace FsharpTodoApp.Application.Features.Todo.Dtos

open System
open FsharpTodoApp.Domain.Features.Todo.Enums
open Microsoft.AspNetCore.Http
open FsharpTodoApp.Application.Common.Utils

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

    let bindQueryParams (ctx: HttpContext) : TodoQueryDto =
        let search = QueryParamsHelper.tryGetQueryParam ctx "search"

        let status =
            QueryParamsHelper.tryGetQueryParam ctx "status"
            |> Option.bind (fun s ->
                match Enum.TryParse<TodoStatusEnum>(s, true) with
                | true, enumValue -> Some enumValue
                | _ -> None)

        let page =
            QueryParamsHelper.tryGetQueryParam ctx "page"
            |> Option.bind (fun s ->
                match Int32.TryParse s with
                | true, intValue -> Some intValue
                | _ -> None)
            |> Option.defaultValue 1

        let pageSize =
            QueryParamsHelper.tryGetQueryParam ctx "pageSize"
            |> Option.bind (fun s ->
                match Int32.TryParse s with
                | true, intValue -> Some intValue
                | _ -> None)
            |> Option.defaultValue 10

        { Search = search
          Status = status
          Page = page
          PageSize = pageSize }
