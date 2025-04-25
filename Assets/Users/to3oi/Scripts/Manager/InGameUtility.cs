using UnityEngine;

public class InGameUtility : SingletonMonoBehaviour<InGameUtility>
{
    public static readonly int MaxDeathCount = 100;
    [SerializeField] private GameObject[] _bloodPoolDecal;
    private int index = 0;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            base.Awake();

            for (int i = 0; i < MaxDeathCount; i++)
            {
                var position = SetDeathPosition(i);
                if(position == Vector3.zero) { continue;}

                var q = Quaternion.Euler(0,Random.Range(0,360),0);
                index = Random.Range(0, _bloodPoolDecal.Length);
                Instantiate(_bloodPoolDecal[index],position,q);
            }
        }
    }
        
    public static void SetDeathPosition(Vector3 deathPosition)
    {
        for (int i = MaxDeathCount - 1; 0 <= i; i--)
        {
            PlayerPrefs.SetFloat($"{i + 1}.SetDeathPosition.x",
                PlayerPrefs.GetFloat($"{i}.SetDeathPosition.x", 0));
            PlayerPrefs.SetFloat($"{i + 1}.SetDeathPosition.y",
                PlayerPrefs.GetFloat($"{i}.SetDeathPosition.y", 0));
            PlayerPrefs.SetFloat($"{i + 1}.SetDeathPosition.z",
                PlayerPrefs.GetFloat($"{i}.SetDeathPosition.z", 0));
        }

        PlayerPrefs.SetFloat($"{0}.SetDeathPosition.x", deathPosition.x);
        PlayerPrefs.SetFloat($"{0}.SetDeathPosition.y", deathPosition.y);
        PlayerPrefs.SetFloat($"{0}.SetDeathPosition.z", deathPosition.z);
    }

    public static Vector3 SetDeathPosition(int index)
    {
        return new Vector3(
            PlayerPrefs.GetFloat($"{index}.SetDeathPosition.x", 0),
            PlayerPrefs.GetFloat($"{index}.SetDeathPosition.y", 0),
            PlayerPrefs.GetFloat($"{index}.SetDeathPosition.z", 0));
    }
}
