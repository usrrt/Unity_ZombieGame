using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 5f; // 앞뒤 움직임의 속도
    public float RotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput _input; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody _rigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator _animator; // 플레이어 캐릭터의 애니메이터
    //Camera _charactorCamera;

    private void Awake()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        _input = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        //_charactorCamera = GetComponent<Camera>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    // rigidbody쓸땐 FixedUpdate를 쓰는게 맞음 국룰임(이때까진 편의상 Update사용)
    // 50fps고정 <- 설정에서 바꾸기 가능
    // 여기서만 사용하는 Time.fixedDeltaTime이 있다
    private void FixedUpdate()
    {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행

        //move();
        LookMouseCursor();
        //rotate();

        _animator.SetFloat(PlayerAnimID.Move, _input.MoveDirection);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    Vector3 direction = Vector3.zero;
    private void move()
    {
        // 캐릭터가 바라보는 정면벡터필요(방향)
        // 거리 = 속력 * 시간 -> MoveSpeed * Time.fixedDeltaTime
        float movementAmount = MoveSpeed * Time.fixedDeltaTime;
        float moveVertical = _input.MoveDirection;
        float moveHorizontal = _input.RotateDirection;


        // 방향 -> transform.forward, _input.MoveDirection
        direction = moveVertical * transform.forward;
        // direction.x = moveVertical;
        // direction.z = moveHorizontal;
        // if (direction != Vector3.zero)
        // {
        //     transform.forward = direction;
        // }
        // 캐릭터기준으로 앞, 뒤를 거리만큼 계산
        Vector3 offset = direction * movementAmount;

        // 이동되어야하는 위치값이 들어가야한다
        _rigidbody.MovePosition(_rigidbody.position + offset);
        // 함수밖에서 계산한다음에 넘겨줘야함
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void rotate()
    {
        float rotationAmount = _input.RotateDirection * RotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotaion = Quaternion.Euler(0f, rotationAmount, 0f);

        // +연산은 안먹힌다
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotaion);
    }

    public float rayDistance = 100f;
    private void LookMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치를 ScreenPointToRay를 이용해 카메라로 부터의 스크린 점을 통해 레이 반환
        RaycastHit raycastHit;

        LayerMask groundLayer = LayerMask.NameToLayer("Ground");

        if (Physics.Raycast(ray, out raycastHit, rayDistance, groundLayer)) // out <-간접참조
        {
            Vector3 mouseDir = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z) - transform.position;

            _animator.transform.forward = mouseDir;


            if (_input.CanMove)
            {
                _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, raycastHit.point, MoveSpeed * Time.deltaTime));
            }
        }
    }
}