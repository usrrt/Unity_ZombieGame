using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    public AudioClip ShotClip;
    public AudioClip ReloadClip;

    public float Damage = 8f;

    public int InitialAmmoCount = 100; // 내가 가진 총알개수
    public int MagCapacity = 30; // 탄알집 최대용량?

    public float FireCooltime = 0.12f;
    public float ReloadTime = 1.8f;
}
