using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// where -> 제네릭을 사용햇을때만 사용할수있는 제약조건
public class SingleTonBehaviour<T> : MonoBehaviour where T : MonoBehaviour // 타입 T 는 monobehaviour를 상속받아야한다
{
    // 일반화 프로그래밍은 타입을 인자처럼 전달할수있다
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }

            return;
        }

        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }

}
