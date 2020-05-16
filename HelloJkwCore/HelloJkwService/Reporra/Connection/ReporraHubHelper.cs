using System.Collections.Generic;

namespace HelloJkwService.Reporra
{
    public class ReporraHubHelper : IReporraHubHelper
    {
        private readonly object _usersLock = new object();
        private readonly Dictionary<string, (Group Group, IReporraUser User)> _users = new Dictionary<string, (Group, IReporraUser)>();

        private readonly Dictionary<string, Group> _group = new Dictionary<string, Group>();
        private readonly Dictionary<string, IReporraUser> _user = new Dictionary<string, IReporraUser>();

        public Group GetGroup(string groupName)
        {
            if (!_group.TryGetValue(groupName, out var _))
            {
                _group.Add(groupName, new Group(groupName));
            }
            return _group[groupName];
        }

        public void SetUser(IReporraUser user)
        {
            _user[user.Id] = user;
        }
        public IReporraUser GetUser(string userId)
        {
            if (_user.TryGetValue(userId, out var user))
            {
                return user;
            }
            return null;
        }
        public void RemoveUser(string userId)
        {
            if (_user.ContainsKey(userId))
            {
                _user.Remove(userId);
            }
        }

        public bool IsInAnotherGroup(string connectionId, out Group group, out IReporraUser user)
        {
            lock (_usersLock)
            {
                if (_users.ContainsKey(connectionId))
                {
                    (group, user) = _users[connectionId];
                    return true;
                }
            }
            group = null;
            user = null;
            return false;
        }

        public void EnterGroup(string connectionId, Group group, IReporraUser user)
        {
            lock (_usersLock)
            {
                _users[connectionId] = (group, user);
            }
        }

        public bool LeaveGroup(string connectionId, out Group group, out IReporraUser user)
        {
            lock (_usersLock)
            {
                if (IsInAnotherGroup(connectionId, out group, out user))
                {
                    _users.Remove(connectionId);
                    return true;
                }
            }
            return false;
        }
    }
}
