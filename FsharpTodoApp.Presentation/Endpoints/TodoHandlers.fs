namespace FsharpTodoApp.Presentation.Endpoints

open System
open Giraffe
open FsharpTodoApp.Application.Features.Todo.Commands
open FsharpTodoApp.Application.Features.Todo.Dtos
open FsharpTodoApp.Application.Features.Todo.Queries
open FsharpTodoApp.Presentation.Utils

module TodoHandlers =
    let private queryTodosHandler: HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let useCase = HttpContextUtils.getService<QueryTodos> ctx
                    let queryDto = TodoQueryDto.bindQueryParams ctx
                    let! result = useCase.Handle(Some actor, queryDto)
                    return! json result next ctx
            }

    let private getTodoDetailsHandler (todoId: Guid) : HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let useCase = HttpContextUtils.getService<GetTodoDetails> ctx
                    let! result = useCase.Handle(Some actor, todoId)
                    return! HttpContextUtils.handleResult result json next ctx
            }

    let private createTodoHandler: HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let! command = ctx.BindJsonAsync<TodoCreateCommandDto>()
                    let useCase = HttpContextUtils.getService<CreateTodo> ctx
                    let! result = useCase.Handle(actor, command)
                    return! HttpContextUtils.handleCreateResult result next ctx
            }

    let private updateTodoHandler (todoId: Guid) : HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let! command = ctx.BindJsonAsync<TodoUpdateCommandDto>()
                    let useCase = HttpContextUtils.getService<UpdateTodo> ctx
                    let! result = useCase.Handle(actor, todoId, command)
                    return! HttpContextUtils.handleCommandResult result next ctx
            }

    let private updateTodoStatusHandler (todoId: Guid) : HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let! command = ctx.BindJsonAsync<TodoUpdateStatusCommandDto>()
                    let useCase = HttpContextUtils.getService<UpdateTodoStatus> ctx
                    let! result = useCase.Handle(actor, todoId, command)
                    return! HttpContextUtils.handleCommandResult result next ctx
            }

    let private deleteTodoHandler (todoId: Guid) : HttpHandler =
        fun next ctx ->
            task {
                match! HttpContextUtils.requireActor ctx with
                | Error error -> return! HttpContextUtils.respondAppError error next ctx
                | Ok actor ->
                    let useCase = HttpContextUtils.getService<DeleteTodo> ctx
                    let! result = useCase.Handle(actor, todoId)
                    return! HttpContextUtils.handleCommandResult result next ctx
            }

    let routes: HttpHandler =
        choose
            [ subRoute
                  "/api/todos"
                  (choose
                      [ routef "/%O/status" (fun (todoId: Guid) -> PATCH >=> updateTodoStatusHandler todoId)
                        routef "/%O" (fun (todoId: Guid) ->
                            choose
                                [ GET >=> getTodoDetailsHandler todoId
                                  PUT >=> updateTodoHandler todoId
                                  DELETE >=> deleteTodoHandler todoId ])
                        subRoute "/" (choose [ GET >=> queryTodosHandler; POST >=> createTodoHandler ]) ])
              fun next ctx -> next ctx ]
