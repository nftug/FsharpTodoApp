namespace FsharpTodoApp.Application.Common.Utils

module AsyncResult =
    open FsToolkit.ErrorHandling

    let fromOption error fetchAsync =
        asyncResult {
            let! optionValue = fetchAsync |> AsyncResult.ofAsync

            match optionValue with
            | Some value -> return value
            | None -> return! Error error
        }
