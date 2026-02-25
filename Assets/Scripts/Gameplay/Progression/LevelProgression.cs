using System.Collections.Generic;

namespace Gameplay.Progression
{
    public class LevelProgression
    {
        public int CurrentLevelIndex { get; private set; }

        public void StartNewRun() => CurrentLevelIndex = 0;

        public void Advance() => CurrentLevelIndex++;

        public bool HasNext(IReadOnlyList<LevelDataSo> levels)
            => CurrentLevelIndex + 1 < levels.Count;
    }
}