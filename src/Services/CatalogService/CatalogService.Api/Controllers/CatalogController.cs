using CatalogService.Api.Core.Application;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _catalogSettings;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> catalogSettings)
        {
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _catalogSettings = catalogSettings.Value;
            _catalogContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int PageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsAsync(ids);
                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-seperated list of numbers");
                }
                return Ok(items);
            }
            var totalItems = await _catalogContext.Set<CatalogItem>().LongCountAsync();

            var itemsOnPage = await _catalogContext.Set<CatalogItem>()
                .OrderBy(c => c.Name)
                .Skip(pageSize * PageIndex)
                .Take(pageSize)
                .ToListAsync();
            itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(PageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [NonAction]
        private async Task<List<CatalogItem>> GetItemsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<CatalogItem>();
            }

            var idToSelect = numIds.Select(id => id.Value);

            var items = await this._catalogContext.Set<CatalogItem>()
                .Where(ci => idToSelect.Contains(ci.Id)).ToListAsync();
            items = ChangeUriPlaceHolder(items);
            return items;

        }
        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var item = await this._catalogContext.Set<CatalogItem>().SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                var baseUri = _catalogSettings.PicBaseUrl;

                item.PictureUri = baseUri + item.PictureFileName;
                return Ok(item);
            }
            return NotFound();
        }

        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetItemsWithName(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalogContext.Set<CatalogItem>()
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.Set<CatalogItem>()
                .Where(ci => ci.Name.StartsWith(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));

        }
        [HttpGet]
        [Route("items/type/{catalogTypeId:int}/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ItemsByTypeIdAndBrandIdAsync(int catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var query = _catalogContext.Set<CatalogItem>().AsQueryable();

            query = query.Where(ci => ci.CatalogTypeId == catalogTypeId);
            if (catalogBrandId.HasValue)
                query = query.Where(ci => ci.CatalogBrandId == catalogBrandId);

            var itemCount = await query.LongCountAsync();

            var itemsOnPage = await query
                .Skip(pageSize * pageIndex)
                .Take(pageIndex)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);
            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, itemCount, itemsOnPage));
        }


        [HttpGet]
        [Route("items/type/all/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            IQueryable<CatalogItem> query = this._catalogContext.Set<CatalogItem>().AsQueryable();

            if (catalogBrandId.HasValue)
                query = query.Where(ci => ci.CatalogBrandId == catalogBrandId);

            var totalCount = await query.LongCountAsync();

            var itemsOnPage = await query
                .Skip(pageSize * pageIndex)
                .Take(pageIndex)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalCount, itemsOnPage));
        }

        [HttpGet]
        [Route("catalogTypes")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogType>>> CatalogTypesAsync()
        {
            return await this._catalogContext.Set<CatalogType>().ToListAsync();

        }
        [HttpGet]
        [Route("catalogbrands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogBrand>>> CatalogBrandsAsync()
        {
            return await this._catalogContext.Set<CatalogBrand>().ToListAsync();

        }

        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateProductAsync([FromBody]CatalogItem productToUpdate)
        {
            var catalogItem = await this._catalogContext.Set<CatalogItem>().SingleOrDefaultAsync(ci => ci.Id == productToUpdate.Id);
            if (catalogItem == null)
                return NotFound(new {Message = $"Item with id {productToUpdate.Id} is not found!"});

            var oldPrice = catalogItem.Price;
            var raiseProducrProiceChangeEvent = oldPrice != productToUpdate.Price;
            catalogItem = productToUpdate;
            this._catalogContext.Set<CatalogItem>().Update(catalogItem);
            await this._catalogContext.SaveChangesAsync();
            
            return CreatedAtAction(nameof(ItemByIdAsync), new { id = productToUpdate.Id }, null);

        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateProductAsync([FromBody]CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                PictureFileName = product.PictureFileName,
                Price = product.Price
            };
            this._catalogContext.Set<CatalogItem>().AddAsync(item);
            await this._catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = product.Id }, null);
        }

        [HttpDelete]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var item = await this._catalogContext.Set<CatalogItem>().SingleOrDefaultAsync(ci => ci.Id == id);

            if(item == null)
                return NotFound();
            this._catalogContext.Set<CatalogItem>().Remove(item);
            await this._catalogContext.SaveChangesAsync();
            return NoContent();

        }

        [NonAction]
        private List<CatalogItem> ChangeUriPlaceHolder(List<CatalogItem> items)
        {
            var baseUri = _catalogSettings.PicBaseUrl;

            items.ForEach(item =>
            {
                if (item != null)
                    item.PictureUri = baseUri + item.PictureFileName;
            });

            return items;
        }
    }

}
