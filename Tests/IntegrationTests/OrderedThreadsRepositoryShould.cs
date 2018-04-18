////using System;
////using System.Linq;
////using Domain.Repositories;
////using FluentAssertions;
////using Persistence;
////using Persistence.Repositories;
////using Xunit;

////namespace IntegrationTests
////{
////    public class OrderedThreadsRepositoryShould
////    {

////        private readonly IOrderedThreadsRepository boards;

////        public OrderedThreadsRepositoryShould()
////        {
////            var cf = new ClientFactory(TestConfiguration.GetConfig());
////            this.boards = new OrderedThreadsRepository(cf.GetClient());
////        }

////        [Fact]
////        public void Test()
////        {
////            var r = boards.GetOrderedThreads(new Guid("b2b39350-237c-4fe1-8cd0-cd40734ad28f"), 10, 0).Result;
////            r.Should().NotBeNull();
////            var arr = r.ToArray();
////            arr.Should().NotBeNull();
////        }
////    }
////}