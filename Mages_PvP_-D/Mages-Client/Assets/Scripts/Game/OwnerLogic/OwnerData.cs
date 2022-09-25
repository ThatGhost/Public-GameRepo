using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerData : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _regenHp;
    [SerializeField] private int _regenMana;

    [SerializeField] Renderer _hpRenderer;
    [SerializeField] Renderer _manaRenderer;

    private char _team;

    public int Mana
    {
        get { return _mana; }
        set { _mana = value; RealignMana(); }
    }
    public int HP
    {
        get { return _hp; }
        set { _hp = value; RealignHP(); }
    }
    public char Team
    {
        get { return _team; }
        set { _team = value; }
    }

    private void Start()
    {
        StartCoroutine(regen());
        RealignHP();
        RealignMana();
        _hpRenderer.sharedMaterial = new Material(_hpRenderer.sharedMaterial);
        _manaRenderer.sharedMaterial = new Material(_manaRenderer.sharedMaterial);
    }

    public void AddHp(int hp,char team)
    {
        if(team != _team)
        {
            _hp += hp;
            RealignHP();
        }
    }

    public void AddMana(int mana)
    {
        _mana += mana;
        RealignMana();
    }

    public void SetRegenHP(int amount)
    {
        _regenHp = amount;
    }
    public void SetRegenMana(int amount)
    {
        _regenMana = amount;
    }


    private void RealignHP()
    {
        _hp = Mathf.Clamp(_hp, 0, _maxHP);
        _hpRenderer.sharedMaterial.SetFloat("Vector1_e581611a24db4801aef0f99c97e40bb6", _hp / (float)_maxHP);
    }
    private void RealignMana()
    {
        _mana = Mathf.Clamp(_mana, 0, _maxMana);
        _manaRenderer.sharedMaterial.SetFloat("Vector1_e581611a24db4801aef0f99c97e40bb6", _mana / (float)_maxMana);
    }

    public bool HasEnoughMana(int manacost)
    {
        return _mana - manacost >= 0;
    }

    private IEnumerator regen()
    {
        yield return new WaitForSeconds(0.5f);
        _hp+= _regenHp;
        _mana+= _regenMana;
        RealignHP();
        RealignMana();
        StartCoroutine(regen());
    }
}
