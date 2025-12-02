using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ReturnOnReleaseToTarget : MonoBehaviour
{
    [Header("Target to return to")]
    public Transform target;                 

    [Header("Return Settings")]
    public bool returnInstantly = true;      
    public float returnDuration = 0.5f;      

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool isReturning = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning($"{nameof(ReturnOnReleaseToTarget)}: target �� �����, ���������� ����������� transform.");
            target = transform;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isReturning = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (returnInstantly)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ReturnRoutine());
        }
    }

    private System.Collections.IEnumerator ReturnRoutine()
    {
        isReturning = true;

        Vector3 fromPos = transform.position;
        Quaternion fromRot = transform.rotation;

        Vector3 toPos = target.position;
        Quaternion toRot = target.rotation;

        float t = 0f;
        while (t < returnDuration)
        {
            if (!isReturning) yield break;

            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / returnDuration);

            transform.position = Vector3.Lerp(fromPos, toPos, lerp);
            transform.rotation = Quaternion.Slerp(fromRot, toRot, lerp);

            yield return null;
        }

        transform.position = toPos;
        transform.rotation = toRot;
        isReturning = false;
    }
}

