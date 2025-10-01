namespace FsharpTodoApp.Application.Common.Dtos

type ItemCreatedResponseDto = { ItemId: System.Guid }

module ItemCreatedResponseDto =
    open FsharpTodoApp.Domain.Common.Entities

    let create entityBase = { ItemId = entityBase.IdSet.PublicId }
