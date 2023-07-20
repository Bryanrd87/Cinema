using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProtoDefinitions;
using static ProtoDefinitions.MoviesApi;

namespace Application.Services
{
    public class ApiClientGrpc
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;
        private readonly string apiKey;
        private readonly string connString;
        private const string redisGetAllKey = "allmovies";        
        private readonly MoviesApiClient _moviesApiClient;

        public ApiClientGrpc(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration;
            apiKey = _configuration.GetSection("MovieApi:ApiKey").Value;
            connString = _configuration.GetSection("MovieApi:ConnectionString").Value;

            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var headers = new Metadata { { "X-Apikey", apiKey } };

            var channel =
                GrpcChannel.ForAddress(connString, new GrpcChannelOptions()
                {
                    HttpHandler = httpHandler,
                    Credentials = ChannelCredentials.Create(new SslCredentials(), CallCredentials.FromInterceptor((context, metadata) =>
                    {
                        foreach (var entry in headers)
                        {
                            metadata.Add(entry.Key, entry.Value);
                        }
                        return Task.CompletedTask;
                    }))
                });

            _moviesApiClient = new MoviesApiClient(channel);           
        }

        public async Task<showListResponse> GetAll(CancellationToken cancellationToken)
        {
            showListResponse data = new showListResponse();
            try
            {


                var all = await _moviesApiClient.GetAllAsync(new Empty());
                all.Data.TryUnpack<showListResponse>(out var result);

                await _distributedCache.SetStringAsync(
                       redisGetAllKey,
                       JsonConvert.SerializeObject(result),
                       cancellationToken);

                return result;

            }
            catch (Exception)
            {
                var cachedMember = await _distributedCache.GetStringAsync(redisGetAllKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedMember))
                    data = JsonConvert.DeserializeObject<showListResponse>(cachedMember);

            }

            return data;
        }

        public async Task<showResponse> GetById(string id, CancellationToken cancellationToken)
        {
            showResponse data = new showResponse();           
            try
            {
                var request = new IdRequest
                {
                    Id = id
                };

                var all = await _moviesApiClient.GetByIdAsync(request);
                all.Data.TryUnpack<showResponse>(out var result);

                await _distributedCache.SetStringAsync(
                       "getmoviebyid-" + id,
                       JsonConvert.SerializeObject(result),
                       cancellationToken);

                return result;

            }
            catch (Exception)
            {
                var cachedMember = await _distributedCache.GetStringAsync("getmoviebyid-" + id, cancellationToken);
                if (!string.IsNullOrEmpty(cachedMember))
                    data = JsonConvert.DeserializeObject<showResponse>(cachedMember);

            }

            return data;
        }
    }
}