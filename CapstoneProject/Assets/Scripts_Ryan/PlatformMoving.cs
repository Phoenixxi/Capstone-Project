using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mono.Cecil.Cil;

public class PlatformMoving : MonoBehaviour
{
    [Header("List of points that the platform will move to. \n Include starting point and end point please")]
    [SerializeField] private List<Vector3> points;
    [Header("How long the platform will take \n to move from point \n to point")]
    [SerializeField] private float duration;
    [Header("How the platforms will ease to a stop")]
    [SerializeField] private Ease ease;

    void Awake()
    {
        transform.position = points[0];
    }

    void Start()
    {
        Move();
    }

    private void Move()
    {
        var Sequence = DOTween.Sequence();
        for (int i = 1; i < points.Count; i++)
        {
            Sequence.Append(transform.DOMove(points[i], duration).SetEase(ease));
        }

        Sequence.SetLoops(-1, LoopType.Yoyo);

        Sequence.Play();
    }
}
