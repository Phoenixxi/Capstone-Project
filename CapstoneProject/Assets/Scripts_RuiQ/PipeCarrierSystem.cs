using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class PipeCarrierSystem : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 10f;

    private bool isPassing = false;
    private GameObject carrier; // 我们的隐形载具

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPassing && spline != null)
        {
            StartCoroutine(CarrierTravelRoutine(other.gameObject));
        }
    }

    private IEnumerator CarrierTravelRoutine(GameObject player)
    {
        isPassing = true;

        // --- 🚀 第一步：制造“囚笼” ---
        carrier = new GameObject("Pipe_Carrier");
        carrier.transform.position = (Vector3)spline.EvaluatePosition(0);
        carrier.transform.forward = (Vector3)spline.EvaluateTangent(0);

        // --- 🚀 第二步：强制绑架 ---
        // 把玩家变成载具的子物体
        Transform originalParent = player.transform.parent;
        player.transform.SetParent(carrier.transform);

        // 归零玩家在载具里的相对位置（强行吸附到中心）
        player.transform.localPosition = Vector3.zero;

        // 关掉干扰组件
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        var anim = player.GetComponent<Animator>();
        if (anim) anim.enabled = false;

        float progress = 0f;
        float splineLength = spline.CalculateLength();

        // --- 🚀 第三步：只移动载具 ---
        while (progress < 1f)
        {
            progress += (speed * Time.deltaTime) / splineLength;

            // 移动载具，玩家作为子物体必须跟着走
            carrier.transform.position = (Vector3)spline.EvaluatePosition(progress);

            Vector3 tangent = (Vector3)spline.EvaluateTangent(progress);
            if (tangent != Vector3.zero)
            {
                carrier.transform.forward = tangent;
            }

            yield return null;
        }

        // --- 🚀 第四步：释放玩家 ---
        player.transform.SetParent(originalParent);
        if (cc) cc.enabled = true;
        if (anim) anim.enabled = true;

        // 销毁载具
        Destroy(carrier);

        isPassing = false;
    }
}