using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MercenaryItem : UI_ItemDragSlot
{
    enum Images
    {
        Background,
        Icon,
    }

    enum Texts
    {
        ItemCountText,
    }

    public MercenaryStat    _mercenary;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        _icon = GetImage((int)Images.Icon);

        RefreshUI();

        return true;
    }

    public void SetInfo(MercenaryStat mercenary, int count = 1)
    {
        _mercenary = mercenary;

        SetCount(count);

        RefreshUI();
    }

    public override void RefreshUI()
    {
        if (_init == false)
            return;

        if (_mercenary.IsNull() == true)
        {
            SetColor(0);
            return;
        }

        SetColor(255);

        _icon.sprite = _mercenary.Icon;
        GetText((int)Texts.ItemCountText).text = _itemCount.ToString();
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        _mercenary = null;
        GetImage((int)Images.Icon).sprite = null;

        base.ClearSlot();
    }

#region EventHandler

    protected override void OnClickEvent(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.itemSlot.IsFakeNull() == false)
            return;

        Debug.Log("OnClickEvent");
        // TODO : 정보 or 소환 확인
    }

    protected override void OnBeginDragEvent(PointerEventData eventData)
    {
        if (_mercenary.IsNull() == true)
            return;

        Managers.Game.isDrag = true;

        UI_DragSlot.instance.itemSlot = this;

        UI_DragSlot.instance.DragSetIcon(_icon.sprite);
        UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnDragEvent(PointerEventData eventData)
    {
        // 마우스 드래그 방향으로 아이템 이동
        if (_mercenary.IsNull() == false && UI_DragSlot.instance.itemSlot.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnEndDragEvent(PointerEventData eventData)
    {
        UI_DragSlot.instance.ClearSlot();
        Managers.Game.isDrag = false;
    }

    protected override void OnDropEvent(PointerEventData eventData)
    {
        UI_DragSlot dragSlot = UI_DragSlot.instance;

        // 드래그 슬롯에 용병 정보가 존재하는가?
        if (dragSlot.GetMercenary().IsNull() == true)
            return;

        // 내 자신일 경우
        if (dragSlot.itemSlot == this)
            return;

        // 내 용병과 같은지 확인
        if (dragSlot.GetMercenary() != _mercenary)
        {
            // 다른 슬롯에 등록
            if (Managers.Game.GameScene.IsSlotCheck(dragSlot.itemSlot as UI_MercenaryItem) == false)
                Managers.Game.GameScene.MercenaryRegister(dragSlot.GetMercenary());
        }
        else
            SetColor(1);

        // 타일에서 왔으면 타일 초기화
        if (dragSlot.tile.IsFakeNull() == false)
        {
            Managers.Resource.Destroy(dragSlot.tile.mercenaryObj);
            dragSlot.tile.Clear();
        }
        
        dragSlot.DragInfoClear();
    }

#endregion
}
