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

        public ReporraLobbyService()
        {
            _roomList = new List<IReporraRoom>();
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
    }
}
