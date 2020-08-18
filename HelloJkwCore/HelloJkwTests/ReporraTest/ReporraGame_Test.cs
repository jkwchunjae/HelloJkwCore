using HelloJkwService.Reporra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloJkwTests.ReporraTest
{
    public class ReporraGame_Test
    {
        public ReporraGame_Test()
        {
        }

        [Fact]
        public void ReporraGameCreateBoardSuccess_WithTwoUsers()
        {
            var users = new List<IReporraUser>
            {
                new MockReporraUser(),
                new MockReporraUser(),
            };

            var game = new ReporraGame(users);
            var result = game.CreateBoard(15);

            Assert.True(result);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(15)]
        public async Task ReporraGameStartAndEnd_ShouldWork(int boardSize)
        {
            var users = new List<IReporraUser>
            {
                new MockReporraUser(),
                new MockReporraUser(),
            };

            var game = new ReporraGame(users);
            game.CreateBoard(boardSize);

            var result = await game.StartGame();

            Assert.True(result);
        }
    }
}
