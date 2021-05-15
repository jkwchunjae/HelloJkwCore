using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineTetris
{
    public interface ITetrisService
    {
        TetrisClient GetTetrisClient();
        void CloseTetrisClient(TetrisClient client);
    }

    public class TetrisService : ITetrisService
    {
        private List<TetrisClient> ClientList { get; set; } = new();

        public TetrisClient GetTetrisClient()
        {
            TetrisClient client = new();
            ClientList.Add(client);
            return client;
        }
        public void CloseTetrisClient(TetrisClient client)
        {
            if (ClientList.Contains(client))
            {
                try
                {
                    client?.Close();
                }
                catch
                {
                }
                ClientList.Remove(client);
            }
        }
    }
}
