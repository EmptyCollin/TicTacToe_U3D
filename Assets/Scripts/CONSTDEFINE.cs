
public enum ChessState{
    empty=0,
    circle=1,
    cross=2
}

public enum GameStage{
    init=0,
    gaming=1,
    ending=2
}


public enum RoundResult
{
    new_round = 0,
    circle_win = 1,
    cross_win = 2,
    draw = 3
}

public struct ChessAction
{
    public ChessState side;
    public int logic_x;
    public int logic_y;

    public ChessAction(ChessState s, int x, int y)
    {
        side = s;
        logic_x = x;
        logic_y = y;
    }
}
