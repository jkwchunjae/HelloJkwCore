using Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloJkwService.Reporra
{
    public class ReporraLobbyService : IReporraLobbyService
    {
        private List<IReporraRoom> _roomList;
        private object _roomListLock = new object();

        private List<IReporraUser> _userList;
        private object _userListLock = new object();

        public ReporraLobbyService()
        {
            _roomList = new List<IReporraRoom>();
            _userList = new List<IReporraUser>();
        }

        public TypedResult<IReporraRoom> CreateRoom(ReporraRoomOption option)
        {
            var room = new ReporraRoom(option);

            lock (_roomListLock)
            {
                if (_roomList.Any(x => x.GetRoomName() == option.RoomName))
                    return TypedResult<IReporraRoom>.Fail(ReporraError.DuplicatedName);

                _roomList.Add(room);
            }

            return TypedResult<IReporraRoom>.Success(room);
        }

        public Result DeleteRoom(string roomName)
        {
            lock (_roomListLock)
            {
                var room = _roomList.FirstOrDefault(x => x.GetRoomName() == roomName);
                if (room != null)
                {
                    _roomList.Remove(room);
                }
            }

            return Result.Success();
        }

        public IEnumerable<IReporraRoom> GetRoomList()
        {
            lock (_roomListLock)
            {
                return _roomList.ToArray();
            }
        }

        public IEnumerable<IReporraUser> GetUserList()
        {
            lock (_userListLock)
            {
                return _userList.ToArray();
            }
        }

        public Result EnterUser(IReporraUser user)
        {
            lock (_userListLock)
            {
                _userList.Add(user);
            }
            return Result.Success();
        }

        public Result LeaveUser(IReporraUser user)
        {
            lock (_userListLock)
            {
                if (_userList.Contains(user))
                {
                    _userList.Remove(user);
                    return Result.Success();
                }
            }
            return Result.Fail(ReporraError.NotExist);
        }
    }
}
