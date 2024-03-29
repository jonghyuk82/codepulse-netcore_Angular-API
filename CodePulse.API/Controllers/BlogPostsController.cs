﻿using Microsoft.AspNetCore.Mvc;
using CodePulse.API.Models.DTO;
using Microsoft.AspNetCore.Http;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
		private readonly ICategoryRepository categoryRepository;

		public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
			this.categoryRepository = categoryRepository;
		}

        // POST: /api/blogposts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            // Convert DTO to domain
            var blogPost = new BlogPost
            {
                Author = request.Author,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                Title = request.Title,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach(var categoryGuid in request.Categories) 
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if(existingCategory != null) 
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            // Convert Domain Model to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(response);

        }

        // GET: /api/blogposts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();

            // Conver Domain model to DTO
            var response = new List<BlogPostDto>();

            foreach(var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto 
                {
					Id = blogPost.Id,
					Author = blogPost.Author,
					Content = blogPost.Content,
					FeaturedImageUrl = blogPost.FeaturedImageUrl,
					IsVisible = blogPost.IsVisible,
					PublishedDate = blogPost.PublishedDate,
					ShortDescription = blogPost.ShortDescription,
					Title = blogPost.Title,
					UrlHandle = blogPost.UrlHandle,
					Categories = blogPost.Categories.Select(x => new CategoryDto
					{
						Id = x.Id,
						Name = x.Name,
						UrlHandle = x.UrlHandle,
					}).ToList()
				});
            }

            return Ok(response);
        }
    }
}
