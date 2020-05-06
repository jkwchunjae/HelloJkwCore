using HelloJkwService.Reporra;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HelloJkwTests.ReporraTest
{
    public class ReporraRoom_Test
    {
        public ReporraRoom_Test()
        {
        }

        [Fact]
        public void CreateRoom_CheckName()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };

            var room = new ReporraRoom(option);

            Assert.Equal("ABC", room.GetRoomName());
        }

        [Fact]
        public void EnterUserToPlayer_Normal1()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user);

            Assert.Contains(room.GetPlayers(), x => x == user);
        }

        [Fact]
        public void EnterUserToPlayer_Normal2()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user);

            Assert.Contains(room.GetAllUsers(), x => x == user);
        }

        [Fact]
        public void EnterUserToPlayer_Full()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user1 = new ReporraUser();
            var user2 = new ReporraUser();
            var user3 = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user1);
            room.EnterUserToPlayer(user2);
            var result3 = room.EnterUserToPlayer(user3);

            Assert.Equal(ReporraError.RoomIsFull, result3.FailReason);
        }

        [Fact]
        public void EnterUserToPlayer_AlreadyStart()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user1 = new ReporraUser();
            var user2 = new ReporraUser();
            var user3 = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user1);
            room.EnterUserToPlayer(user2);

            room.StartGame();

            var result3 = room.EnterUserToPlayer(user3);
            Assert.Equal(ReporraError.AlreadyStart, result3.FailReason);
        }

        [Fact]
        public void EnterUserToSpectator_Normal1()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToSpectator(user);

            Assert.Contains(room.GetSpectators(), x => x == user);
        }

        [Fact]
        public void EnterUserToSpectator_Normal2()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToSpectator(user);

            Assert.Contains(room.GetAllUsers(), x => x == user);
        }

        [Fact]
        public void LeaveUser_Normal1()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user);
            room.LeaveUser(user);

            Assert.DoesNotContain(room.GetAllUsers(), x => x == user);
        }

        [Fact]
        public void StartGame_Normal()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user1 = new ReporraUser();
            var user2 = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user1);
            room.EnterUserToPlayer(user2);

            var result = room.StartGame();

            Assert.Equal(ReporraRoomStatus.Playing, room.GetStatus());
        }

        [Fact]
        public void StartGame_NotEnoughPlayer()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user1 = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user1);

            var result = room.StartGame();

            Assert.Equal(ReporraError.NotEnoughPlayer, result.FailReason);
        }

        [Fact]
        public void StartGame_AlreadyStart()
        {
            var option = new ReporraRoomOption
            {
                RoomName = "ABC",
            };
            var user1 = new ReporraUser();
            var user2 = new ReporraUser();

            var room = new ReporraRoom(option);
            room.EnterUserToPlayer(user1);
            room.EnterUserToPlayer(user2);

            room.StartGame();
            var result = room.StartGame();

            Assert.Equal(ReporraError.AlreadyStart, result.FailReason);
        }
    }
}
