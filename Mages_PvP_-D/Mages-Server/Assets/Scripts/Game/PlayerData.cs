using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _regenHp;
    [SerializeField] private int _regenMana;
    private char _team;

    private void Start()
    {
        StartCoroutine(regen());
    }

    public void AddHp(int hp)
    {
        _hp += hp;
        RealignHP();
    }

    public void AddMana(int mana)
    {
        _mana += mana;
        RealignMana();
    }

    public void AddRegenHP(int amount)
    {
        _regenHp += amount;
    }
    public void AddRegenMana(int amount)
    {
        _regenMana += amount;
    }
    public void SetTeam(char team) => _team = team;

    private void RealignHP()
    {
        _hp = Mathf.Clamp(_hp,0,_maxHP);
    }
    private void RealignMana()
    {
        _mana = Mathf.Clamp(_mana,0,_maxMana);
    }
    
    public bool HasEnoughMana(int manacost)
    {
        return _mana - manacost >= 0;
    }

    public int GetHP() => _hp;
    public int GetMana() => _mana;
    public char GetTeam() => _team;

    private IEnumerator regen()
    {
        yield return new WaitForSeconds(0.5f);
        _hp += _regenHp;
        _mana += _regenMana;
        RealignHP();
        RealignMana();
        StartCoroutine(regen());
    }
}
