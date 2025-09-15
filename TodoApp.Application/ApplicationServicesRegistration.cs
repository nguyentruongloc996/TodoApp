using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.GoogleLogin;
using TodoApp.Application.UseCases.Auth.Login;
using TodoApp.Application.UseCases.Auth.Register;
using TodoApp.Application.UseCases.Task.CompleteTask;
using TodoApp.Application.UseCases.Task.CreateTask;
using TodoApp.Application.UseCases.Task.DeleteTask;
using TodoApp.Application.UseCases.Task.UpdateTask;
using TodoApp.Application.UseCases.User.CreateUser;
using TodoApp.Application.UseCases.User.SearchUsers;

namespace TodoApp.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Auth Use Cases
            services.AddScoped<ICommandHandle<LoginCommand, LoginResponseDto>, LoginCommandHandle>();
            services.AddScoped<ICommandHandle<GoogleLoginCommand, LoginResponseDto>, GoogleLoginCommandHandle>();
            services.AddScoped<ICommandHandle<RegisterCommand, RegisterRequestDto>, RegisterCommandHandle>();

            // Task Use Cases
            services.AddScoped<ICommandHandle<CreateTaskCommand, TaskDto>, CreateTaskCommandHandle>();
            services.AddScoped<ICommandHandle<UpdateTaskCommand, TaskDto>, UpdateTaskCommandHandle>();
            services.AddScoped<ICommandHandle<CompleteTaskCommand<TaskDto>, TaskDto>, CompleteTaskCommandHandle>();
            services.AddScoped<ICommandHandle<DeleteTaskCommand, bool>, DeleteTaskCommandHandle>();

            // User Use Cases
            services.AddScoped<ICommandHandle<CreateUserCommand, UserDto>, CreateUserCommandHandle>();
            services.AddScoped<IQueryHandle<SearchUsersQuery, List<UserDto>>, SearchUsersQueryHandle>();

            return services;
        }
    }
} 