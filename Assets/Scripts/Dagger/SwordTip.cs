using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SwordTip : MonoBehaviour
{
    [Header("References")]

    [SerializeField, Tooltip("A reference to the two edges of the blade")]
    private Collider[] edgeCols;

    [SerializeField, Tooltip("A reference to the center collider down the blade")]
    private Collider bladeCol;

    [SerializeField, Tooltip("A reference to the rigidbody of the topmost object in the sword")]
    private Rigidbody parentRB;

    [SerializeField, Tooltip("The particle to be spawned on hitting an object")]
    private GameObject hitParticle;

    [SerializeField, Tooltip("The bleeding particle to be spawned on hitting an enemy")]
    private GameObject bleedParticle;

    [SerializeField, Tooltip("A reference to a blank material for use in modifying particle materials")]
    private Material blankMaterial;

    [SerializeField, Tooltip("A reference to the ClimbSword script attached to the parent")]
    private ClimbSwordAutohand climb;

    [SerializeField, Tooltip("A reference to the audio mixer, for use in playing the collision sound")]
    private AudioSource audioM;

    [SerializeField, Tooltip("A layermask used for linecasts to determine distance from stab target")]
    private LayerMask mask = (1 << 16);

    [SerializeField, Tooltip("A reference to the SwordTipTrigger script attached to this object")]
    private SwordTipTrigger trig;


    [Header("Variables")]
    [System.NonSerialized, Tooltip("Can the sword currently be used to climb")]
    public bool canClimb = true;

    public Text txt1;
    public Text txt2;

    //Unserialized private vars

    // Material created from blankMat used to modify particle appearance
    private Material defaultMat;

    // Starting position of sword tip
    private Vector3 startPos;

    // Joint attaching tip to another object
    private FixedJoint joint;

    // Joint allowing tip to slide down blade
    private ConfigurableJoint cj;

    // spring value on cj, used to reset after overriding
    private float spring;

    // damper value on cj, used to reset after overriding
    private float damper;

    // Used to prevent dagger from hitting wall on being pulled out

    // is the dagger being used to climb the wall currently
    private bool isClimbingWall = false;

    private bool skipFrame = false;
    private bool hasNegatedForce = false;


    // Sets all private references
    void Start()
    {
        startPos = transform.localPosition;
        cj = GetComponent<ConfigurableJoint>();
        spring = cj.yDrive.positionSpring;
        damper = cj.yDrive.positionDamper;
        defaultMat = new Material(blankMaterial);
    }

    // Checks for if joint should be broken if it exists, and updates cooldown
    void Update()
    {
        //if (cd > 0) {
        //    if (GetDepthPercent() <= 0.05)
        //        waitOnCooldown = false;
        //    if (!waitOnCooldown)
        //        cd -= Time.deltaTime;
        //}
        if (txt1)
            txt1.text = string.Format("{0}\n{1}", GetDepthPercent(), joint ? true : false);
        if (txt2)
            if (joint)
                txt2.text = string.Format("{0}", joint.connectedBody.name);
            else
                txt2.text = "";
        
        if (joint) {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (GetDepthPercent() <= .05f)
                parentRB.AddForce(transform.up * 0.2f);
            if (GetDepthPercent() >= 0.8f && !hasNegatedForce) {
                parentRB.velocity *= 0f;
                parentRB.angularVelocity *= 0f;
                parentRB.Sleep();
                rb.velocity *= 0f;
                rb.angularVelocity *= 0f;
                rb.Sleep();
                hasNegatedForce = true;
            }
            parentRB.velocity *= 0.9f * Time.deltaTime;
            parentRB.angularVelocity *= 0.9f * Time.deltaTime;
            rb.velocity = rb.velocity * 0.9f * Time.deltaTime;
            rb.angularVelocity *= 0.9f * Time.deltaTime;
            if ((transform.localPosition - startPos).normalized.y >= 0.1 && Vector3.Distance(startPos, transform.localPosition) >= 0.01)
                DetachJoint();
            Vector3 s = transform.position;
            Vector3 t = trig.transform.position;
            Vector3 tmp1 = Vector3.zero;
            Vector3 tmp2 = Vector3.zero;
            tmp1.x = s.x;
            tmp2.x = t.x;
            if(Vector3.Distance(tmp1, tmp2) >= 0.1) {
                Vector3 newPos = transform.localPosition;
                newPos.x = 0f;
                transform.localPosition = newPos;
            }
            tmp1.x = 0;
            tmp1.z = s.z;
            tmp2.x = 0;
            tmp2.z = t.z;
            if (Vector3.Distance(tmp1, tmp2) >= 0.1) {
                Vector3 newPos = transform.localPosition;
                newPos.z = 0f;
                transform.localPosition = newPos;
            }
        }
        if (!joint && GetDepthPercent() >= 0.95)
            transform.localPosition = startPos;
        if (skipFrame)
            skipFrame = false;
    }

    // AttachJoint - Creates and attaches a new fixed joint to a target rigidbody
    // target: The target rigidbody
    public void AttachJoint(Rigidbody target) {
        // Add fixed joint
        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = target;
        var newJoint = new JointDrive();

        // Modify config joint (slider) to be less reactive
        newJoint.positionDamper = 10f;
        newJoint.positionSpring = 0f;
        newJoint.maximumForce = 10f;
        cj.yDrive = newJoint;

        // Modify mass of parent rb to prevent drooping and sliding in wall
        parentRB.mass = 0.01f;

        // Disable colliders to prevent any freaking out
        foreach (var col in edgeCols) {
            col.gameObject.SetActive(false);
        }
        bladeCol.gameObject.SetActive(false);

        // CD makes sure sword doesnt attempt to reattach joint on withdrawing from wall

        skipFrame = true;
    }

    // Performs actions related to stabbing and climbing on collision of sword tip with a valid object
    void OnCollisionEnter(Collision col) {
        if (!joint && canClimb) {
            for (int k=0; k < col.contacts.Length; k++) 
            {
                // Collided with a surface facing mostly upwards
                if (Vector3.Angle(col.contacts[k].normal, -transform.up) <= 45)
                {
                    canClimb = false;
                    trig.obj = col.gameObject;
                    Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                    if (!rb) {
                        rb = col.gameObject.AddComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                    StartCoroutine(SpawnParticlesAndSound(col.gameObject, col.contacts[k].point));
                    
                    AttachJoint(rb);
                    break;
                }
            }
        }
    }

    void OnCollisionExit(Collision col) {
        if (joint && col.gameObject == joint.connectedBody)
            DetachJoint();
    }

    // Detaches the fixed joint, and ensures tracking is in place
    public void DetachJoint() {
        if (joint)
            Destroy(joint);
        else
            return;

        transform.localPosition = startPos;
        var newJoint = new JointDrive();
        newJoint.positionDamper = damper;
        newJoint.positionSpring = spring;
        newJoint.maximumForce = Mathf.Infinity;
        cj.yDrive = newJoint;
        StartCoroutine(EnableColliders());
        parentRB.mass = 5f;
        parentRB.freezeRotation = false;
        //climb.trackPosition = true;
        //climb.trackRotation = true;
        isClimbingWall = false;
        hasNegatedForce = false;
    }

    // Waits for depth to be >= 80% or for the joint to be disabled
    // If depth is >= 80%, performs a sanity check then disables tracking and sets climbing hand
    public IEnumerator TurnOffTracking() {
        //yield return null; // wait exactly 1 frame, sanity check to ensure joint is in place before performning check for joint
        while (joint && GetDepthPercent() <= 0.8) {
            yield return null;
        }
        if (!joint)
            yield break;

        // Sanity check, ensures sword is in a valid position before setting climb hand
        RaycastHit hit;
        if (isClimbingWall) {
            if (Physics.Linecast(transform.position, transform.position + (transform.up * 4), out hit, mask)) {
                if (hit.distance >= 0.3f || hit.rigidbody.gameObject.tag != "Wall") {
                    DetachJoint();
                    yield break;
                }
            } else {
                yield break;
            }
        }

        if (joint && !((transform.localPosition - startPos).normalized.y >= -0.1 && Vector3.Distance(startPos, transform.localPosition) >= 0.01)) {
            //climb.trackPosition = false;
            //climb.trackRotation = false;
            climb.SetClimbingHand();
        }
    }

    // Returns the percentage of depth of the tip along the slider as a percentage represented by a decimal
    // i.e. 99% depth returns 0.99
    private float GetDepthPercent() {
        var tmp = Vector3.Distance(startPos, transform.localPosition) / (cj.linearLimit.limit * 2);
        tmp = tmp > 1 ? 1 : tmp <= 0 ? 0 : tmp;
        return tmp;
    }

    private IEnumerator EnableColliders() {
        yield return new WaitForSeconds(0.1f);
        foreach (var col in edgeCols)
            col.gameObject.SetActive(true);
        bladeCol.gameObject.SetActive(true);
    }

    private IEnumerator SpawnParticlesAndSound(GameObject obj, Vector3 pos) {
        yield return null;
        if (GetDepthPercent() <= 0)
            yield break;
        if (GetDepthPercent() <= .05)
            yield return null;
        if (obj.tag == "Wall") {
            if (!isClimbingWall) {
                // Sets static variable for hand currently anchored for climbing
                StartCoroutine(TurnOffTracking());
                isClimbingWall = true;
                audioM.Play();
            }
            else
                yield break;
        }
        GameObject newParticle;
        var enemy = obj.GetComponent<Skeleton>();

        if (enemy && enemy.blood) {
            parentRB.freezeRotation = true;
            newParticle = Instantiate(bleedParticle, pos, Quaternion.identity, null);
            var main = newParticle.GetComponent<ParticleSystem>().main;
            main.startColor = enemy.blood.color;
            newParticle.transform.LookAt(this.gameObject.transform);
            newParticle.transform.parent = obj.transform;
        }

        
        newParticle = Instantiate(hitParticle, pos, Quaternion.identity, null);
        newParticle.transform.LookAt(this.gameObject.transform);
        if (enemy)
            defaultMat.color = enemy.blood.color;
        else
            defaultMat.color = new Color(.86f, .84f, .8f); // Really needs a more dynamic option, may need a better color for mvp models
        newParticle.GetComponent<ParticleSystemRenderer>().material = defaultMat;
    
    }
}