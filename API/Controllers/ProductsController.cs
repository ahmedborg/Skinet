using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Helpers;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;

        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, 
                                  IProductRepository repo,
                                  IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _productRepo = productRepo;           
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>>  GetProducts(
            [FromQuery]ProductSpecParams productParams)
        { 
            var spec = new ProductWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFilterForCountSpecification(productParams);
            var totalItems = await _productRepo.CountAsync(countSpec);
           // var products = await _repo.GetProductsAsync();
           var products = await _productRepo.ListAsync(spec);

           var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

           return Ok(new Pagination<ProductToReturnDto>(productParams.PageIdex,
                        productParams.PageSize, totalItems, data));

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
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithTypesAndBrandsSpecification(id);

            var product = await _productRepo.GetEntityWithSpec(spec);

            return _mapper.Map<Product, ProductToReturnDto>(product);

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
            //return await _repo.GetProductByIdAsync(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _repo.GetProductBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _repo.GetProductTypesAsync());
        }
    }
}