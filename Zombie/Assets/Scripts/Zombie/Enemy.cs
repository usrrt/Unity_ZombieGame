using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 적 AI를 구현한다
public class Enemy : LivingEntity
{
    public LayerMask TargetLayer; // 추적 대상 레이어
    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    public AudioClip deathSound; // 사망시 재생할 소리
    public AudioClip hitSound; // 피격시 재생할 소리

    public float Damage = 20f; // 공격력
    public float AttackCooltime = 0.5f; // 공격 간격

    private LivingEntity _target; // 추적할 대상
    private NavMeshAgent _navMeshAgent; // 경로계산 AI 에이전트
    private Animator _animator; // 애니메이터 컴포넌트
    private AudioSource _audioPlayer; // 오디오 소스 컴포넌트
    private Renderer _renderer; // 렌더러 컴포넌트

    private float LastAttackTime; // 마지막 공격 시점

    private Collider[] _targetCandiates = new Collider[5]; // 충돌된 콜라이더
    private int _targetCandiatesCount; // 반환값 이용 용도

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool _hasTargetFound
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (_target != null && !_target.IsDead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake()
    {
        // 초기화
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioSource>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        InitialHealth = newHealth;
        Damage = newDamage;
        _navMeshAgent.speed = newSpeed;
        _renderer.material.color = skinColor;
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        _animator.SetBool(ZombieAnimID.HasTarget, _hasTargetFound);
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath()
    {
        // 살아있는 동안 무한 루프
        while (false == IsDead)
        {
            // 대상을 찾았을때
            if (_hasTargetFound)
            {
                _navMeshAgent.isStopped = false;
                // 타겟한테 가야한다
                _navMeshAgent.SetDestination(_target.transform.position);
            }
            else
            {
                _navMeshAgent.isStopped = true;

                _targetCandiatesCount = Physics.OverlapSphereNonAlloc(transform.position, 7f, _targetCandiates, TargetLayer); // 무슨 역할?

                // 순회범위
                // 콜라이더 배열의 존재이유
                // 감지된 것이 플레이어인지 확인
                // 배열안에 담아야하는 정보는?

                for (int i = 0; i < _targetCandiatesCount; i++)
                {
                    Collider targetCandidate = _targetCandiates[i];

                    LivingEntity living = targetCandidate.GetComponent<LivingEntity>();
                    Debug.Assert(living != null);

                    // 찾은 콜라이더가 살아있으면
                    if (living.IsDead == false)
                    {
                        // 그것은 타겟이다
                        _target = living;
                        // 반복문탈출
                        break;
                    }
                }
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용

        if (IsDead == false)
        {
            _audioPlayer.PlayOneShot(hitSound);
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal); // 법선벡터를 활용해 맞은부위에서 파티클이 나오게함
            // 개같이 복잡함

            hitEffect.Play();
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        _audioPlayer.PlayOneShot(deathSound);
        // 애니메이션 트리거 설정
        _animator.SetTrigger(PlayerAnimID.Die);
        // 네비메시 비활성화
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행   
        // 공격가능 판단
        // 내가 살아있는가
        // 공격 쿨타임지났는가
        if (IsDead == false && Time.time >= LastAttackTime + AttackCooltime)
        {
            LivingEntity livingEntity = other.GetComponent<LivingEntity>();
            if (livingEntity == _target)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                livingEntity.OnDamage(Damage, hitPoint, hitNormal);

                LastAttackTime = Time.time;
            }
        }

    }
}