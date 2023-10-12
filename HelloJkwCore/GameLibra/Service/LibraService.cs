
namespace GameLibra;

public interface ILibraService
{
    LibraGameState CreateGame(AppUser owner, string name);
    IEnumerable<LibraGameState> GetAllGames();
    LibraGameState GetGame(string id);
    void DeleteGame(string id);
}
public class LibraService : ILibraService
{
    Dictionary<string, LibraGameState> _games = new();
    public LibraGameState CreateGame(AppUser owner, string name)
    {
        var newId = GetNewId();
        var state = new GameStateBuilder()
            .SetId(newId)
            .SetName(name)
            .SetOwner(owner)
            .UseDevilsPlanRule()
            .Build();
        _games.Add(state.Id, state);

        return state;

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

    public IEnumerable<LibraGameState> GetAllGames()
    {
        var states = _games.Values.ToArray();
        return states;
    }

    public LibraGameState GetGame(string id)
    {
        if (_games.TryGetValue(id, out var state))
        {
            return state;
        }
        else
        {
            return null;
        }
    }
}