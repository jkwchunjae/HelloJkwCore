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

        private List<BadukGameData> GameDataList = new();

        private string SaveFileName = string.Empty;

        protected override async Task OnPageInitializedAsync()
        {
            if (IsAuthenticated)
            {
                GameDataList = await BadukService.GetBadukSummaryList(User);
            }
        }

        private void ChangeSize(int size)
        {
            Board = new BadukBoard(size);
            SaveFileName = string.Empty;
        }

        private void ClickCell(int row, int column)
        {
            Board.ClickCell(row, column);
            StateHasChanged();
        }

        private async Task SaveBoard()
        {
            if (SaveFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return;
            }
            var gameData = new BadukGameData
            {
                Subject = SaveFileName,
                Favorite = false,
                CreateTime = DateTime.Now,
                LastModifyTime = DateTime.Now,
                OwnerEmail = User.Email,
                Size = Board.Size,
                ChangeMode = Board.ChangeMode,
                CurrentColor = Board.CurrentColor,
                CurrentIndex = Board.CurrentIndex,
                VisibleStoneIndex = Board.VisibleStoneIndex,
                Memo = string.Empty,
                StoneLog = Board.StoneLog,
            };

            await BadukService.SaveBadukGameData(User, gameData);
            GameDataList = await BadukService.GetBadukSummaryList(User);
        }

        private void ChangeLoadFileName(ChangeEventArgs args)
        {
            if (string.IsNullOrEmpty((string)args.Value))
            {
                Board = new BadukBoard(Board.Size);
                SaveFileName = string.Empty;
            }
            else if (int.TryParse((string)args.Value, out var index))
            {
                var gameData = GameDataList[index];
                JsRuntime.ConsoleLogAsync("log", gameData);

                Board = new BadukBoard(gameData.Size, gameData.StoneLog)
                {
                    CurrentColor = gameData.CurrentColor,
                    ChangeMode = gameData.ChangeMode,
                    CurrentIndex = gameData.CurrentIndex,
                    VisibleStoneIndex = gameData.VisibleStoneIndex,
                };

                SaveFileName = gameData.Subject;
            }
        }
    }
}
