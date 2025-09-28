namespace FsharpTodoApp.Domain.Common.ValueObjects

type EntityIdSet = { DbId: int64; PublicId: System.Guid }

module EntityIdSet =
    let create () =
        { DbId = 0L
          PublicId = System.Guid.NewGuid() }
