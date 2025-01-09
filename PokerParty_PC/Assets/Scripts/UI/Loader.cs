using UnityEngine;

public class Loader : MonoBehaviour
{
    public static Loader instance;

    [SerializeField]
    private GameObject loader;

    private void Awake()
    {
        instance = this;
    }

    public void StartLoading()
    {
        loader.SetActive(true);
    }

    public void StopLoading()
    {
        loader.SetActive(false);
    }
}
