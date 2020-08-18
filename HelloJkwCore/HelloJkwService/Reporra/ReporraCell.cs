namespace HelloJkwService.Reporra
{
    public class ReporraCell
    {
        public IReporraUser Owner { get; set; } = null;
        public int Row { get; set; }
        public int Column { get; set; }
        public ReporraColor Color { get; set; } = ReporraColor.A;

        public ReporraCell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
