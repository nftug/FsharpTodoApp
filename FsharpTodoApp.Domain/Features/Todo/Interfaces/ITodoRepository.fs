namespace FsharpTodoApp.Domain.Features.Todo.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities
open System.Threading.Tasks

type ITodoRepository =
    abstract member GetById: Actor option * System.Guid -> Task<TodoEntity option>
    abstract member Save: Actor * TodoEntity -> Task<TodoEntity>
