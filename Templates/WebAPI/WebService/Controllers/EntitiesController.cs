using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using WebApi.LinkHeader;
using XProjectNamespaceX.BusinessLogic;
using XProjectNamespaceX.Model;

namespace XProjectNamespaceX.WebService.Controllers
{
    [RoutePrefix("entities")]
    [Authorize]
    public class EntitiesController : ApiController
    {
        private readonly IMyService _service;

        public EntitiesController(IMyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns all <see cref="MyEntity"/>s.
        /// </summary>
        [HttpGet, Route("")]
        public IEnumerable<MyEntity> ReadAll() => _service.GetAll();

        /// <summary>
        /// Returns a specific <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> to look for.</param>
        [HttpGet, Route("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public MyEntity Read(long id) => _service.Get(id);

        /// <summary>
        /// Creates a new <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="entity">The new <see cref="MyEntity"/>.</param>
        [HttpPost, Route("")]
        [Authorize(Roles = "MyRole")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Missing or invalid request body.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public IHttpActionResult Create(MyEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");

            _service.Add(entity);

            return Created(
                location: new Uri(Request.RequestUri.EnsureTrailingSlash(), entity.Id.ToString()),
                content: entity);
        }

        /// <summary>
        /// Updates an existing <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> of the entity to update.</param>
        /// <param name="entity">The modified <see cref="MyEntity"/>.</param>
        [HttpPut, Route("{id}")]
        [Authorize(Roles = "MyRole")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Missing or invalid request body.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public IHttpActionResult Update(long id, MyEntity entity)
        {
            if (entity == null) throw new InvalidDataException("Missing request body.");
            if (!ModelState.IsValid) throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            if (entity.Id != id) throw new InvalidDataException($"ID in URI ({id}) must match the ID in the body ({entity.Id}).");

            _service.Update(entity);

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes an existing <see cref="MyEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="MyEntity.Id"/> of the entity to delete.</param>
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "MyRole")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound, "Specified entity not found.")]
        public void Delete(long id)
        {
            _service.Remove(id);
        }
    }
}