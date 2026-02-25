using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using Unity.Mathematics;

public class NPCMovement : MonoBehaviour
{
    [SerializeField]
    private SplineContainer splineContainer;
    [Header("Lower the number, the faster the npc will move")]
    [SerializeField]
    private float duration;
    [Header("Go back and forth? (Make false if you want the NPC to go in a circle)")]
    [SerializeField]
    private bool pingpong;

    private float t = 0f;
    private Spline spline;

    void Awake()
    {
        spline = splineContainer.Spline;
        if(duration == 0f) duration = 5f;
    }

    void Start()
    {
        if (spline == null || spline.GetLength() == 0) return;
        Tween tw = DOTween.To(() => t, x => t = x, 1f, duration).OnUpdate(() =>
        {
            Vector3 newPos = spline.EvaluatePosition(t);
            transform.position = newPos;
        });

        if(pingpong) tw.SetLoops(-1, LoopType.Yoyo);
        else tw.SetLoops(-1);
        tw.SetEase(Ease.Linear);
    }
}
