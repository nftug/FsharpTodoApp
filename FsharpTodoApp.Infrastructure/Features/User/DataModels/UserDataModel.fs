namespace FsharpTodoApp.Infrastructure.Features.User.DataModels

open System
open Microsoft.EntityFrameworkCore.Storage.ValueConversion
open FsharpTodoApp.Infrastructure.Persistence.DataModels
open FsharpTodoApp.Domain.Common.Enums
open FsharpTodoApp.Domain.Features.User.Entities

[<Sealed>]
type UserDataModel() =
    inherit DataModelBase()

    member val UserName = String.Empty with get, set
    member val FullName: string option = None with get, set
    member val Roles: ActorRoleEnum list = [] with get, set

module UserDataModel =
    let onModelCreating modelBuilder =
        DataModelBase.onModelCreating<UserDataModel> "Users" modelBuilder

        modelBuilder
            .Entity<UserDataModel>()
            .HasIndex(fun x -> x.UserName :> obj)
            .IsUnique()
        |> ignore

        modelBuilder
            .Entity<UserDataModel>()
            .Property(fun x -> x.Roles)
            .HasConversion(
                new ValueConverter<ActorRoleEnum list, string>(
                    (fun v -> v |> List.map string |> String.concat ","),
                    (fun v ->
                        v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        |> Array.map Enum.Parse<ActorRoleEnum>
                        |> Array.toList)
                )
            )
        |> ignore

    let dehydrate (dataModel: UserDataModel) (entity: UserEntity) =
        entity.Base |> DataModelBase.dehydrate dataModel

        dataModel.UserName <- entity.UserName
        dataModel.FullName <- entity.FullName
        dataModel.Roles <- entity.Roles |> List.map ActorRoleEnum.fromDomain
