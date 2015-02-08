using UnityEngine;

[System.Serializable]
public class AnimValue
{
    private int hash;

    [SerializeField]
    private float Value;

    public int Hash
    {
        get { return hash; }
    }

    [SerializeField]
    private Animator animator;
    
    [SerializeField]
    public float Min;
    [SerializeField]
    public float Max;

    [SerializeField]
    private string name;


    public AnimValue(string name)
    {
        this.name = name;
        hash = Animator.StringToHash(name);
    }

    public AnimValue(string name, float min, float max)
    {
        this.name = name;
        this.Min = min;
        this.Max = max;
        hash = Animator.StringToHash(name);    
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        if (hash == 0)
            hash = Animator.StringToHash(name);
        animator.SetFloat(hash, Value);
    }

    public float Float
    {
        get
        {
            if(hash == 0)
                hash = Animator.StringToHash(name);
            return animator.GetFloat(hash);
        }
        set
        {
            if (hash == 0)
                hash = Animator.StringToHash(name);
            if (Min != 0 && Max != 0)
            {
                float val = Mathf.Clamp(value, Min, Max);
                Value = val;
                animator.SetFloat(hash, val);
            }
            
        }
    }   
}
