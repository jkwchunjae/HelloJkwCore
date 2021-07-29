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

        private BadukDiary Diary = null;
        private List<BadukDiary> DiaryList = new();
        private List<BadukGameData> GameDataList = new();

        private string SaveFileName = string.Empty;
        private bool DeleteFlag = false;

        private string NewDiaryName = string.Empty;
        private bool DiaryDeleteFlag = false;

        protected override async Task OnPageInitializedAsync()
        {
            if (IsAuthenticated)
            {
                await Init();
            }
        }

        private async Task Init()
        {
            DiaryList = await BadukService.GetBadukDiaryList(User);
            if (DiaryList.Any())
            {
                Diary = DiaryList.First();
                GameDataList = await BadukService.GetBadukSummaryList(Diary.Name);
            }
        }

        private void ChangeSize(int size)
        {
            Board = new BadukBoard(size);
            SaveFileName = string.Empty;
            DeleteFlag = false;
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
            if (Diary is null)
            {
                return;
            }

            var prevGameData = GameDataList.Find(x => x.Subject == SaveFileName);

            var gameData = new BadukGameData
            {
                Subject = SaveFileName,
                Favorite = prevGameData?.Favorite ?? false,
                CreateTime = prevGameData?.CreateTime ?? DateTime.Now,
                LastModifyTime = DateTime.Now,
                OwnerEmail = User.Email,
                Size = Board.Size,
                ChangeMode = Board.ChangeMode,
                CurrentColor = Board.CurrentColor,
                CurrentIndex = Board.CurrentIndex,
                VisibleStoneIndex = Board.VisibleStoneIndex,
                Memo = prevGameData?.Memo ?? string.Empty,
                StoneLog = Board.StoneLog,
            };

            await BadukService.SaveBadukGameData(Diary.Name, gameData);
            GameDataList = await BadukService.GetBadukSummaryList(Diary.Name);
            DeleteFlag = false;
        }

        private async Task DeleteBoard()
        {
            if (GameDataList.Any(x => x.Subject == SaveFileName))
            {
                await BadukService.DeleteBadukGameData(Diary.Name, SaveFileName);
                GameDataList = await BadukService.GetBadukSummaryList(Diary.Name);
                Board = new BadukBoard(Board.Size);
                SaveFileName = string.Empty;
                DeleteFlag = false;
            }
        }

        private void ChangeGameData(ChangeEventArgs args)
        {
            if (string.IsNullOrEmpty((string)args.Value))
            {
                Board = new BadukBoard(Board.Size);
                SaveFileName = string.Empty;
                DeleteFlag = false;
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
                DeleteFlag = false;
            }
        }

        private async Task CreateDiary()
        {
            if (string.IsNullOrEmpty(NewDiaryName.Trim()))
                return;

            if (NewDiaryName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return;

            await BadukService.CreateBadukDiary(User, new DiaryName(NewDiaryName));
            await Init();
        }

        private async Task DeleteDiary()
        {
            if (string.IsNullOrEmpty(NewDiaryName.Trim()))
                return;

            var diaryName = new DiaryName(NewDiaryName.Trim());
            if (DiaryList.Any(x => x.Name == diaryName))
            {
                await BadukService.DeleteBadukDiary(User, diaryName);
            }
        }

        private async Task ChangeDiaryInfo(ChangeEventArgs args)
        {
            if (int.TryParse((string)args.Value, out var index))
            {
                Diary = DiaryList[index];
                GameDataList = await BadukService.GetBadukSummaryList(Diary.Name);
                Board = new BadukBoard(Board.Size);
                SaveFileName = string.Empty;
                DeleteFlag = false;
            }
        }
    }
}
