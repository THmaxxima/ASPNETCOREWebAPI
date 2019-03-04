using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWebApi.Helpers
{
    public class QueueData 
{
    public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key,
        string navigationProperty, [FromBody] Uri link)
    {
        var product = db.Products.SingleOrDefault(p => p.Id == key);
        if (product == null)
        {
            return NotFound();
        }

        switch (navigationProperty)
        {
            case "Supplier":
                product.Supplier = null;
                break;

            default:
                return StatusCodeResult(HttpStatusCode.NotImplemented);
        }
        await db.SaveChangesAsync();

        return StatusCode(HttpStatusCode.NoContent);
    }

    // Other controller methods not shown.
}

}




