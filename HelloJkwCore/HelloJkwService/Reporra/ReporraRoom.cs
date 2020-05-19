using Common.Core;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloJkwService.Reporra
{
    public class ReporraRoom : IReporraRoom
    {
        private static readonly int MinimumPlayerCount = 2;
        private static readonly int MaximumPlayerCount = 2;

        private string _roomName;

        private List<IReporraUser> _users;

        private ReporraRoomStatus _roomStatus;

        public string Id { get; } = Guid.NewGuid().ToString("N").Substring(StaticRandom.Next(0, 20), StaticRandom.Next(6, 10));
        public string RoomName => _roomName;
        public ReporraRoomStatus Status => _roomStatus;

        public ReporraRoom(ReporraRoomOption option)
        {
            _roomName = option.RoomName;

            _users = new List<IReporraUser>();
            _roomStatus = ReporraRoomStatus.Waiting;
        }

        public Result EnterUserToPlayer(IReporraUser user)
        {
            lock (this)
            {
                if (Status == ReporraRoomStatus.Playing)
                    return Result.Fail(ReporraError.AlreadyStart);

                if (_users.Count(x => x.IsPlayer) >= MaximumPlayerCount)
                    return Result.Fail(ReporraError.RoomIsFull);

                user.ChangeUserType(ReporraUserType.Player);
                _users.Add(user);

                return Result.Success();
            }
        }

        public Result EnterUserToSpectator(IReporraUser user)
        {
            lock (this)
            {
                user.ChangeUserType(ReporraUserType.Spectator);
                _users.Add(user);

                return Result.Success();
            }
        }

        public IEnumerable<IReporraUser> GetAllUsers()
        {
            lock (this)
            {
                return _users.ToArray();
            }
        }

        public IEnumerable<IReporraUser> GetPlayers()
        {
            lock (this)
            {
                return _users.Where(x => x.IsPlayer).ToArray();
            }
        }

        public IEnumerable<IReporraUser> GetSpectators()
        {
            lock (this)
            {
                return _users.Where(x => x.IsSpectator).ToArray();
            }
        }

        private TypedResult<IReporraUser> FindUserBy(Func<IReporraUser, bool> func)
        {
            lock (this)
            {
                var user = _users.FirstOrDefault(func);
                if (user != null)
                    return TypedResult<IReporraUser>.Success(user);

                return TypedResult<IReporraUser>.Fail();
            }
        }

        public TypedResult<IReporraUser> FindUserById(string id)
        {
            return FindUserBy(x => x.IsAuthenticated && x.Id == id);
        }

        public TypedResult<IReporraUser> FindUserByCode(string code)
        {
            return FindUserBy(x => !x.IsAuthenticated && x.Code == code);
        }

        public Result LeaveUser(IReporraUser user)
        {
            lock (this)
            {
                _users.Remove(user);

                return Result.Success();
            }
        }

        public Result StartGame()
        {
            lock (this)
            {
                if (_users.Count(x => x.IsPlayer) < MinimumPlayerCount)
                {
                    return Result.Fail(ReporraError.NotEnoughPlayer);
                }

                if (_roomStatus == ReporraRoomStatus.Playing)
                {
                    return Result.Fail(ReporraError.AlreadyStart);
                }

                _roomStatus = ReporraRoomStatus.Playing;

                return Result.Success();
            }
        }
    }
}
