﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Idevgame.GameState.DialogState;

public class UnitTopState:PersistDialog<UnitTopUI>
{
    public override string DialogName { get { return "UnitTopUI"; } }
    public UnitTopState(MonoBehaviour owner)
    {
        Owner = owner;
    }
}

public class UnitTopUI : Dialog {
    public Text MonsterName;
    Transform target;
    RectTransform rect;
    MeteorUnit owner;
    UnitDebugInfo Info;
    const float ViewLimit = 250.0f;
    // Use this for initialization
    public override void OnDialogStateEnter(PersistState ownerState, BaseDialogState previousDialog, object data)
    {
        base.OnDialogStateEnter(ownerState, previousDialog, data);
        rect = GetComponent<RectTransform>();
        owner = ownerState.Owner as MeteorUnit;
        Init(owner.Attr, owner.transform, owner.Camp);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (Main.Instance.CombatData.PauseAll)
            return;
        if (owner == null)
            return;
        if (owner == Main.Instance.MeteorManager.LocalPlayer)
            return;
        if (Main.Instance.MeteorManager.LocalPlayer == null)
            return;
        bool faceto = Main.Instance.MeteorManager.LocalPlayer.IsFacetoTarget(owner);
        if (faceto)
        {
            float unitDis = Vector3.Distance(owner.transform.position, Main.Instance.MeteorManager.LocalPlayer.transform.position);
            //UnityEngine.Debug.LogError(string.Format("主角面向{0}", owner.name));
            if ((unitDis <= ViewLimit) && !MonsterName.gameObject.activeSelf)
                MonsterName.gameObject.SetActive(true);
            else if ((unitDis > ViewLimit) && MonsterName.gameObject.activeSelf)
            {
                MonsterName.gameObject.SetActive(false);
                if (Info.gameObject.activeInHierarchy)
                    Info.gameObject.SetActive(false);
            }
        }
        else
        {
            //UnityEngine.Debug.LogError(string.Format("主角背向{0}", owner.name));
            if (MonsterName.gameObject.activeSelf)
                MonsterName.gameObject.SetActive(false);
            if (Info.gameObject.activeInHierarchy)
                Info.gameObject.SetActive(false);
        }
        //1.6f 1.7666f
        if (MonsterName.gameObject.activeSelf)
        {
            Vector3 vec = Main.Instance.MainCamera.WorldToScreenPoint(new Vector3(target.position.x, target.position.y + 50.0f, target.position.z));
            float scalex = vec.x / Screen.width;
            float scaley = vec.y / Screen.height;
            rect.anchoredPosition3D = new Vector3(2160.0f * scalex, 1080f * scaley, vec.z);
            if (Main.Instance.GameStateMgr.gameStatus.EnableDebugStatus)
            {
                if (!Info.gameObject.activeInHierarchy)
                    Info.gameObject.SetActive(true);
            }
         }
    }

    public void EnableInfo(bool enable)
    {
        if (Info != null && MonsterName.gameObject.activeSelf)
            Info.gameObject.SetActive(enable);
    }

    public void Init(MonsterEx attr, Transform attach, EUnitCamp camp)
    {
        owner = attach.GetComponent<MeteorUnit>();
        Info = GetComponentInChildren<UnitDebugInfo>(true);
        Info.SetOwner(owner);
        //Transform mainCanvas = GameObject.Find("Canvas").transform;
        //transform.SetParent(mainCanvas);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        transform.SetAsLastSibling();
        //rect.anchoredPosition3D = new Vector3(0, 40.0f, 0);
        //transform.localScale = Vector3.one * 0.1f;//0.07-0.03 0.07是主角与敌人距离最近时，0.03是摄像机与敌人最近时，摄像机与敌人最近时，要看得到敌人，必须距离 44左右
        MonsterName.text = attr == null ? "" : attr.Name;
        if (camp == EUnitCamp.EUC_ENEMY)
            MonsterName.color = Color.red;
        else if (camp == EUnitCamp.EUC_NONE)
            MonsterName.color = Color.white;
        target = attach;
        if (owner == Main.Instance.MeteorManager.LocalPlayer)
            MonsterName.gameObject.SetActive(false);
     }
}
