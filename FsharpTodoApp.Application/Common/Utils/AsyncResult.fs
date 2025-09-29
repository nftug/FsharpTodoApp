namespace FsharpTodoApp.Application.Common.Utils

module AsyncResult =
    open FsToolkit.ErrorHandling

    let requireSomeAsync error fetchAsync =
        asyncResult {
            let! optionValue = fetchAsync |> AsyncResult.ofAsync

            match optionValue with
            | Some value -> return value
            | None -> return! Error error
        }

    let maybeFetchAsync error fetchAsync optionParam =
        match optionParam with
        | Some param -> fetchAsync param |> requireSomeAsync error |> AsyncResult.map Some
        | None -> asyncResult { return None }
