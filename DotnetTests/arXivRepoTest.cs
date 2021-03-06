using System;
using System.Threading.Tasks;
using AngularInspiration.Model;
using Xunit;

namespace DotnetTests
{
    public class arXivRepoTest
    {
        [Theory]
        [InlineData("blue")]
        public async Task RepoTest1(string searchTerm)
        {
            var repo = new arXivRepo() as ArticleRepo;
            var x = await repo.Search(searchTerm, 1);
            Assert.True(string.Equals(x.SearchTerm, searchTerm));
            Assert.True(x.Count >= 0);
            Assert.True(x.TotalMatching >= x.Count);
            foreach(var t in x)
            {
                Assert.NotNull(t);
            }
        }
    }
}
