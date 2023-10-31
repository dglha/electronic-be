using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Interfaces.Persistences;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewController : ControllerBase
    {
        private readonly INewService _newService;

        public NewController(INewService newService)
        {
            _newService = newService;
        }

        /// <summary>
        /// Add new category for New
        /// </summary>
        /// <returns></returns>
        [HttpPost("add-new")]
        public async Task<ActionResult<NewCategoryDto>> CreateNewCategory(CreateNewCategoryDto request)
            => Ok(await _newService.AddNewCategory(request));
    }
}
