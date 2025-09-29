namespace FsharpTodoApp.Application.Features.Todo.Dtos.Commands

open FsharpTodoApp.Application.Features.Todo.Enums

type TodoCreateCommandDto =
    { Title: string
      Description: string option
      DueDate: System.DateTime option
      AssigneeUserName: string option
      ReviewerUserName: string option }

type TodoUpdateCommandDto =
    { Title: string
      Description: string option
      DueDate: System.DateTime option
      AssigneeUserName: string option
      ReviewerUserName: string option }

type TodoUpdateStatusCommandDto = { Status: TodoStatusEnum }
