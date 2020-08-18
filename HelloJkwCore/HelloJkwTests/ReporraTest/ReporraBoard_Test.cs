using HelloJkwService.Reporra;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HelloJkwTests.ReporraTest
{
    public class ReporraBoard_Test
    {
        [Fact]
        public void CheckEnd_ShouldWork()
        {
            var size = 5;
            var board = new ReporraBoard(size);
            Cells.Range(size, _ => size)
                .ForEach(x => board.GetCell(x.Row, x.Column).Color = ReporraColor.A);
            board.GetCell(0, 0).Color = ReporraColor.B;

            Assert.True(board.IsEnded);
        }

        [Fact]
        public void CheckNotEnd_ShouldWork()
        {
            var size = 5;
            var board = new ReporraBoard(size);
            Cells.Range(size, _ => size)
                .ForEach(x => board.GetCell(x.Row, x.Column).Color = ReporraColor.A);
            board.GetCell(0, 0).Color = ReporraColor.B;
            board.GetCell(0, 1).Color = ReporraColor.C;

            Assert.False(board.IsEnded);
        }

        [Theory]
        [InlineData(5, "DDDFCDDFFDACCDDACECFBEEFF", "FFFFCFFFFDACCDDACECFBEEFF")]
        [InlineData(15, "EEEEEEEECAABDEFAAEEAEFCDFCCDBFECEEECBACFBDCBFFEEBABEEEEFBDCBCBEACDCBBEFFBDFAFEDDCFEAEBFFDDFCEFBFADFEDCCCEECEEBEFCEEDDFFEFEEEEEEEEEFBBEEFDDCEABAAABCEACADBDFCBEABAAFADFEEBDAEFBFFDCDFBDEDDFDBCCFACFAFEAAFABDDCDBBFBEDBBADDDFAFFABA", "FFFFFFFFCAABDEFAAFFAFFCDFCCDBFECFFFCBACFBDCBFFFFBABFFFFFBDCBCBFACDCBBFFFBDFAFFDDCFEAFBFFDDFCFFBFADFFDCCCEECFFBFFCFFDDFFEFFFFFFFFFFFBBEEFDDCFABAAABCEACADBDFCBEABAAFADFEEBDAEFBFFDCDFBDEDDFDBCCFACFAFEAAFABDDCDBBFBEDBBADDDFAFFABA")]
        public void ChangeColor_ShouldWork(int size, string initColor, string expectedColor)
        {
            var board = new ReporraBoard(size);
            var colors = ConvertReporraColorEnum(initColor, size);
            
            Cells.Range(size, _ => size)
                .ForEach(x => board.GetCell(x.Row, x.Column).Color = colors[x.Row][x.Column]);

            board.ChangeColor(0, 0, ReporraColor.F);

            var afterColor = board.GetColorString(false);

            Assert.Equal(expectedColor, afterColor);
        }

        [Theory]
        [InlineData(5, 0, 0, 12, "EEEADEEAFAEEECDBEBCDBEEEA")]
        [InlineData(5, 4, 4, 10, "CFFACADDDCFBBBBCFBBBEFBBB")]
        public void CountArea_ShouldWork(int size, int row, int column, int expectedCount, string initColor)
        {
            var board = new ReporraBoard(size);
            var colors = ConvertReporraColorEnum(initColor, size);

            Cells.Range(size, _ => size)
                .ForEach(x => board.GetCell(x.Row, x.Column).Color = colors[x.Row][x.Column]);

            var actualCount = board.CountArea(row, column);

            Assert.Equal(expectedCount, actualCount);
        }

        private List<List<ReporraColor>> ConvertReporraColorEnum(string str, int size)
        {
            return Enumerable.Range(0, str.Length / size)
                .Select(x => x * size)
                .Select(x => str.Substring(x, size))
                .Select(arr => arr.Select(x => (ReporraColor)(x - 'A')).ToList())
                .ToList();
        }

        private string ConvertReporraColorString(List<List<ReporraColor>> colors)
        {
            return colors.Select(x => x.Select(e => e.ToString()).StringJoin(""))
                .StringJoin("");
        }
    }
}
