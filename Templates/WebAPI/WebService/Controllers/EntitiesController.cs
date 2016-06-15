using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using WebApi.LinkHeader;
using XProjectNamespaceX.BusinessLogic;
using XProjectNamespaceX.Model;

namespace XProjectNamespaceX.WebService.Controllers
{
    [RoutePrefix("entities")]
    [Authorize(Roles = "MyRole")]
    public class EntitiesController : ApiController
    {
        private readonly IMyService _myService;

        public EntitiesController(IMyService myService)
        {
            _myService = myService;
        }

        /// <summary>
        /// Returns all <see cref="MyEntity"/>s.
        /// </summary>
        [HttpGet, Route("")]
        public virtual IEnumerable<MyEntity> ReadAll()
        {
            // TODO: Read
            return new[] {new MyEntity {Id = "1"}};
        }

        /// <summary>
        /// Returns a specific <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> to look for.</param>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public virtual async Task<MyEntity> Read(long id)
        {
            // TODO: Read
            return await Task.FromResult(new MyEntity {Id = "1"});
        }

        /// <summary>
        /// Creates a new <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="entity">The new <see cref="MyEntity"/>.</param>
        [HttpPost, Route("")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Missing or invalid request body.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public virtual async Task<IHttpActionResult> Create(MyEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");

            // TODO: Create
            await Task.Yield();

            return Created(
                location: new Uri(Request.RequestUri.EnsureTrailingSlash(), entity.Id),
                content: entity);
        }

        /// <summary>
        /// Updates an existing <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> of the entity to update.</param>
        /// <param name="entity">The modified <see cref="MyEntity"/>.</param>
        [HttpPut, Route("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Missing or invalid request body.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public virtual async Task<IHttpActionResult> Update(long id, MyEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");

            // TODO: Update
            await Task.Yield();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes an existing <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> of the entity to delete.</param>
        [HttpDelete, Route("{id}")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public virtual async Task Delete(long id)
        {
            // TODO: Delete
            await Task.Yield();
        }
    }
}