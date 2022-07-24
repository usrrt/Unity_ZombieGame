using System.Collections;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour
{
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State
    {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State CurrentState { get; private set; } // 현재 총의 상태
    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러
    private AudioSource gunAudioPlayer; // 총 소리 재생기


    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과


    public GunData Data;

    public int RemainAmmo; // 남아있는 모든 총알개수
    public int magAmmo; // 현재 탄창에 남아있는 탄약

    private float fireDistance = 50f; // 사정거리
    private float lastFireTime; // 총을 마지막으로 발사한 시점


    private void Awake()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;

        gunAudioPlayer = GetComponent<AudioSource>();

    }

    private void OnEnable()
    {
        // 총 상태 초기화
        RemainAmmo = Data.InitialAmmoCount;
        magAmmo = Data.MagCapacity;
        CurrentState = State.Ready;
        lastFireTime = 0f;
    }

    // 발사 시도
    public void Fire()
    {
        // 발사가능?
        // 2. 상태가 레디임
        // 3. 쿨타임이 다 찼을때
        if (CurrentState != State.Ready || Time.time < lastFireTime + Data.FireCooltime) // 상태가 레디가 아니거나 현재시간이 쿨타임+마지막타임보다 작을경우 
        {
            return; // 총안쏨
        }

        lastFireTime = Time.time;
        Shot();

    }

    // 실제 발사 처리
    private void Shot()
    {
        RaycastHit hit;
        Vector3 hitPosition;

        if (Physics.Raycast(fireTransform.position, transform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(Data.Damage, hit.point, hit.normal);
            }

            hitPosition = hit.point;
        }
        else
        {
            hitPosition = fireTransform.position + transform.forward * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));
        --magAmmo;
        if (magAmmo <= 0)
        {
            CurrentState = State.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();

        // 라인 렌더러를 활성화하여 총알 궤적을 그린다
        bulletLineRenderer.SetPosition(0, fireTransform.position); // 쏘는 위치 0
        bulletLineRenderer.SetPosition(1, hitPosition); // 맞는위치 1
        bulletLineRenderer.enabled = true;

        gunAudioPlayer.PlayOneShot(Data.ShotClip);

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload()
    {
        // 재장전불가
        // 1. 이미 재장전중
        // 2. 탄창에 총알이 가득
        // 3. 장전할 총알이 없음
        if (CurrentState == State.Reloading || RemainAmmo <= 0)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());

        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine()
    {
        // 현재 상태를 재장전 중 상태로 전환
        CurrentState = State.Reloading;

        // 소리재생
        gunAudioPlayer.PlayOneShot(Data.ReloadClip);

        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(Data.ReloadTime);

        // 총알채우기
        int ammoToFill = Mathf.Min(Data.MagCapacity - magAmmo, RemainAmmo); // 최대탄창용량 - 탄창에 남은총알, 내가 가진 총알의총량중 작은값을 할당해줌
        magAmmo += ammoToFill;
        RemainAmmo -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        CurrentState = State.Ready;
    }
}