using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IGenericService<Product, ProductDto> _productService;

        public ProductController(IGenericService<Product, ProductDto> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return ActionResultInstance(await _productService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return ActionResultInstance(await _productService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto productDto)
        {
            return ActionResultInstance(await _productService.AddAsync(productDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductDto productDto)
        {
            return ActionResultInstance(await _productService.Update(productDto, productDto.Id));
        }

        [HttpDelete("/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return ActionResultInstance(await _productService.Remove(id));
        }
    }
}