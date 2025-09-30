namespace FsharpTodoApp.Domain.Features.Todo.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities

type ITodoRepository =
    abstract member GetByIdAsync: Actor option * System.Guid -> Async<TodoEntity option>
    abstract member SaveAsync: Actor * TodoEntity -> Async<TodoEntity>
