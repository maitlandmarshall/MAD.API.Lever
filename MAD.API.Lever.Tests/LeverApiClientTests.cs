using MAD.API.Lever;
using MAD.API.Lever.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Lever.Tests
{
    [TestClass]
    public class LeverApiClientTests
    {
        private LeverApiClient client;

        [TestInitialize]
        public void Init()
        {
            string apiKey = File.ReadAllText("ApiKey.txt");
            this.client = new LeverApiClient(apiKey);
        }

        [TestMethod]
        public async Task TestRequisitionsAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Requisition> requisitions = await client.Requisitions();

            Assert.IsNotNull(requisitions);
            Assert.IsTrue(requisitions.Any());
        }

        [TestMethod]
        public async Task TestPostingsAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Posting> postings = await client.Postings();

            Assert.IsNotNull(postings);
            Assert.IsTrue(postings.Any());
        }

        [TestMethod]
        public async Task TestStagesAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Stage> stages = await client.Stages();

            Assert.IsNotNull(stages);
            Assert.IsTrue(stages.Any());
        }

        [TestMethod]
        public async Task TestOpportunitiesAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> opportunities = await client.Opportunities();

            Assert.IsNotNull(opportunities);
            Assert.IsTrue(opportunities.Any());
        }

        [TestMethod]
        public async Task TestCandidatesAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> candidates = await client.Candidates();

            Assert.IsNotNull(candidates);
            Assert.IsTrue(candidates.Any());
        }

        [TestMethod]
        public async Task TestCandidateOffersAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (Opportunity c in candidates)
            {
                var cOffers = await client.OffersForCandidate(c.Id);

                if (cOffers.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestCandidateReferralsAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (Opportunity c in candidates)
            {
                var cReferrals = await client.ReferralsForCandidate(c.Id);

                if (cReferrals.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestCandidateInterviewsAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (Opportunity c in candidates)
            {
                var cInterviews = await client.InterviewsForCandidate(c.Id);

                if (cInterviews.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestUsersAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<User> users = await client.Users();

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        public async Task TestCandidateApplicationsAsync()
        {
            LeverApiClient client = this.client;
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (Opportunity c in candidates)
            {
                var cApplications = await client.ApplicationsForCandidate(c.Id);

                if (cApplications.Any())
                    break;
            }
        }
    }
}
