// 인터페이스선언
// 이름앞에 I를 붙이는게 관례
// 형용사를 많이 쓴다
// 인터페이스는 '구현'한다라고 표현한다
public interface I_Flyable
{
    // 정의할 메시지
    void Fly(); // 클래스와 비슷하게 사용하면된다

}

public interface I_Eatable
{
    void Eat();
}

public interface I_Walkable
{
    void Walk();
}

public class Bird : I_Flyable, I_Eatable, I_Walkable
{
    public void Eat()
    {
        //냠냠
    }
    public void Fly()
    {
        // 난다요~
    }

    public void Walk()
    {
        // 끼발노이
    }
}