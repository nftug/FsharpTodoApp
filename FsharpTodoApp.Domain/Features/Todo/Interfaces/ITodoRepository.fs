namespace FsharpTodoApp.Domain.Features.Todo.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities

type ITodoRepository =
    abstract member GetById: Actor option * System.Guid -> Async<TodoEntity option>
    abstract member Save: Actor * TodoEntity -> Async<TodoEntity>
