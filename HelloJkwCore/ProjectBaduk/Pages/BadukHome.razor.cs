﻿namespace ProjectBaduk.Pages;

public partial class BadukHome : JkwPageBase
{
    private BadukBoard Board { get; set; } = new BadukBoard(19);

    private ValueTask OnGameDataSaved(BadukGameData saveGameData)
    {
        return ValueTask.CompletedTask;
    }

    private ValueTask OnGameDataDeleted()
    {
        Board = new BadukBoard(Board.Size);
        return ValueTask.CompletedTask;
    }

    private void ChangeGameData(BadukGameData gameData)
    {
        if (gameData is null)
        {
            Board = new BadukBoard(Board.Size);
        }
        else
        {
            Board = new BadukBoard(gameData.Size, gameData.StoneLog)
            {
                CurrentColor = gameData.CurrentColor,
                ChangeMode = gameData.ChangeMode,
                CurrentIndex = gameData.CurrentIndex,
                VisibleStoneIndex = gameData.VisibleStoneIndex,
            };
        }
    }
}