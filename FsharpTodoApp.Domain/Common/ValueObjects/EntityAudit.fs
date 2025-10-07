namespace FsharpTodoApp.Domain.Common.ValueObjects

type EntityAudit =
    { UserInfo: UserInfo
      Timestamp: System.DateTime }

module private EntityAudit =
    let create (ctx: AuditContext) : EntityAudit =
        { UserInfo = ctx.Actor.UserInfo
          Timestamp = ctx.DateTime.UtcNow() }

    let hydrate (timestamp: System.DateTime, userName: string) : EntityAudit =
        { UserInfo = { UserName = userName }
          Timestamp = timestamp }

type CreatedAudit = private CreatedAudit of EntityAudit

module CreatedAudit =
    let create (ctx: AuditContext) : CreatedAudit = EntityAudit.create ctx |> CreatedAudit

    let hydrate (timestamp: System.DateTime, userName: string) : CreatedAudit =
        EntityAudit.hydrate (timestamp, userName) |> CreatedAudit

    let value (CreatedAudit audit) : EntityAudit = audit

type UpdatedAudit = private UpdatedAudit of EntityAudit option

module UpdatedAudit =
    let create (ctx: AuditContext) : UpdatedAudit =
        EntityAudit.create ctx |> Some |> UpdatedAudit

    let hydrate (timestamp: System.DateTime option, userName: string option) : UpdatedAudit =
        match timestamp, userName with
        | Some ts, Some user -> EntityAudit.hydrate (ts, user) |> Some |> UpdatedAudit
        | _ -> UpdatedAudit None

    let none = UpdatedAudit None

    let value (UpdatedAudit auditOpt) : EntityAudit option = auditOpt

type DeletedAudit = private DeletedAudit of EntityAudit option

module DeletedAudit =
    let create (ctx: AuditContext) : DeletedAudit =
        EntityAudit.create ctx |> Some |> DeletedAudit

    let hydrate (timestamp: System.DateTime option, userName: string option) : DeletedAudit =
        match timestamp, userName with
        | Some ts, Some user -> EntityAudit.hydrate (ts, user) |> Some |> DeletedAudit
        | _ -> DeletedAudit None

    let none = DeletedAudit None

    let value (DeletedAudit auditOpt) : EntityAudit option = auditOpt
