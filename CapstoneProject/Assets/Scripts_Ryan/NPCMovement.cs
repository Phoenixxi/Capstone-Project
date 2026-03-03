using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using Unity.Mathematics;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private Spline spline;

    void Awake()
    {
        spline = splineContainer.Spline;
    }

    void Start()
    {
        SplineAnimate splineAnimate = GetComponentInChildren<SplineAnimate>();
        if (spline != null)
        {
            if(spline.GetLength() != 0)
            {
                splineAnimate.Container = splineContainer;
                splineAnimate.Restart(true);
            }
        }
    }
}
