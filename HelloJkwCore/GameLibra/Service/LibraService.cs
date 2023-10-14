
namespace GameLibra;

public interface ILibraService
{
    GameEngine CreateGame(AppUser owner, string name);
    IEnumerable<GameEngine> GetAllGames();
    GameEngine GetGame(string id);
    void DeleteGame(string id);
}
public class LibraService : ILibraService
{
    Dictionary<string, GameEngine> _games = new();
    public GameEngine CreateGame(AppUser owner, string name)
    {
        var newId = GetNewId();
        var state = new GameStateBuilder()
            .SetId(newId)
            .SetName(name)
            .SetOwner(owner)
            .UseDevilsPlanRule()
            .Build();
        var engine = new GameEngine
        {
            State = state,
        };
        _games.Add(state.Id, engine);

        return engine;

        string GetNewId()
        {
            int id;
            do
            {
                id = StaticRandom.Next(11111, 99999);
            } while (_games.ContainsKey(id.ToString()));
            return id.ToString();
        }
    }

    public void DeleteGame(string id)
    {
        _games.Remove(id);
    }

    public IEnumerable<GameEngine> GetAllGames()
    {
        var engines = _games.Values.ToArray();
        return engines;
    }

    public GameEngine GetGame(string id)
    {
        if (_games.TryGetValue(id, out var engine))
        {
            return engine;
        }
        else
        {
            return null;
        }
    }
}