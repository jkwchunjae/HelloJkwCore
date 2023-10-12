using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibra.Page;

public partial class LibraHome : JkwPageBase
{
    [Inject] public ILibraService LibraService { get; set; }

    IEnumerable<LibraGameState> GameStates { get; set; }

    protected override void OnPageInitialized()
    {
        GameStates = LibraService.GetAllGames();
    }

    private void DeleteGame(string gameId)
    {
        LibraService.DeleteGame(gameId);
        StateHasChanged();
    }

}
