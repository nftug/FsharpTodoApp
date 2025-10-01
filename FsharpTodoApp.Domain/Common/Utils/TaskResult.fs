namespace FsharpTodoApp.Domain.Common.Utils

module TaskResult =
    open FsToolkit.ErrorHandling

    let maybeFetch error fetchTaskFn =
        function
        | Some arg -> fetchTaskFn arg |> TaskResult.requireSome error |> TaskResult.map Some
        | None -> taskResult { return None }
