namespace FsharpTodoApp.Domain.Common.ValueObjects

type EntityAudit =
    { UserInfo: UserInfo
      Timestamp: System.DateTime }

module private EntityAudit =
    let create ctx =
        { UserInfo = ctx.Actor.UserInfo
          Timestamp = ctx.DateTime.UtcNow() }

    let hydrate (timestamp, userName) =
        { UserInfo = { UserName = userName }
          Timestamp = timestamp }

type CreatedAudit = private CreatedAudit of EntityAudit

module CreatedAudit =
    let create ctx = EntityAudit.create ctx |> CreatedAudit

    let hydrate (timestamp, userName) =
        EntityAudit.hydrate (timestamp, userName) |> CreatedAudit

    let value (CreatedAudit audit) = audit

type UpdatedAudit = private UpdatedAudit of EntityAudit option

module UpdatedAudit =
    let create ctx =
        EntityAudit.create ctx |> Some |> UpdatedAudit

    let hydrate (timestamp, userName) =
        match timestamp, userName with
        | Some ts, Some user -> EntityAudit.hydrate (ts, user) |> Some |> UpdatedAudit
        | _ -> UpdatedAudit None

    let none = UpdatedAudit None

    let value (UpdatedAudit auditOpt) = auditOpt

type DeletedAudit = private DeletedAudit of EntityAudit option

module DeletedAudit =
    let create ctx =
        EntityAudit.create ctx |> Some |> DeletedAudit

    let hydrate (timestamp, userName) =
        match timestamp, userName with
        | Some ts, Some user -> EntityAudit.hydrate (ts, user) |> Some |> DeletedAudit
        | _ -> DeletedAudit None

    let none = DeletedAudit None

    let value (DeletedAudit auditOpt) = auditOpt
