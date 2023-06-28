using System.Collections.Generic;
using UnityEngine;

public class StartParticleVIPMenu : MonoBehaviour
{
    [SerializeField] private List<RandomPlaySparkle> _sparks;

    public void StartSparks()
    {
        for (int i = 0; i < _sparks.Count; i++)
        {
            _sparks[i].StartPlaySparkle();
        }
    }

    public void StopSparks()
    {
        for (int i = 0; i < _sparks.Count; i++)
        {
            _sparks[i].StopPlaySparkle();
        }
    }
}
