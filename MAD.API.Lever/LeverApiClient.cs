using MAD.API.Lever.Api;
using MAD.API.Lever.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MAD.API.Lever
{
    public class LeverApiClient : IDisposable
    {
        private const string ApiBaseUri = "https://api.lever.co/v1/";

        public string LeverApiKey { get; }

        private readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private readonly HttpClient httpClient;

        public LeverApiClient(string leverApiKey)
        {
            if (string.IsNullOrEmpty(leverApiKey))
                throw new ArgumentException("The Lever API Key must be supplied.", nameof(leverApiKey));

            this.LeverApiKey = leverApiKey;

            this.httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            this.httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.Authorization), $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(this.LeverApiKey))}:");
            this.httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.Accept), "application/json");
            this.httpClient.BaseAddress = new Uri(ApiBaseUri);
        }

        private async Task<T> ExecuteRequest<T>(string relativeUri, string parameters = "", int attempt = 0)
        {
            try
            {
                using Stream responseStream = await this.httpClient.GetStreamAsync(relativeUri.ToLower() + parameters);
                using StreamReader sr = new StreamReader(responseStream);
                using JsonReader jr = new JsonTextReader(sr);

                return this.jsonSerializer.Deserialize<T>(jr);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError
                    && ex.Response is HttpWebResponse webResponse)
                {
                    // Have some tolerance for a TooManyRequests status. Give it a couple of tries before fully throwing an exception.
                    if (webResponse.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        const int tooManyRequestsErrorRestMilliseconds = 1000;
                        const int maxAttempts = 5;

                        if (attempt < maxAttempts)
                        {
                            await Task.Delay(tooManyRequestsErrorRestMilliseconds);
                            return await this.ExecuteRequest<T>(relativeUri, parameters, attempt + 1);
                        }
                    }
                }

                throw;
            }
        }

        public async Task<ApiResponse<T>> GetResponse<T>(string endpoint, string parameters = "")
        {
            var response = await this.ExecuteRequest<ApiResponse<T>>(endpoint, parameters);
            return response;
        }

        private async Task<IEnumerable<T>> GetAllForEndpoint<T>(string endpoint, int? take = null)
        {
            ApiResponse<T> apiResult;
            List<T> result = new List<T>();

            string offset = string.Empty;

            do
            {
                apiResult = await this.ExecuteRequest<ApiResponse<T>>(endpoint, offset);

                foreach (var d in apiResult.Data)
                    result.Add(d);

                string nextGuid = HttpUtility.UrlDecode(apiResult.Next);
                offset = $"?offset={nextGuid}";

                if (take.HasValue
                    && result.Count >= take.Value)
                {
                    break;
                }

            } while (apiResult.HasNext == true);

            return result;
        }

        public async IAsyncEnumerable<T> GetEndpointAsAsyncEnumerable<T>(string endpoint, string parameters = "")
        {
            ApiResponse<T> apiResult;

            var queryParams = HttpUtility.ParseQueryString(parameters);
            queryParams.Add("confidentiality", "all");

            var offset = queryParams.ToString();

            do
            {
                if (offset.StartsWith("?") == false) offset = $"?{offset}";

                apiResult = await this.ExecuteRequest<ApiResponse<T>>(endpoint, offset);

                foreach (var d in apiResult.Data) yield return d;

                var nextGuid = HttpUtility.UrlDecode(apiResult.Next);
                offset = $"?offset={nextGuid}";

            } while (apiResult.HasNext == true);
        }

        public Task<IEnumerable<Requisition>> Requisitions(int? take = null)
        {
            return this.GetAllForEndpoint<Requisition>(nameof(this.Requisitions), take);
        }

        public Task<IEnumerable<Posting>> Postings(int? take = null)
        {
            return this.GetAllForEndpoint<Posting>(nameof(this.Postings), take);
        }

        public Task<IEnumerable<Stage>> Stages(int? take = null)
        {
            return this.GetAllForEndpoint<Stage>(nameof(this.Stages), take);
        }

        public Task<IEnumerable<Opportunity>> Candidates(int? take = null)
        {
            return this.GetAllForEndpoint<Opportunity>(nameof(this.Candidates), take);
        }

        public Task<IEnumerable<Opportunity>> Opportunities(int? take = null)
        {
            return this.GetAllForEndpoint<Opportunity>(nameof(this.Opportunities), take);
        }

        public Task<IEnumerable<Offer>> OffersForOpportunity(string opportunityId, int? take = null)
        {
            return this.GetAllForEndpoint<Offer>($"opportunities/{opportunityId}/offers", take);
        }

        public Task<IEnumerable<Offer>> OffersForCandidate(string candidateId, int? take = null)
        {
            return this.GetAllForEndpoint<Offer>($"candidates/{candidateId}/offers", take);
        }

        public Task<IEnumerable<Referral>> ReferralsForCandidate(string candidateId, int? take = null)
        {
            return this.GetAllForEndpoint<Referral>($"candidates/{candidateId}/referrals", take);
        }

        public Task<IEnumerable<Referral>> ReferralsForOpportunity(string opportunityId, int? take = null)
        {
            return this.GetAllForEndpoint<Referral>($"opportunities/{opportunityId}/referrals", take);
        }

        public Task<IEnumerable<Interview>> InterviewsForOpportunity(string opportunityId, int? take = null)
        {
            return this.GetAllForEndpoint<Interview>($"opportunities/{opportunityId}/interviews", take);
        }

        public Task<IEnumerable<Interview>> InterviewsForCandidate(string candidateId, int? take = null)
        {
            return this.GetAllForEndpoint<Interview>($"candidates/{candidateId}/interviews", take);
        }

        public Task<IEnumerable<User>> Users(int? take = null)
        {
            return this.GetAllForEndpoint<User>(nameof(this.Users), take);
        }

        public Task<IEnumerable<Application>> ApplicationsForCandidate(string candidateId, int? take = null)
        {
            return this.GetAllForEndpoint<Application>($"candidates/{candidateId}/applications", take);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
