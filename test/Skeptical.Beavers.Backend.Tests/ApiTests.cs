using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Skeptical.Beavers.Backend.Model;
using Xunit;

namespace Skeptical.Beavers.Backend.Tests
{
    public class UnitTest1 : IDisposable
    {
        private readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080")
        };

        [Fact]
        public async Task LoginReturnsAuthToken()
        {
            // prepare
            var request = new LoginRequest
            {
                UserName = "admin",
                Password = "admin"
            };
            var formData = new MultipartFormDataContent
            {
                {new StringContent(request.UserName), "userName"},
                {new StringContent(request.Password), "password"}
            };

            // act
            var response = await _client.PostAsync("/login", formData).ConfigureAwait(false);

            // assert
            response.EnsureSuccessStatusCode();
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));

            responseData.UserName.Should().Be("admin");
            responseData.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task AppReturns400WhenNotAuthorized()
        {
            // act
            var response = await _client.GetAsync("/app").ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task AppReturnsHtmlWhenAuthorized()
        {
            // prepare
            var token = await GetAuthTokenForAdminAsync();

            // act
            var message = new HttpRequestMessage(HttpMethod.Get, "/app")
            {
                Headers = { {"Authorization", $"Bearer {token}"} }
            };
            var response = await _client.SendAsync(message).ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            content.Should().Contain("html");
        }

        [Fact]
        public async Task TransactionReturnsNotFoundWhenAuthorizedAndNoAppKey()
        {
            // prepare
            var token = await GetAuthTokenForAdminAsync();
            var formData = new MultipartFormDataContent
            {
                {new StringContent("734 34 2874 21094"), "accountNumber"},
                {new StringContent("Ivan Ivanov"), "accountHolder"},
                {new StringContent("65"), "MoneySent"}
            };

            // act
            var message = new HttpRequestMessage(HttpMethod.Post, "/transaction")
            {
                Headers = { {"Authorization", $"Bearer {token}"} },
                Content = formData
            };
            var response = await _client.SendAsync(message).ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task TransactionReturn404WhenAuthorizedAndNoAppKey()
        {
            // prepare
            var token = await GetAuthTokenForAdminAsync();
            var formData = new MultipartFormDataContent
            {
                {new StringContent("734 34 2874 21094"), "accountNumber"},
                {new StringContent("Ivan Ivanov"), "accountHolder"},
                {new StringContent("65"), "moneySent"}
            };

            // act
            var message = new HttpRequestMessage(HttpMethod.Post, Routes.Transaction)
            {
                Headers = { {"Authorization", $"Bearer {token}"} },
                Content = formData
            };
            var response = await _client.SendAsync(message).ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task TransactionReturn404WhenAuthorizedAndAppKeyForDifferentUser()
        {
            // prepare
            var token = await GetAuthTokenForAdminAsync();
            var formData = new MultipartFormDataContent
            {
                {new StringContent("734 34 2874 21094"), "accountNumber"},
                {new StringContent("Ivan Ivanov"), "accountHolder"},
                {new StringContent("65"), "moneySent"}
            };

            // act
            var message = new HttpRequestMessage(HttpMethod.Post, Routes.Transaction)
            {
                Headers =
                {
                    {"Authorization", $"Bearer {token}"},
                    {"App-Auth", $"secret test app key for user1"}
                },
                Content = formData
            };
            var response = await _client.SendAsync(message).ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task TransactionWorkWhenAuthorizedAndWithAppKey()
        {
            // prepare
            var token = await GetAuthTokenForAdminAsync();
            var formData = new MultipartFormDataContent
            {
                {new StringContent("734 34 2874 21094"), "accountNumber"},
                {new StringContent("Ivan Ivanov"), "accountHolder"},
                {new StringContent("65"), "moneySent"}
            };

            // act
            var message = new HttpRequestMessage(HttpMethod.Post, Routes.Transaction)
            {
                Headers =
                {
                    {"Authorization", $"Bearer {token}"},
                    {"App-Auth", "secret test app key for admin"}
                },
                Content = formData
            };
            var response = await _client.SendAsync(message).ConfigureAwait(false);

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        private async Task<string> GetAuthTokenForAdminAsync()
        {
            var formData = new MultipartFormDataContent
            {
                {new StringContent("admin"), "userName"},
                {new StringContent("admin"), "password"}
            };
            var response = await _client.PostAsync("/login", formData).ConfigureAwait(false);
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false));
            return responseData.AccessToken;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}