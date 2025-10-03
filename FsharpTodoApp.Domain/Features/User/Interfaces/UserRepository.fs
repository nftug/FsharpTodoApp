namespace FsharpTodoApp.Domain.Features.User.Interfaces

open System.Threading.Tasks
open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.User.Entities

type UserRepository =
    { GetUserByUserName: Actor option -> string -> Task<UserEntity option>
      SaveUser: Actor -> UserEntity -> Task<UserEntity> }
