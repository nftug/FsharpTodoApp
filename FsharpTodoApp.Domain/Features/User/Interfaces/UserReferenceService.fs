namespace FsharpTodoApp.Domain.Features.User.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects
open System.Threading.Tasks

type UserReferenceService =
    { GetUserRefByUserName: string -> Task<UserInfo option> }
