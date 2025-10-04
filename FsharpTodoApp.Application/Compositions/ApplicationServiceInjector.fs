namespace FsharpTodoApp.Application.Compositions

module ApplicationServiceInjector =
    open Microsoft.Extensions.DependencyInjection
    open FsharpTodoApp.Domain.Features.Todo.Interfaces
    open FsharpTodoApp.Domain.Features.Todo.Services
    open FsharpTodoApp.Domain.Features.User.Interfaces
    open FsharpTodoApp.Application.Features.Todo.Commands
    open FsharpTodoApp.Application.Features.User.Commands
    open FsharpTodoApp.Domain.Features.User.Services
    open FsharpTodoApp.Application.Features.Todo.Queries
    open FsharpTodoApp.Application.Features.Todo.Interfaces

    let inject (services: IServiceCollection) =
        services
            .AddScoped<GetOrCreateUser>(fun sp ->
                let repo = sp.GetRequiredService<UserRepository>()
                let policyService = sp.GetRequiredService<UserPolicyService>()
                GetOrCreateUser.create repo policyService)
            .AddScoped<CreateTodo>(fun sp ->
                let repo = sp.GetRequiredService<TodoRepository>()
                let userRef = sp.GetRequiredService<UserReferenceService>()
                let policyService = sp.GetRequiredService<TodoPolicyService>()
                CreateTodo.create repo userRef policyService)
            .AddScoped<UpdateTodo>(fun sp ->
                let repo = sp.GetRequiredService<TodoRepository>()
                let userRef = sp.GetRequiredService<UserReferenceService>()
                let policyService = sp.GetRequiredService<TodoPolicyService>()
                UpdateTodo.create repo userRef policyService)
            .AddScoped<UpdateTodoStatus>(fun sp ->
                let repo = sp.GetRequiredService<TodoRepository>()
                let policyService = sp.GetRequiredService<TodoPolicyService>()
                UpdateTodoStatus.create repo policyService)
            .AddScoped<DeleteTodo>(fun sp ->
                let repo = sp.GetRequiredService<TodoRepository>()
                let policyService = sp.GetRequiredService<TodoPolicyService>()
                DeleteTodo.create repo policyService)
            .AddScoped<GetTodoDetails>(fun sp ->
                let queryService = sp.GetRequiredService<TodoQueryService>()
                GetTodoDetails.create queryService)
            .AddScoped<QueryTodos>(fun sp ->
                let queryService = sp.GetRequiredService<TodoQueryService>()
                QueryTodos.create queryService)
        |> ignore

        services
