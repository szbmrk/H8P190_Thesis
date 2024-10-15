using UnityEngine;

public class Loader : MonoBehaviour
{
    public static Loader Instance;

    [SerializeField]
    private GameObject loader;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLoading()
    {
        loader.SetActive(true);
        loader.transform.SetAsLastSibling();
    }

    public void StopLoading()
    {
        loader.SetActive(false);
    }
}
