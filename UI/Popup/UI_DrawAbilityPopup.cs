using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DrawAbilityPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        AbilityGrid,
    }

    enum Buttons
    {
        CheckButton,
    }

    enum Texts
    {
        TitleText,
        CheckButtonText,
    }

    public Action<GameObject> _onClickAbilityCard;

    private UI_AbilityCard  _currentAbilityCard;

    private int     _cardCount = 3;
    private bool    _isCheck = false;

    private List<UI_AbilityCard> _abilityCards = new List<UI_AbilityCard>();
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);

        _onClickAbilityCard -= CheckButtonActive;
        _onClickAbilityCard += CheckButtonActive;

        PopulateAbilityCard();

        RefreshUI();

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        foreach(UI_AbilityCard abilityCard in _abilityCards)
            abilityCard.RefreshUI();

        StartCoroutine(CallPopup());
    }

    private void PopulateAbilityCard()
    {
        foreach(Transform child in GetObject((int)GameObjects.AbilityGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<_cardCount; i++)
            _abilityCards.Add(Managers.UI.MakeSubItem<UI_AbilityCard>(GetObject((int)GameObjects.AbilityGrid).transform));
    }

    private void OnClickCheckButton()
    {
        Debug.Log("OnClickCheckButton");

        if (_isCheck == false)
            return;

        // 뽑은 능력 적용
        Managers.Game.Abilities.Add(_currentAbilityCard._ability);
        Managers.Game.RefreshAbility();

        Clear();

        // 카드 뽑기
        Managers.UI.ShowPopupUI<UI_RPSPopup>().RefreshUI(); 
    }

    private void CheckButtonActive(GameObject go)
    {
        _currentAbilityCard = go.GetComponent<UI_AbilityCard>();

        if (_isCheck == true)
            return;

        _isCheck = true;
        GetButton((int)Buttons.CheckButton).GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Btn_Green");
    }

    private float maxAlpha = 180f/255f;  // 투명도 최대치
    private IEnumerator CallPopup()
    {
        Image icon = GetObject((int)GameObjects.Background).GetComponent<Image>();

        float currentAlpha = 0f;
        while (currentAlpha < maxAlpha)
        {
            yield return null;

            currentAlpha += 0.02f;
            SetColor(icon, currentAlpha);
        }
    }
    
    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    public void Clear()
    {
        _isCheck = false;
        Managers.UI.ClosePopupUI(this);
    }
}
