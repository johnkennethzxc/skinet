using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    public class ProductsController : BaseApiController //ControllerBase
    {
        // private readonly IProductRepository _productRepository;
        // public ProductsController(IProductRepository productRepository)
        // {
        //     _productRepository = productRepository;
        // }

        // Using Generic Repository
        private readonly IGenericRepository<Product> _productGenericRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandGenericRepo;
        private readonly IGenericRepository<ProductType> _productTypeGenericRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productGenericRepo, 
            IGenericRepository<ProductBrand> productBrandGenericRepo, 
            IGenericRepository<ProductType> productTypeGenericRepo,
            IMapper mapper)
        {
            _productTypeGenericRepo = productTypeGenericRepo;
            _productBrandGenericRepo = productBrandGenericRepo;
            _productGenericRepo = productGenericRepo;
            _mapper = mapper;
        }
        
        // [HttpGet]
        // public async Task<ActionResult<List<Product>>> GetProducts()
        // {
        //     // return Ok(await _productRepository.GetProductsAsync());

        //     // Using Generic Repository
        //     // return Ok(await _productGenericRepo.ListAllAsync());

        //     // Using Generic Repository with Specification
        //     var spec = new ProductsWithTypesAndBrandsSpecification();
        //     var products = await _productGenericRepo.ListAsync(spec);
        //     return Ok(products);

        // }

        // Using DTO
        // [HttpGet]
        // public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(
        //     /*string sort, int? brandId, int? typeId*/ [FromQuery] ProductSpecParams productParams)
        // {
        //     // return Ok(await _productRepository.GetProductsAsync());

        //     // Using Generic Repository
        //     // return Ok(await _productGenericRepo.ListAllAsync());

        //     // Using Generic Repository with Specification
        //     var spec = new ProductsWithTypesAndBrandsSpecification(/*sort, brandId, typeId*/ productParams);
        //     var products = await _productGenericRepo.ListAsync(spec);
            
        //     // return products.Select(product => new ProductToReturnDto
        //     // {
        //     //     Id = product.Id,
        //     //     Name = product.Name,
        //     //     Description = product.Description,
        //     //     PictureUrl = product.PictureUrl,
        //     //     Price = product.Price,
        //     //     ProductBrand = product.ProductBrand.Name,
        //     //     ProductType = product.ProductType.Name
        //     // }).ToList();

        //     // Using AutoMapper
        //     return Ok(_mapper
        //         .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));

        // }

        // Using Pagination and getting the details of the pagination
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            /*string sort, int? brandId, int? typeId*/ [FromQuery] ProductSpecParams productParams)
        {
            // return Ok(await _productRepository.GetProductsAsync());

            // Using Generic Repository
            // return Ok(await _productGenericRepo.ListAllAsync());

            // Using Generic Repository with Specification
            var spec = new ProductsWithTypesAndBrandsSpecification(/*sort, brandId, typeId*/ productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _productGenericRepo.CountAsync(countSpec);

            var products = await _productGenericRepo.ListAsync(spec);

            var data = _mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
            
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
            
            // return products.Select(product => new ProductToReturnDto
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     PictureUrl = product.PictureUrl,
            //     Price = product.Price,
            //     ProductBrand = product.ProductBrand.Name,
            //     ProductType = product.ProductType.Name
            // }).ToList();

            // Using AutoMapper
            // return Ok(_mapper
            //     .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));

        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<Product>> GetProduct(int id)
        // {
        //     // return await _productRepository.GetProductByIdAsync(id);

        //     // Using Generic Repository
        //     // return await _productGenericRepo.GetByIdAsync(id);

        //     // Using Generic Repository with Specification
        //     var spec = new ProductsWithTypesAndBrandsSpecification(id);

        //     // return await _productGenericRepo.GetEntityWithSpec(spec);
        // }

        // Using DTO
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Swagger configuration
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)] // Swagger Configuration
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            // return await _productRepository.GetProductByIdAsync(id);

            // Using Generic Repository
            // return await _productGenericRepo.GetByIdAsync(id);

            // Using Generic Repository with Specification
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            // return await _productGenericRepo.GetEntityWithSpec(spec);
            var product = await _productGenericRepo.GetEntityWithSpec(spec);

            if (product == null) return NotFound(new ApiResponse(404));

            // Using DTO
            // return new ProductToReturnDto
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     PictureUrl = product.PictureUrl,
            //     Price = product.Price,
            //     ProductBrand = product.ProductBrand.Name,
            //     ProductType = product.ProductType.Name
            // };

            // Using AutoMapper
            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            // return Ok(await _productRepository.GetProductBrandsAsync());

            // Using Generic Repository
            return Ok(await _productBrandGenericRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            // return Ok(await _productRepository.GetProductTypesAsync());

            // Using Generic Repository
            return Ok(await _productTypeGenericRepo.ListAllAsync());
        }
    }
}