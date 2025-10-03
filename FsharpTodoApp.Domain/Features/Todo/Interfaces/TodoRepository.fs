namespace FsharpTodoApp.Domain.Features.Todo.Interfaces

open FsharpTodoApp.Domain.Common.ValueObjects
open FsharpTodoApp.Domain.Features.Todo.Entities
open System.Threading.Tasks

type TodoRepository =
    { GetTodoById: Actor option -> System.Guid -> Task<TodoEntity option>
      SaveTodo: Actor -> TodoEntity -> Task<TodoEntity> }
