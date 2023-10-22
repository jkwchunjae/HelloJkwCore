using GameLibra.Service;

namespace GameLibra;

public interface ILibraService
{
    Task<GameEngine> CreateGameWithDevilsPlan(AppUser owner, string name);
    Task<GameEngine> CreateGame(AppUser owner, string name, LibraGameRule rule);
    IEnumerable<GameEngine> GetAllGames();
    GameEngine GetGame(string id);
    void DeleteGame(string id);
}
public class LibraService : ILibraService
{
    Dictionary<string, GameEngine> _games = new();
    public Task<GameEngine> CreateGameWithDevilsPlan(AppUser owner, string name)
    {
        return Task.Run(() =>
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
                Assistor = new LibraAssistor(),
            };
            _games.Add(state.Id, engine);

            return engine;
        });

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
    public Task<GameEngine> CreateGame(AppUser owner, string name, LibraGameRule rule)
    {
        return Task.Run(() =>
        {
            var newId = GetNewId();
            var state = new GameStateBuilder()
                .SetId(newId)
                .SetName(name)
                .SetOwner(owner)
                .SetRule(rule)
                .Build();
            var engine = new GameEngine
            {
                State = state,
                Assistor = new LibraAssistor(),
            };
            _games.Add(state.Id, engine);

            return engine;
        });

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