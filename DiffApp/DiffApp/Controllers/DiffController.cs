using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace DiffApp.Controllers
{
    [Route("v1/diff")]
    public class DiffController : ControllerBase
    {
        private IEnumerable<EndpointDataSource> _endpointSources;
        
        public DiffController(IEnumerable<EndpointDataSource> endpointSources)
        {
            _endpointSources = endpointSources;
        }

        [HttpGet]
        public JsonResult Get()
        {
            // returns all implemented endpoints
            var endpoints = _endpointSources
            .SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>();
            var output = endpoints.Select(
                e =>
                {
                    var controller = e.Metadata
                        .OfType<ControllerActionDescriptor>()
                        .FirstOrDefault();

                    return new
                    {
                        Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                        Route = $"/{e.RoutePattern.RawText.TrimStart('/')}"
                    };
                }
            );

            return new JsonResult(value: output);
        }

        [HttpGet("{ID}")]
        public IActionResult GetDiff(int ID)
        {            
            if (GlobalVars.all_entries.ContainsKey(key: ID))
            {
                // instance with this ID allready exists - get difference
                var entry = GlobalVars.all_entries[ID];
                ResultModel result = entry.Compare();
                if(result.diffResultType == ResultDescription.InvalidInputValue)
                {
                    return NotFound();
                }
                else
                {
                    //return JSON data of result
                    return Ok(result);
                }
                
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{ID}/right")]
        public IActionResult PutRight(int ID, [FromBody]DiffModel input)
        {
            if(input == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(input.data))
            {
                return BadRequest();
            }

            try
            {
                if (GlobalVars.all_entries.ContainsKey(key: ID))
                {
                    // instance with this ID allready exists - update property right
                    var existing_entry = GlobalVars.all_entries[ID];
                    existing_entry.Right = input.data;
                }
                else
                {
                    // instance with this ID doesn't exist - insert new
                    DiffEntry new_entry = new DiffEntry { Id = ID, Right = input.data };
                    GlobalVars.all_entries[ID] = new_entry;
                }

                return Created("", input);
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("{ID}/left")]
        public IActionResult PutLeft(int ID, [FromBody]DiffModel input)
        {
            if (input == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(input.data))
            {
                return BadRequest();
            }

            try
            {
                if (GlobalVars.all_entries.ContainsKey(key: ID))
                {
                    // instance with this ID allready exists - update property left
                    var existing_entry = GlobalVars.all_entries[ID];
                    existing_entry.Left = input.data;
                }
                else
                {
                    // instance with this ID doesn't exist - insert new
                    DiffEntry new_entry = new DiffEntry { Id = ID, Left = input.data };
                    GlobalVars.all_entries[ID] = new_entry;
                }

                return Created("", input);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}
