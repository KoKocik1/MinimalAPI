using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.ToDo
{
	public static class ToDoRequest
	{

        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapGet("todos", ToDoRequest.GetAll)
                .Produces<List<ToDo>>()
                .WithTags("To Dos")
                .RequireAuthorization();

            app.MapGet("todos/{id}", ToDoRequest.GetById)
                .Produces<ToDo>()
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("To Dos");

            app.MapPost("todos", ToDoRequest.Create)
                .Produces<ToDo>(StatusCodes.Status201Created)
                .Accepts<ToDo>("application/json")
                .WithTags("To Dos")
                .WithValidator<ToDo>();

            app.MapPut("todos/{id}", ToDoRequest.Update)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent)
                .Accepts<ToDo>("application/json")
                .WithTags("To Dos")
                .WithValidator<ToDo>();

            app.MapDelete("todos", ToDoRequest.Delete)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent)
                .WithTags("To Dos")
                .ExcludeFromDescription();

            return app;
        }
		public static IResult GetAll(IToDoService service)
		{
			var todos = service.GetAll();
			return Results.Ok(todos);
		}
        public static IResult GetById(IToDoService service, Guid id)
        {
            var todo = service.GetById(id);
            if (todo is null)
                return Results.NotFound();
            return Results.Ok(todo);
        }
        [Authorize]
        public static IResult Create(IToDoService service, ToDo toDo)
        {
            service.Create(toDo);
            return Results.Created($"/todos/{toDo.Id}", toDo);
        }
        public static IResult Update(IToDoService service, Guid id, ToDo toDo)
        {
            var existedToDo = service.GetById(id);
            if (existedToDo is null)
                return Results.NotFound();


            service.Update(toDo);
            return Results.NoContent();
        }
        public static IResult Delete(IToDoService service, Guid id)
        {
            var existedToDo = service.GetById(id);
            if (existedToDo is null)
                return Results.NotFound();
            service.Delete(id);
            return Results.NoContent();
        }

    }
}

