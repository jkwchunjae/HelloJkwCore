namespace HelloJkwService.Reporra
{
    public interface IReporraHubHelper
    {
        Group GetGroup(string groupName);
        void SetUser(IReporraUser user);
        IReporraUser GetUser(string userName);
        void RemoveUser(string userName);

        bool IsInAnotherGroup(string connectionId, out Group group, out IReporraUser user);
        void EnterGroup(string connectionId, Group group, IReporraUser user);
        bool LeaveGroup(string connectionId, out Group group, out IReporraUser user);
    }
}
