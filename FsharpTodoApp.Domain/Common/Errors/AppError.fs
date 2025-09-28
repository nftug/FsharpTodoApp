namespace FsharpTodoApp.Domain.Common.Errors

type AppError =
    | ValidationError of string
    | NotFoundError
    | ForbiddenError
    | UnauthorizedError

module AppError =
    let validationError msg = Error(ValidationError msg)
    let notFoundError = Error NotFoundError
    let forbiddenError = Error ForbiddenError
    let unauthorizedError = Error UnauthorizedError
