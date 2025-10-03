namespace FsharpTodoApp.Domain.Features.User.Policies

open FsharpTodoApp.Domain.Common.Policies
open FsharpTodoApp.Domain.Features.User.Entities

module UserCreationPermission =
    let create = AdminOnlyPermission.create

module UserUpdatePermission =
    let create actor entity =
        OwnerOnlyPermission.create actor entity.Base

module UserDeletionPermission =
    let create actor entity =
        OwnerOnlyPermission.create actor entity.Base
