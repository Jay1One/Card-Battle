using Gameplay.Units;
using UnityEngine;

namespace Gameplay.Progression
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")] 
    public class LevelDataSo : ScriptableObject
    {
        [SerializeField] private Enemy[] _enemies;
        
        public Enemy[] Enemies => _enemies;
    }
}