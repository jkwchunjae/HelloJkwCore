using Common;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjectBaduk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Baduk
{
    public partial class BadukHome : JkwPageBase
    {
        [Inject]
        private IBadukService BadukService { get; set; }
        private BadukBoard Board { get; set; } = new BadukBoard(19);

        private List<BadukDiary> DiaryList = new();

        private void ChangeSize(int size)
        {
            Board = new BadukBoard(size);
        }

        private ValueTask OnGameDataSaved(BadukGameData saveGameData)
        {
            return ValueTask.CompletedTask;
        }

        private ValueTask OnGameDataDeleted()
        {
            Board = new BadukBoard(Board.Size);
            return ValueTask.CompletedTask;
        }

        private void ChangeGameData(BadukGameData gameData)
        {
            if (gameData is null)
            {
                Board = new BadukBoard(Board.Size);
            }
            else
            {
                Board = new BadukBoard(gameData.Size, gameData.StoneLog)
                {
                    CurrentColor = gameData.CurrentColor,
                    ChangeMode = gameData.ChangeMode,
                    CurrentIndex = gameData.CurrentIndex,
                    VisibleStoneIndex = gameData.VisibleStoneIndex,
                };
            }
        }
    }
}
