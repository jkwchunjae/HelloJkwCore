<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\SudokuModel"

public struct StrategyResult
{
    public ICell Target{ get; init; }
    public int Value { get; init; }
}

/// <summary>
/// 값을 찾는 전략
/// </summary>
public interface IValueStrategy
{
    string Name { get; }
    string Description { get; }
    bool TryFind(IBoard board, [NotNullWhen(true)] out StrategyResult? result);
}

/// <summary>
/// 후보를 지우는 전략
/// </summary>
public interface ICandidateStrategy
{
    string Name { get; }
    string Description { get; }
    
    bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result);
}

