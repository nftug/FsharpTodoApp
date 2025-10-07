namespace FsharpTodoApp.Domain.Common.Policies

open FsharpTodoApp.Domain.Common.ValueObjects

module PassThroughPermission =
    let create () : Permission =
        { CanCreate = true
          CanUpdate = true
          CanDelete = true }
