namespace FsharpTodoApp.Domain.Common.Errors

type AppError =
    | ValidationError of string
    | NotFoundError
    | ForbiddenError
    | UnauthorizedError

module Validation =
    /// Create a validation error Result<'a, AppError> with the given message
    let error msg = Error(ValidationError msg)

    /// Create a validation error using sprintf-style formatting
    let errorf fmt =
        Printf.ksprintf (fun s -> Error(ValidationError s)) fmt
