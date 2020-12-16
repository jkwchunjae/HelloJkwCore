﻿using Common;
using Common.Extensions;
using Common.FileSystem;
using Common.User;
using Microsoft.Extensions.Configuration;
using ProjectDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Diary
{
    public class DiaryServiceTest
    {
        IDiaryService _diaryService;

        AppUser _user;
        DiaryInfo _diary;

        public DiaryServiceTest()
        {
            ConfigurationPathExtensions.SetPathConfig(new Dictionary<PathType, string>
            {
                [PathType.DiaryListFile] = "/db/diary-list.json",
                [PathType.DiaryRootPath] = "/diary",
            });

            var fs = new InMemoryFileSystem();
            _diaryService = new DiaryService(fs);

            _user = new AppUser("Test", "1234")
            {
                Email = "test@hellojkw.com",
                UserName = "test user",
            };
            _diary = new DiaryInfo
            {
                DiaryName = "test-diary",
                Id = _user.Id,
                IsSecret = false,
                Owner = _user.Email,
                Writers = new List<string>(),
                Viewers = new List<string>(),
            };
        }

        [Fact]
        public async Task CreateDiaryTest()
        {
            var diaryName = "test-diary";
            var isSecret = false;

            var diary = await _diaryService.CreateDiaryInfoAsync(_user, diaryName, isSecret);

            Assert.Equal(_user.Id, diary.Id);
            Assert.Equal(_user.Email, diary.Owner);
            Assert.Equal(diaryName, diary.DiaryName);
            Assert.Equal(isSecret, diary.IsSecret);
            Assert.Empty(diary.Writers);
            Assert.Empty(diary.Viewers);
        }

        [Fact]
        public async Task GetUserDiaryTest()
        {
            var diaryName = "test-diary";
            var isSecret = false;

            await _diaryService.CreateDiaryInfoAsync(_user, diaryName, isSecret);
            var diary = await _diaryService.GetUserDiaryInfoAsync(_user);

            Assert.Equal(diaryName, diary.DiaryName);
        }

        [Fact]
        public async Task GetLastDiary_ReturnEmpty_WhenEmptyDiary()
        {
            var diaryView = await _diaryService.GetLastDiaryViewAsync(_user, _diary);

            Assert.Null(diaryView);
        }

        [Fact]
        public async Task GetLastDiary_ReturnLast()
        {
            var lastDate = new DateTime(2020, 2, 1);
            await _diaryService.WriteDiaryAsync(_user, _diary, lastDate, "content-2");
            await _diaryService.WriteDiaryAsync(_user, _diary, lastDate.AddDays(-1), "content-1");

            var view = await _diaryService.GetLastDiaryViewAsync(_user, _diary);

            Assert.NotNull(view);
            Assert.Equal(lastDate, view.DiaryContents.First().Date);
            Assert.Equal(lastDate, view.DiaryNavigationData.Today);
        }

        [Fact]
        public async Task WriteDiary_CheckDefaults()
        {
            var date = DateTime.Today;
            var text = "test";

            var content = await _diaryService.WriteDiaryAsync(_user, _diary, date, text);

            Assert.Equal(date, content.Date);
            Assert.True(content.RegDate >= DateTime.Now.AddMinutes(-1) && content.RegDate <= DateTime.Now.AddMinutes(1));
            Assert.True(content.LastModifyDate >= DateTime.Now.AddMinutes(-1) && content.LastModifyDate <= DateTime.Now.AddMinutes(1));
            Assert.False(content.IsSecret);
            Assert.Equal(1, content.Index);
            Assert.Equal(text, content.Text);
            Assert.Equal(date.ToString("yyyyMMdd") + "_1.diary", content.GetFileName());
        }

        [Fact]
        public async Task GetDiary_CheckNavigationData()
        {
            var date = DateTime.Today;
            var nextDate = date.AddDays(1);
            await _diaryService.WriteDiaryAsync(_user, _diary, nextDate, "content-2");
            await _diaryService.WriteDiaryAsync(_user, _diary, date, "content-1");

            var view = await _diaryService.GetDiaryViewAsync(_user, _diary, date);

            Assert.Equal(_diary, view.DiaryInfo);
            Assert.Equal(date, view.DiaryNavigationData.Today);
        }

        [Fact]
        public async Task GetDiary_CheckNavigationData_NextDate()
        {
            var date = DateTime.Today;
            var nextDate = date.AddDays(1);
            await _diaryService.WriteDiaryAsync(_user, _diary, nextDate, "content-2");
            await _diaryService.WriteDiaryAsync(_user, _diary, date, "content-1");

            var view = await _diaryService.GetDiaryViewAsync(_user, _diary, date);

            Assert.Equal(date, view.DiaryNavigationData.Today);
            Assert.False(view.DiaryNavigationData.HasPrev);
            Assert.True(view.DiaryNavigationData.HasNext);
            Assert.Equal(nextDate, view.DiaryNavigationData.NextDate);
        }

        [Fact]
        public async Task GetDiary_CheckNavigationData_PrevDate()
        {
            var date = DateTime.Today;
            var prevDate = date.AddDays(-11);
            await _diaryService.WriteDiaryAsync(_user, _diary, prevDate, "content-2");
            await _diaryService.WriteDiaryAsync(_user, _diary, date, "content-1");

            var view = await _diaryService.GetDiaryViewAsync(_user, _diary, date);

            Assert.Equal(date, view.DiaryNavigationData.Today);
            Assert.True(view.DiaryNavigationData.HasPrev);
            Assert.False(view.DiaryNavigationData.HasNext);
            Assert.Equal(prevDate, view.DiaryNavigationData.PrevDate);
        }

        [Fact]
        public async Task UpdateDiary_ChangeContents()
        {
            var date = DateTime.Today;
            var text = "test content";

            var content = await _diaryService.WriteDiaryAsync(_user, _diary, date, text);
            content.Text = "modified content";
            await _diaryService.UpdateDiaryAsync(_user, _diary, new List<DiaryContent> { content });

            var view = await _diaryService.GetDiaryViewAsync(_user, _diary, date);
            var modified = view.DiaryContents.First();

            Assert.Equal(content.Text, modified.Text);
        }

        [Fact]
        public async Task UpdateDiary_DeleteContents()
        {
            var date = DateTime.Today;
            var text = "test content";

            var content = await _diaryService.WriteDiaryAsync(_user, _diary, date, text);
            content.Text = "";
            await _diaryService.UpdateDiaryAsync(_user, _diary, new List<DiaryContent> { content });

            var view = await _diaryService.GetDiaryViewAsync(_user, _diary, date);

            Assert.Empty(view.DiaryContents);
        }

        [Fact]
        public async Task UpdateDiary_ChangeAndDeleteContents()
        {
            var date = DateTime.Today;
            var text1 = "test content1";
            var text2 = "test content2";

            await _diaryService.WriteDiaryAsync(_user, _diary, date, text1);
            await _diaryService.WriteDiaryAsync(_user, _diary, date, text2);

            var viewBefore = await _diaryService.GetDiaryViewAsync(_user, _diary, date);
            viewBefore.DiaryContents[0].Text = "";
            viewBefore.DiaryContents[1].Text = "modified content";

            await _diaryService.UpdateDiaryAsync(_user, _diary, viewBefore.DiaryContents);

            var viewAfter = await _diaryService.GetDiaryViewAsync(_user, _diary, date);

            Assert.Single(viewAfter.DiaryContents);
            Assert.Equal(viewBefore.DiaryContents[1].Text, viewAfter.DiaryContents[0].Text);
            Assert.Equal(2, viewAfter.DiaryContents[0].Index);
        }
    }
}
