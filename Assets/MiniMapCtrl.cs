﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCtrl : MonoBehaviour {
    public GameObject Map;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject FriendPrefab;
    public GameObject FlagPrefab;
    //224 X 180
	// Use this for initialization
	void Start () {
		
	}
    Dictionary<int, GameObject> Notify = new Dictionary<int, GameObject>();
	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < Main.Instance.MeteorManager.UnitInfos.Count; i++)
        {
            if (!Notify.ContainsKey(Main.Instance.MeteorManager.UnitInfos[i].InstanceId))
            {
                GameObject obj = null;
                if (Main.Instance.MeteorManager.LocalPlayer == Main.Instance.MeteorManager.UnitInfos[i])
                    obj = GameObject.Instantiate(PlayerPrefab);
                else if (Main.Instance.MeteorManager.UnitInfos[i].Camp == EUnitCamp.EUC_FRIEND)
                    obj = GameObject.Instantiate(FriendPrefab);
                else if (Main.Instance.MeteorManager.UnitInfos[i].Camp == EUnitCamp.EUC_ENEMY)
                    obj = GameObject.Instantiate(EnemyPrefab);
                else
                    obj = GameObject.Instantiate(FlagPrefab);
                Notify.Add(Main.Instance.MeteorManager.UnitInfos[i].InstanceId, obj);
                obj.transform.SetParent(Map.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.Euler(0, 0, -Main.Instance.MeteorManager.UnitInfos[i].transform.eulerAngles.y);

                Vector2 vec = ConvertToMainPlayer(Main.Instance.MeteorManager.UnitInfos[i].transform.position);
                obj.transform.localPosition = new Vector3(vec.x, vec.y, 0);
                //主角放最上面,依次是伙伴,最后是敌人
                if (Main.Instance.MeteorManager.LocalPlayer == Main.Instance.MeteorManager.UnitInfos[i])
                    obj.transform.SetAsLastSibling();
                else if (Main.Instance.MeteorManager.UnitInfos[i].Camp == EUnitCamp.EUC_ENEMY)
                    obj.transform.SetAsFirstSibling();
                obj.name = Main.Instance.MeteorManager.UnitInfos[i].name;
                obj.SetActive(true);
            }
            else
            {
                Notify[Main.Instance.MeteorManager.UnitInfos[i].InstanceId].transform.localRotation = Quaternion.Euler(0, 0, -Main.Instance.MeteorManager.UnitInfos[i].transform.eulerAngles.y); //MeteorManager.Instance.UnitInfos[i].transform.rotation.y;
                Vector2 vec = ConvertToMainPlayer(Main.Instance.MeteorManager.UnitInfos[i].transform.position);
                Notify[Main.Instance.MeteorManager.UnitInfos[i].InstanceId].transform.localPosition = new Vector3(vec.x, vec.y, 0);
            }
        }	
	}

    Vector2 ConvertToMainPlayer(Vector3 vec)
    {
        Vector3 vec2 = Main.Instance.MeteorManager.LocalPlayer.transform.position - vec;
        return new Vector2(vec2.x / 10.0f, vec2.z / 10.0f); //224像素-从左侧往右侧 224 * 20 = 4480码,角色的视力范围约为450码,角色高度38码
    }

}
