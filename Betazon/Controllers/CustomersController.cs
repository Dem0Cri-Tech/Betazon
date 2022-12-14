using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betazon.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Hashing;

namespace Betazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;
        private Encrypt _encrypt;

        public CustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
             using (var context = _context)
             {
                 var customers = await context.Customers
                     .AsNoTracking()
                     .Include(customer => customer.CustomerAddresses)// DA CUSTOMER A CUSTOMERADDRESS
                     .ThenInclude(customerAddress => customerAddress.Address)// ALL'interno di CUSTOMER ADDRESS otteniamo gli ADDRESS
                                                                             //.Include(customer =>customer.SalesOrderHeaders)
                                                                             //.ThenInclude(salesOrderHeaders=> salesOrderHeaders.SalesOrderDetails)
                                                                             //.ThenInclude(salesOrderDetails => salesOrderDetails.Product)
                     .ToListAsync();

                 return customers;
             }

        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            using (var context = _context)
            {
                var customer = await context.Customers
                   .AsNoTracking()
                   .Where(customer => customer.CustomerId == id)
                   .Include(customer => customer.CustomerAddresses)// DA CUSTOMER A CUSTOMERADDRESS
                   .ThenInclude(customerAddress => customerAddress.Address)// ALL'interno di CUSTOMER ADDRESS otteniamo gli ADDRESS
                   .Include(customer => customer.SalesOrderHeaders)
                   .ThenInclude(salesOrderHeaders => salesOrderHeaders.SalesOrderDetails)
                   .ThenInclude(salesOrderDetails => salesOrderDetails.Product)
                   .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
           
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (_context.Customers == null)
                {
                    return Problem("Entity set 'AdventureWorksLt2019Context.Customers'  is null.");
                }
                customer.PasswordSalt = _encrypt.SaltGenerator();
                customer.PasswordHash = _encrypt.Hash(customer.PasswordHash, customer.PasswordSalt);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
            }
            catch (Exception e) 
            {
                
                _context.ErrorLogs.FromSql($"uspLogError {0}");
                transaction.Rollback();
                return Problem("Error During insert on Customer table");
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
