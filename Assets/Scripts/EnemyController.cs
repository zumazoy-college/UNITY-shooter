using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float ViewingDistance = 10f;
    //дистанция аттаки для слайма вход
    public float AttackDistance = 2f;
    //точка атаки
    public GameObject AttackPoint;

    public float AttackRange = 0.7f;
    //слой плэйер
    public LayerMask PlayerLayer;
    //количество секунд между ударами
    public int CountdownSeconds = 1;
    //
    public int Health = 30;

    public ParticleSystem DamageParticle;
    //может слайм атаковать или нет
    private bool EnableAttack = true;
    //положение игрока тоес куда должен двигаться слайм
    private Transform _Target;
    //для аи
    private NavMeshAgent _Agent;

    private Animator _Animator;

    private GameManager _GameManager;
    //текущее расстояние между игроком и плейером
    private float DistanceToPlayer;


    private void Start()
    {
        _Target = GameManager.ManagerInstance.Player.transform; //ссылка на таргет
        _Agent = GetComponent<NavMeshAgent>(); //получаем компоненты
        _Animator = GetComponent<Animator>();
        _GameManager = FindObjectOfType<GameManager>();

    }

    private void FixedUpdate()
    {
        SetAnimation();

        //дистанцию приравниваем к вектор3 и метод дистанс считает расстояние
        //между двумя точками трансформ таргет и трансформ позишион
        DistanceToPlayer = Vector3.Distance(_Target.position, transform.position);

        if (DistanceToPlayer <= ViewingDistance) //условие дистанс меньше либо ранво вьювинг
        {
            _Agent.SetDestination(_Target.position); //обращаемся
            transform.LookAt(_Target.position); //поворачиваемся в сторону игрока
            //если достанция до игрока меньше либа равна атаки дистанции и атака разрешена то вызываем корутину 
            if (DistanceToPlayer <= AttackDistance && EnableAttack) StartCoroutine(AttackCountdown());
        }
        //ну и смерть по другому 
        if (Health <= 0) Death();
    }

    //нанесение урона инт коунт колво урона
    public void DealDamage(int Count)
    {
        //обращаемся к хеалф и вычитаем каунт после чего проигрываем систему частиц
        Health -= Count;
        DamageParticle.Play();
    }

    //метод для атаки
    private void Attack()
    {
        //коллайдер массив с коллайдерами игровых объектов по которым мы домажим
        //вызываем OverlapSphere, центр сферы, радиус атаки тоес сферы,
        //и с какого слоя будет брать наши коллайдеры и возвращать массив
        Collider[] HitedColliders = Physics.OverlapSphere(AttackPoint.transform.position, AttackRange, PlayerLayer);
        EnableAttack = true;

        foreach (Collider HitedCollider in HitedColliders)
        {
            _GameManager.DamagePlayer(10); //минус 10 игроку
            Debug.Log(_GameManager.Health);
        }
    }

    //задержки между атаками
    private IEnumerator AttackCountdown()
    {
        EnableAttack = false; //выключаем
        yield return new WaitForSeconds(1f); //задержка
        Attack(); //атакуем метод
    }

    //логично смерть
    private void Death()
    {
        DamageParticle.transform.parent = null; //отделяем
        DamageParticle.Play(); //проигрываем систему частиц

        Destroy(gameObject); //уничтожаем 
    }

    //событие гизмос
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; //вызываем цвет
        Gizmos.DrawWireSphere(AttackPoint.transform.position, AttackRange); //гизмос  атаки 
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ViewingDistance); //гизмос видимости
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance); //гизмос слайм атакует
    }

    private void SetAnimation()
    {
        if (DistanceToPlayer <= AttackDistance && EnableAttack) _Animator.SetTrigger("Attack");
        else
        {
            if (DistanceToPlayer <= ViewingDistance) _Animator.SetInteger("Animation", 1);
            else _Animator.SetInteger("Animation", 0);
        }

    }
}