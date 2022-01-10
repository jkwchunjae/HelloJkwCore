using Common;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBaduk.Pages
{
    public partial class BadukSaveDataComponent : JkwPageBase
    {
        [Parameter]
        public BadukBoard Board { get; set; }

        [Parameter]
        public EventCallback<BadukGameData> OnChangeGameData { get; set; }

        [Parameter]
        public EventCallback<BadukGameData> OnSaveBadukData { get; set; }

        [Parameter]
        public EventCallback OnDeleteGameData { get; set; }

        [Inject]
        private IBadukService BadukService { get; set; }

        private bool IsDiaryCreator => User?.HasRole(UserRole.BadukCreator) ?? false;

        private BadukDiary Diary = new() { GameDataList = new() };
        private string Memo = string.Empty;
        private string SaveFileName = string.Empty;
        private string NewDiaryName = string.Empty;
        private bool DeleteFlag = false;
        private bool DiaryDeleteFlag = false;
        private bool ShowAllSaveData = false;

        private List<BadukDiary> DiaryList = new();

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
            }
        }

        private async Task ChangeGameData(ChangeEventArgs args)
        {
            if (string.IsNullOrEmpty(((string)args.Value).Trim()))
            {
                await ClickGameData(null);
            }
            else
            {
                var gameName = (string)args.Value;
                await ClickGameData(gameName);
            }
        }

        private async Task ClickGameData(string gameName)
        {
            if (string.IsNullOrEmpty(gameName?.Trim()))
            {
                await OnChangeGameData.InvokeAsync(null);
                Memo = string.Empty;
                SaveFileName = string.Empty;
            }
            else
            {
                var gameData = await BadukService.GetBadukGameData(Diary.Name, gameName);
                await OnChangeGameData.InvokeAsync(gameData);
                Memo = gameData.Memo;
                SaveFileName = gameData.Subject;
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

        private void ChangeDiaryInfo(ChangeEventArgs args)
        {
            if (int.TryParse((string)args.Value, out var index))
            {
                Diary = DiaryList[index];
                Memo = string.Empty;
                SaveFileName = string.Empty;
                DeleteFlag = false;
            }
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

            var prevGameData = await BadukService.GetBadukGameData(Diary.Name, SaveFileName);

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
                Memo = Memo,
                StoneLog = Board.StoneLog,
            };

            Diary = await BadukService.SaveBadukGameData(Diary.Name, gameData);

            await OnSaveBadukData.InvokeAsync(gameData);

            DeleteFlag = false;
        }

        private async Task DeleteBoard()
        {
            if (Diary.GameDataList.Contains(SaveFileName))
            {
                Diary = await BadukService.DeleteBadukGameData(Diary.Name, SaveFileName);
                await OnDeleteGameData.InvokeAsync();
                Memo = string.Empty;
                SaveFileName = string.Empty;
                DeleteFlag = false;
            }
        }
    }
}
