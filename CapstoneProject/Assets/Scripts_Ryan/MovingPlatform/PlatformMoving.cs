using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mono.Cecil.Cil;

public class PlatformMoving : MonoBehaviour
{
    [Header("List of points that the platform will move to. \n Include starting point and end point please")]
    [SerializeField] private List<Vector3> points;
    [Header("How long the platform will take \nto move from point \nto point. \nIf left at 0, will default to 3")]
    [SerializeField] private float duration;
    [Header("How the platforms will ease to a stop from point to point.\n This list should have one less than the \namount of points in points.\nIf left empty, or not complete, fills list with Linear")]
    [SerializeField] private List<Ease> ease;
    [Header("Set true to have the platform wait for the player \n to land on the platform before moving, \n Set false to have the platform continously \n loop through the points.")]
    [SerializeField] private bool WaitForPlayer;
    private bool completedSequence;

    void Awake()
    {
        completedSequence = false;
        transform.position = points[0];
        if(duration == 0)
        {
            duration = 3;
        }
    }

    void Start()
    {
        if (!WaitForPlayer)
        {
            Move(WaitForPlayer);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (completedSequence) return;
        if (WaitForPlayer && other.gameObject.CompareTag("Player"))
        {
            Move(WaitForPlayer);
        }
    }

    private void Move(bool wait)
    {
        var Sequence = DOTween.Sequence();
        for (int i = 1; i < points.Count; i++)
        {
            Ease currentEase;
            if (ease == null || ease.Count <= i)
            {
                currentEase = Ease.Linear;
            }
            else
            {
                currentEase = ease[i-1];
            }
            Sequence.Append(transform.DOMove(points[i], duration).SetEase(currentEase));
        }

        if(!wait)
        {
            Sequence.SetLoops(-1, LoopType.Yoyo);
        }

        Sequence.Play();
        completedSequence = true;
    }
}
