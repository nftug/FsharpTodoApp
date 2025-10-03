namespace FsharpTodoApp.Infrastructure.Features.User.DataModels

open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Features.User.Entities
open FsharpTodoApp.Persistence.DataModels
open FsharpTodoApp.Domain.Common.ValueObjects

module UserDataModel =
    let dehydrate (dataModel: UserDataModel) (entity: UserEntity) =
        entity.Base |> DataModelBase.dehydrate dataModel

        dataModel.UserName <- entity.UserName
        dataModel.FullName <- entity.FullName |> Option.defaultValue null
        dataModel.Roles <- entity.Roles |> List.toArray |> Array.map ActorRole.toEnum
