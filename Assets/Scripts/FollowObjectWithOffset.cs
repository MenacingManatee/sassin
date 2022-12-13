using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectWithOffset : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The target to be followed")]
    internal GameObject followTarget;


    [Header("Variables")]
    [SerializeField, Tooltip("Offset between this object and the target to be maintained")]
    internal Vector3 offset;
    internal Vector3 rotOffset;

    [SerializeField, Tooltip("The starting position of the object")]
    internal Vector3 _startPos;


    [Header("Boolean Toggles")]
    [SerializeField, Tooltip("Is the follow currently activated")]
    internal bool followOn = true;
    [SerializeField, Tooltip("Is the rotation follow currently activated")]
    internal bool followRotOn = true;
    [SerializeField, Tooltip("Ignore offsets - set to exact position/rotation")]
    internal bool ignoreOffsets = true;

    

    // Start is called before the first frame update
    void Start()
    {
        if (!followTarget)
            Destroy(this);
        offset = transform.position - followTarget.transform.position;
        rotOffset = transform.eulerAngles - followTarget.transform.eulerAngles;
        _startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ignoreOffsets) {
            if (followOn)
                transform.position = followTarget.transform.position + offset;
            else
                transform.localPosition = _startPos;
            if (followRotOn) {
                transform.eulerAngles = followTarget.transform.eulerAngles + rotOffset;
            }
        } else {
            if (followOn)
                transform.position = followTarget.transform.position;
            if (followRotOn)
                transform.rotation = followTarget.transform.rotation;
        }
    }
}
