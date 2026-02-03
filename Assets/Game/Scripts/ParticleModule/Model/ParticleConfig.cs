using System.Collections.Generic;
using UnityEngine;

namespace Game.ParticleModule.Config
{
    [CreateAssetMenu(fileName = "ParticleConfig", menuName = "Config/ParticleConfig")]
    public sealed class ParticleConfig : ScriptableObject
    {
        public List<ParticleData> particles;
    }
}