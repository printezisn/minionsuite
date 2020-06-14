using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinionSuiteExample.Web.Models;
using MinionSuiteExample.Web.Services;
using Newtonsoft.Json;
using Xunit;

namespace MinionSuiteExample.Tests.Controllers
{
    public class CommentsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ICommentService _service;
        private readonly IPostService _postService;

        public CommentsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _scope = _factory.Services.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<ICommentService>();
            _postService = _scope.ServiceProvider.GetRequiredService<IPostService>();
        }

        [Fact]
        public async Task Index_Get_Successful()
        {
            var entity = await BuildEntity(true);

            var response = await _client.GetAsync($"/Comments?asc=false&postId={entity.PostId}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"Comments/Details/{entity.Id}", content);
        }

        [Fact]
        public async Task Index_Get_SearchSuccessful()
        {
            var firstEntity = await BuildEntity(true);
            var secondEntity = await BuildEntity(true, firstEntity.PostId);

            var response = await _client.GetAsync($"/Comments?postId={firstEntity.PostId}&term={HttpUtility.UrlEncode(firstEntity.Body)}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"Comments/Details/{firstEntity.Id}", content);
            Assert.DoesNotContain($"Comments/Details/{secondEntity.Id}", content);
        }

        [Fact]
        public async Task Details_Get_NotFound()
        {
            var response = await _client.GetAsync("/Comments/Details/0");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Details_Get_Successful()
        {
            var entity = await BuildEntity(true);
            var response = await _client.GetAsync($"/Comments/Details/{entity.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_Successful()
        {
            var response = await _client.GetAsync("/Comments/Create");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_Post_Successful()
        {
            var verificationToken = await GetVerificationTokenFromPage("/Comments/Create");

            var entity = await BuildEntity(false);
            var dict = EntityToDictionary(entity);
            dict["__RequestVerificationToken"] = verificationToken;
            var formContent = new FormUrlEncodedContent(dict);

            var response = await _client.PostAsync("/Comments/Create", formContent);
            var createdEntity = await FetchEntityFromDetailsUrl(response.Headers.Location.ToString());

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotNull(createdEntity);
            Assert.Equal(entity.Body, createdEntity.Body);
            Assert.Equal(entity.PostId, createdEntity.PostId);
        }

        [Fact]
        public async Task Edit_Get_NotFound()
        {
            var response = await _client.GetAsync("/Comments/Edit/0");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Get_Successful()
        {
            var entity = await BuildEntity(true);
            var response = await _client.GetAsync($"/Comments/Edit/{entity.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_Successful()
        {
            var entity = await BuildEntity(true);
            var verificationToken = await GetVerificationTokenFromPage($"/Comments/Edit/{entity.Id}");

            var dict = EntityToDictionary(await BuildEntity(false));
            dict["Id"] = entity.Id.ToString();
            dict["__RequestVerificationToken"] = verificationToken;
            var formContent = new FormUrlEncodedContent(dict);

            var response = await _client.PostAsync("/Comments/Edit", formContent);
            var updatedEntity = await _service.GetAsync(entity.Id);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotNull(updatedEntity);
            Assert.Equal(entity.Body, updatedEntity.Body);
            Assert.Equal(entity.PostId, updatedEntity.PostId);
        }

        [Fact]
        public async Task Delete_Get_NotFound()
        {
            var response = await _client.GetAsync("/Comments/Delete/0");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Get_Successful()
        {
            var entity = await BuildEntity(true);
            var response = await _client.GetAsync($"/Comments/Delete/{entity.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Post_Successful()
        {
            var entity = await BuildEntity(true);
            var verificationToken = await GetVerificationTokenFromPage($"/Comments/Delete/{entity.Id}");

            var dict = new Dictionary<string, string>
            {
                ["Id"] = entity.Id.ToString(),
                ["__RequestVerificationToken"] = verificationToken
            };
            var formContent = new FormUrlEncodedContent(dict);

            var response = await _client.PostAsync("/Comments/Delete", formContent);
            entity = await _service.GetAsync(entity.Id);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Null(entity);
        }

        private async Task<Comment> BuildEntity(bool save, int? postId = null)
        {
            if (!postId.HasValue)
            {
                var post = new Post()
                {
                    Title = Guid.NewGuid().ToString(),
                    Body = Guid.NewGuid().ToString(),
                };
                post = (await _postService.CreateAsync(post)).Result;
                postId = post.Id;
            }

            var entity = new Comment()
            {
                Body = Guid.NewGuid().ToString(),
                PostId = postId.Value,
            };

            if (save)
            {
                var result = await _service.CreateAsync(entity);
                return result.Result;
            }

            return entity;
        }

        private Dictionary<string, string> EntityToDictionary(Comment entity)
        {
            var jsonContent = JsonConvert.SerializeObject(entity);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
        }

        private async Task<string> GetVerificationTokenFromPage(string pageUrl)
        {
            var response = await _client.GetAsync(pageUrl);
            var content = await response.Content.ReadAsStringAsync();
            var verificationTokenMatch = Regex.Match(content, "name=\"__RequestVerificationToken\"(.*?)value=\"(.+?)\"");

            Assert.True(verificationTokenMatch.Success);

            return verificationTokenMatch.Groups[2].Value;
        }

        private async Task<Comment> FetchEntityFromDetailsUrl(string url)
        {
            var match = Regex.Match(url, "/Comments/Details/(.+)");
            if (!match.Success)
            {
                return null;
            }

            var key = int.Parse(match.Groups[1].Value);
            return await _service.GetAsync(key);
        }
    }
}
