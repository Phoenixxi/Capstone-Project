using System.Linq;
using UnityEngine;

public class FinalBossAttackIndicator : MonoBehaviour
{
    [SerializeField] private AnimationClip swipeAttackAnimationClip;
    [SerializeField] private AnimationClip teethAttackAnimationClip;
    [SerializeField] private GameObject swipeAttackTopBlendShapeIndicator;
    [SerializeField] private GameObject swipeAttackBottomBlendShapeIndicator;
    [SerializeField] private GameObject teethAttackBlendShapeIndicator;

    void Awake()
    {
        float timeTaken = (38f - 0f)/swipeAttackAnimationClip.frameRate;
        swipeAttackTopBlendShapeIndicator.GetComponent<BlendShapeIndicators>().SetTimeTaken(timeTaken);
        swipeAttackBottomBlendShapeIndicator.GetComponent<BlendShapeIndicators>().SetTimeTaken(timeTaken);
        timeTaken = (96f - 0f)/teethAttackAnimationClip.frameRate;
        teethAttackBlendShapeIndicator.GetComponent<BlendShapeIndicators>().SetTimeTaken(timeTaken);
    }

    public void SwipeTopIndicatorActivate()
    {
        swipeAttackTopBlendShapeIndicator.SetActive(true);
    }

    public void SwipeBottomIndicatorActivate()
    {
        swipeAttackBottomBlendShapeIndicator.SetActive(true);
    }

    public void TeethAttackIndicatorActivate()
    {
        teethAttackBlendShapeIndicator.SetActive(true);
    }
}
