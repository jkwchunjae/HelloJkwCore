using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public class ReporraGame
    {
        public List<IReporraUser> Users { get; } = new List<IReporraUser>();
        public IReporraUser CurrentUser { get; private set; }
        private Dictionary<IReporraUser, (int Row, int Column)> _userBasePosition
            = new Dictionary<IReporraUser, (int Row, int Column)>();

        public ReporraBoard Board { get; private set; } = null;

        public ReporraGame(IEnumerable<IReporraUser> users)
        {
            Users.AddRange(users);
        }

        public bool CreateBoard(int size)
        {
            if (Users.Count() != 2)
                return false;

            Board = new ReporraBoard(size);

            _userBasePosition.Add(Users[0], (0, 0));
            _userBasePosition.Add(Users[1], (size - 1, size - 1));

            CurrentUser = Users[0];

            Users
                .ForEach(user => user.UpdateGame(this));

            return true;
        }

        public async Task<bool> StartGame()
        {
            if (Board == null)
                return false;

            if (Users.Count() != 2)
                return false;

            while (Users.Count() == 2)
            {
                Users.ForEach(x => x.UpdateGame(this));

                var reverse = Users.First() == CurrentUser ? false : true;
                var userColor = await CurrentUser.WaitUserChoiceAsync(Board.Size, Board.GetColorString(reverse));

                var basePosition = _userBasePosition[CurrentUser];
                Board.ChangeColor(basePosition.Row, basePosition.Column, userColor);

                if (Board.IsEnded)
                    break;

                CurrentUser = GetNextUser();
            }

            Users.ForEach(x => x.UpdateGame(this));
            return true;
        }

        private IReporraUser GetNextUser()
        {
            var index = Users.FindIndex(x => x == CurrentUser);
            var nextIndex = (index + 1) % Users.Count();
            return Users[nextIndex];
        }
    }
}
