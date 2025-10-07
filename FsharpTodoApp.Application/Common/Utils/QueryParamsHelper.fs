namespace FsharpTodoApp.Application.Common.Utils

open Microsoft.AspNetCore.Http

module QueryParamsHelper =
    let tryGetQueryParam (ctx: HttpContext) (key: string) : string option =
        match ctx.Request.Query.TryGetValue key with
        | true, values when values.Count > 0 -> Some values.[0]
        | _ -> None
