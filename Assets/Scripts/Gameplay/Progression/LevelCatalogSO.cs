using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Progression
{
    [CreateAssetMenu(menuName = "Levels Catalog")]
    public class LevelsCatalogSo : ScriptableObject
    {
        [SerializeField] private LevelDataSo[] _levels;
        public IReadOnlyList<LevelDataSo> Levels => _levels;
    }
}