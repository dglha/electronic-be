using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Dashboard;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.DTOs.Product.User;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        
        /// <summary>
        /// Get Dashboard information
        /// </summary>
        /// <returns></returns>
        [HttpGet("feature-product")]
        public async Task<ActionResult<BaseResponse<DashboardDto>>> GetDashboardInformation()
        {
            return Ok(await _dashboardService.GetDashboarData());
        }
        
        /// <summary>
        /// Get Dashboard information
        /// </summary>
        /// <returns></returns>
        [HttpGet("latest-payment")]
        public async Task<ActionResult<Pagination<PaymentDto>>> GetLatestPayment()
        {
            return Ok(await _dashboardService.GetLatestPayment());
        }
    }
}
