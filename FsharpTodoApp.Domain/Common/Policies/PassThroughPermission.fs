namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

module PassThroughPermission =
    let create () =
        { CanCreate = true
          CanUpdate = true
          CanDelete = true }
