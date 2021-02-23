using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  

namespace API.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase 
    {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get (
            [FromServices] DataContext context
        )
        {
            var products = await  context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
                return Ok(products); 
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById (
            int id,
            [FromServices] DataContext context)
        {
            var product = await  context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return Ok(product); 
        }
        [HttpGet]
        [Route("categories/{id:int}")]
        public async Task<ActionResult<Product>> GetByCategory (
            int id,
            [FromServices] DataContext context)
        {
            var product = await  context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
                return Ok(product); 
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model
        )
        {
            if(ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}