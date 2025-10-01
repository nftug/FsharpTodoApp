namespace FsharpTodoApp.Domain.Common.ValueObjects

type Permission =
    { CanCreate: bool
      CanUpdate: bool
      CanDelete: bool }
