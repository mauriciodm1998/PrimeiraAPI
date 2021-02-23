using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using API.Services;

namespace API.Controllers
{
    
    [Route("users")]
    
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles= "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
        )
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                // esconde a senha
                model.Password = "";
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { messasge = "Não foi possível criar um usuário"});
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            var user = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == model.Username && x.Password == model.Password)
            .FirstOrDefaultAsync();

            if(user == null)
                return NotFound(new { message = "usuário ou senha inválidos"});

            var token = TokenService.GenerateToken(user);

            user.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put (
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(id != model.Id)
                return NotFound(new { message = "Usuário não encontrado"});
            
            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário"});
            }
        }

    }
}