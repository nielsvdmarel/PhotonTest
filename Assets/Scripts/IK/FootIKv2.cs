using UnityEngine;
using System.Collections;

public class FootIKv2 : MonoBehaviour
{
    public LayerMask rayMask;
    public float baseOffset;
    public float alignSpeed = 5.0f;
    public bool FixColliderCenter;
    [SerializeField]
    float lIKh;
    [SerializeField]
	float rIKh;
    [SerializeField]
    float lIKw;
    [SerializeField]
    float rIKw;
    [SerializeField]
	bool lHit;
    [SerializeField]
	bool rHit;
    [SerializeField]
	bool useLIK;
    [SerializeField]
	bool useRIK;
    [SerializeField]
	Vector3 lNrm;
    [SerializeField]
	Vector3 rNrm;
    [SerializeField]
    bool groundHit;
    [SerializeField]
    float groundHeight;
    [SerializeField]
    RaycastHit groundInfo;

    [SerializeField]
    bool grounded;
	Animator anim;
	CharacterController controller;

	void Start()
	{
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
	}

	void OnAnimatorIK(int layer)
	{
        GatherGroundInfo();

        if (!groundHit)
            return;

        RayCastLeg(AvatarIKGoal.LeftFoot);
		RayCastLeg(AvatarIKGoal.RightFoot);

		Transform leftHeel = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
		Transform rightHeel = anim.GetBoneTransform(HumanBodyBones.RightFoot);

		if(!lHit) lIKh = leftHeel.position.y;
		if(!rHit) rIKh = rightHeel.position.y;
        
        SetFootPosition(lHit, AvatarIKGoal.LeftFoot, leftHeel.position, lNrm, lIKh, ref lIKw);
        SetFootPosition(rHit, AvatarIKGoal.RightFoot, rightHeel.position, rNrm, rIKh, ref rIKw);
	}

    void LateUpdate()
    {
        //
        //
        //
        if (controller.center.y > 1.31f)
        {
            controller.center = new Vector3(0, 1.31f, 0);
        }
        Vector3 lerpvector = new Vector3(0, (.91f + (Mathf.Abs(lIKh - rIKh))), 0);
        Vector3 currentCenter = new Vector3(0, controller.center.y, 0);
        controller.center = Vector3.Lerp(currentCenter, lerpvector, 1f);
        //
        //
        //
        if (FixColliderCenter)
        {

        }
    }

    private void GatherGroundInfo()
    {
        Vector3 cBase = transform.TransformPoint(controller.center) + Vector3.down * (controller.height * 0.5f - controller.radius);
        groundHit = Physics.SphereCast(cBase, controller.radius, Vector3.down, out groundInfo, Mathf.Infinity, rayMask.value);
        groundHeight = cBase.y - groundInfo.distance - controller.radius;
    }
    
	private void RayCastLeg(AvatarIKGoal ag)
	{
		if(ag == AvatarIKGoal.LeftHand || ag == AvatarIKGoal.RightHand) return;
		bool h = false;
        float baseHeight = GetBaseHeight();
        float rayheight = (transform.TransformPoint(controller.center).y - (controller.height * 0.5f - controller.radius)) - baseHeight;
		RaycastHit hit;
		Transform heel = anim.GetBoneTransform(ag == AvatarIKGoal.LeftFoot?HumanBodyBones.LeftFoot:HumanBodyBones.RightFoot);
		Vector3 heelPos = heel.position;
        heelPos.y = groundHeight + rayheight;
		if(Physics.Raycast(heelPos,Vector3.down,out hit, rayheight * 1.5f,rayMask.value))
		{
			h = true;
			if(ag == AvatarIKGoal.LeftFoot)
			{
				lHit = true;
				lIKh = hit.point.y;
				lNrm = hit.normal;
			}
			else
			{
				rHit = true;
                rIKh = hit.point.y;
				rNrm = hit.normal;
			}
		}
		if(!h)
		{
            if (ag == AvatarIKGoal.LeftFoot) lHit = false;
			else rHit = false;
            /////
            
        }
	}

    private void SetFootPosition(bool use, AvatarIKGoal ag, Vector3 heelPos, Vector3 nrm, float IKh, ref float weight)
	{
		if(ag == AvatarIKGoal.LeftHand || ag == AvatarIKGoal.RightHand) return;
        weight += (use ? 1.0f : -1.0f) * Time.deltaTime * alignSpeed;
        weight = Mathf.Clamp01(weight);

		if(use)
		{
            Vector3 rotAxis = Vector3.Cross(Vector3.up, nrm);
            float angle = Vector3.Angle(Vector3.up, nrm);
            Quaternion rot = Quaternion.AngleAxis(angle * weight, rotAxis);
            anim.SetIKRotationWeight(ag, weight);
            anim.SetIKRotation(ag, rot * anim.GetIKRotation(ag));

            float baseHeight = GetBaseHeight();
            float animHeight = (heelPos.y - baseHeight) / (rot * Vector3.up).y;
            Vector3 pos = new Vector3(heelPos.x, Mathf.Max(IKh, baseHeight) + animHeight, heelPos.z);
			anim.SetIKPositionWeight(ag, weight);
			anim.SetIKPosition(ag, pos);
		}
		else
		{
            anim.SetIKPositionWeight(ag, weight);
            anim.SetIKRotationWeight(ag, weight);
        }
	}

	public bool isGrounded()
	{
		Vector3 tempNRM;
		return isGrounded(out tempNRM);
	}

	public bool isGrounded(out Vector3 nrm)
	{
		nrm = Vector3.up;
        if (!groundHit)
            return false;

        nrm = groundInfo.normal;
        return grounded;
    }

    private float GetBaseHeight()
    {
        return transform.position.y + baseOffset;
    }
}