using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        //this methods responds to a HTTP GET request
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
                                .Select(x => ItemToDTO(x))
                                .ToListAsync();
        }

        // GET: api/TodoItems/5
        // {id} will be provided in the url
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        // PUT: api/TodoItems/5
        //replaces the whole resource
        [HttpPut("{id}")]
        //update the TodoItem with id, meaning swap it with todoItem
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            //the entity is being tracked by the context and exists in the database, and some or all of its property values have been modified
            //_context.Entry(todoItem).State = EntityState.Modified;

            //with DTO
            var todoItem = _context.TodoItems.Find(id);
            if(todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;
            //

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        // POST: api/TodoItems
        [HttpPost]
        //gets the value of the todo item from the body of the HTTP request
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

        
            //CreatedAtAction( ) produces a standard HTTP POST method response (an HTTP 201 status code ) which means a new resource was created on the server
        //a new URL is generated for that item
        //this method adds a Location header to the response, stating WHERE that new resource was created
        //Parameters:
            //   1:
            //     The name of the action to use for generating the URL.
            //
            //   2:
            //     The route data to use for generating the URL.
            //
            //   3:
            //     The content value to format in the entity body (http response body (html code)).
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
        }


        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) => new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

    }
}
