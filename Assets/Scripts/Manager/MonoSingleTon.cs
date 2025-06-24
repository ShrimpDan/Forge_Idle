using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static bool isApplicationQuit = false;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (isApplicationQuit)
                return null;
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    protected virtual void Awake()
    {
        isApplicationQuit = false;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;
    }

    private void OnDestroy()
    {
        isApplicationQuit = true;
    }
}