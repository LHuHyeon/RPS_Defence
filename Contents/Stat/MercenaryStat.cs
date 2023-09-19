using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

/*
 * File :   MercenaryStat.cs
 * Desc :   용병 스탯
 */

public class MercenaryStat : Stat
{
    [SerializeField] protected Define.GradeType     _grade;         // 등급
    [SerializeField] protected Define.JobType       _job;           // 직업
    [SerializeField] protected int                  _damage;        // 공격력
    [SerializeField] protected float                _attackRate;    // 공격 속도
    [SerializeField] protected float                _attackRange;   // 공격 사거리
    [SerializeField] protected GameObject           _projectile;    // 발사체 Prefab
    [SerializeField] protected SpriteLibraryAsset   _spriteLibrary; // 캐릭터 파츠

    public int                  Damage          { get { return _damage; }           set { _damage = value; } }
    public float                AttackRate      { get { return _attackRate; }       set { _attackRate = value; } }
    public float                AttackRange     { get { return _attackRange; }      set { _attackRange = value; } }
    public Define.GradeType     Grade           { get { return _grade; }            set { _grade = value; } }
    public Define.JobType       Job             { get { return _job; }              set { _job = value; } }
    public GameObject           Projectile      { get { return _projectile; }       set { _projectile = value; } }
    public SpriteLibraryAsset   SpriteLibrary   { get { return _spriteLibrary; }    set { _spriteLibrary = value; }}
}
