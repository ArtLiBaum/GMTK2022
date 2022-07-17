using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Intsance;
    [SerializeField] private GameObject hitPrefab, landPrefab;
    private List<ParticleSystem> hits = new List<ParticleSystem>();
    private List<ParticleSystem> lands = new List<ParticleSystem>();

    private void Start()
    {
        if (Intsance)
        {
            Destroy(this);
        }
        else
        {
            Intsance = this;
        }
    }

    public void HitBurst(Vector3 location)
    {
        var ps =GetActive(true);
        ps.transform.position = location;
        ps.Play();
    }

    public void LandBurst(Vector3 location)
    {
        var ps =GetActive(false);
        ps.transform.position = location;
        ps.Play();
    }


    private ParticleSystem GetActive(bool hit)
    {
        if (hit)
        {
            foreach (var ps in hits)
            {
                if (!ps.isEmitting)
                {
                    return ps;
                }
            }

            ParticleSystem p = Instantiate(hitPrefab).GetComponent<ParticleSystem>();
            hits.Add(p);
            return p;
        }
        foreach (var ps in lands)
        {
            if (!ps.isEmitting)
            {
                return ps;
            }
        }

        ParticleSystem l = Instantiate(hitPrefab).GetComponent<ParticleSystem>();
        lands.Add(l);
        return l;
    }
}
