using UnityEngine;
using System.Collections;
using GameCore;

public class CameraShake : MonoBehaviour {
    public Shake Shake;
    private Camera mainCamera;
    private Quaternion originRotation;

	// Use this for initialization
	void Awake () {
	    Services.Override(typeof(CameraShake), this);

        mainCamera = GetComponentInChildren<Camera>();
        originRotation = mainCamera.transform.localRotation;
	}

    void Update()
    {
        if (Shake.shake_intensity > 0)
        {
            Vector3 pos;
            Quaternion rot;
            Shake.DoShake(originRotation, out pos, out rot);
            mainCamera.transform.localPosition = pos;
            mainCamera.transform.localRotation = rot;
        }
        else
        {
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = originRotation;
        }
    }

    public void ApplyShake(float intensity, float timeDecay)
    {
         Shake = new Shake(intensity, timeDecay);  
    }
}

[System.Serializable]
public class Shake
{
    //public float shake_decay = 0.3f;
    public float shake_intensity;
    public float coef_shake_intensity = 0.3f;
    public float TimeDecay;
    public float startIntensity;
    private double StartTime;

    public void DoShake(Quaternion originalRot, out Vector3 position, out Quaternion rot)
    {
        position = Random.insideUnitSphere * shake_intensity * 0.1f ;
        rot = new Quaternion(
                        originalRot.x + Random.Range(-shake_intensity, shake_intensity) * .01f,
                        originalRot.y + Random.Range(-shake_intensity, shake_intensity) * .01f,
                        originalRot.z + Random.Range(-shake_intensity, shake_intensity) * .01f,
                        originalRot.w + Random.Range(-shake_intensity, shake_intensity) * .01f);
        shake_intensity = Mathf.Lerp(startIntensity, 0, (float)(Time.timeSinceLevelLoad - StartTime) / TimeDecay);
    }

    public Shake(float intensity, float timeDecay)
    {
        StartTime = Time.timeSinceLevelLoad;
        this.shake_intensity = intensity;
        this.startIntensity = intensity;
        this.TimeDecay = timeDecay;
    }
}
