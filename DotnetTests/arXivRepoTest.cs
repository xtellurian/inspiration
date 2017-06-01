using System;
using System.Threading.Tasks;
using AngularInspiration.Model;
using Xunit;

namespace DotnetTests
{
    public class arXivRepoTest
    {
        [Fact]
        public async Task Test1()
        {
            var repo = new arXivRepo();
            var x = await repo.Search("blue", 1);

        }
    }
}
