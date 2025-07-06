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
            { 
                return null;
            }
            if (instance == null)
            {
                instance = (T)FindAnyObjectByType(typeof(T));

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    protected virtual void Awake()
    {
        isApplicationQuit = false;

        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }

        else if (instance != null)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        isApplicationQuit = true;
    }
}