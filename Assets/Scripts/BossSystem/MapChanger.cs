using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapChanger : MonoBehaviour
{
    [SerializeField] private MapDataSO mapData;
    [SerializeField] private int currentIndexMap;
    [SerializeField] private List<Button> selectMapDifficultyBtn;

    void Awake()
    {
        foreach (Transform t in transform.GetChild(2))
        {
            if (t.GetComponent<Button>() != null)
                selectMapDifficultyBtn.Add(t.GetComponent<Button>());
        }
    }
    private void OnEnable()
    {
        ChangerMap(0);
    }

    //thay đổi và lựa chọn Map mới
    public void ChangerMap(int _change)
    {
        currentIndexMap += _change;

        if (currentIndexMap < 0)
        {
            currentIndexMap = mapData.map_.Count - 1;
        }
        else if (currentIndexMap > mapData.map_.Count - 1)
        {
            currentIndexMap = 0;
        }
        LoadMapDifficultyToButton(currentIndexMap);
        ChangerMapDifficulty(selectMapDifficultyBtn.Find(x => x.interactable == true));
        MapInfo.instance.SetMapInfo(mapData.map_[currentIndexMap].mapDifficulty_[0]);
        MapDisplay.instance.DisplayMap(mapData.map_[currentIndexMap].mapDifficulty_[0]);
    }

    //tải tất cả các độ khó và bật tương tác các button độ khó tương ứng 
    public void LoadMapDifficultyToButton(int _currentIndexMap)
    {
        foreach(Button btn in selectMapDifficultyBtn)
        {
            btn.interactable = false;
        }
        for(int i = 0; i < mapData.map_[_currentIndexMap].mapDifficulty_.Count; i++)
        {
            switch (mapData.map_[_currentIndexMap].mapDifficulty_[i].mapDifficulty_)
            {
                case MapDifficulty.easy:
                    selectMapDifficultyBtn.Find(x => x.name == "EasyModeBtn").interactable = true;
                    break;
                case MapDifficulty.normal:
                    selectMapDifficultyBtn.Find(x => x.name == "NormalModeBtn").interactable = true;
                    break;
                case MapDifficulty.difficult:
                    selectMapDifficultyBtn.Find(x => x.name == "DifficultModeBtn").interactable = true;
                    break;
                case MapDifficulty.hero:
                    selectMapDifficultyBtn.Find(x => x.name == "HeroModeBtn").interactable = true;
                    break;

            }
        }
    }

    //tải thông tin Map mới chọn vào MapInfo để sẵn sàng tải khi bắt đầu màn chơi
    public void ChangerMapDifficulty(Button _modeBtn)
    {
        int indexButton_ = selectMapDifficultyBtn.FindIndex(x => x.name == _modeBtn.name);
        if (indexButton_ > -1)
        {
            for (int i = 0; i < selectMapDifficultyBtn.Count; i++)
            {
                if (i == indexButton_)
                {
                    selectMapDifficultyBtn[i].GetComponent<MapDifficultyBtn>().Selected();
                    switch (_modeBtn.name)
                    {
                        case "EasyModeBtn":
                            MapInfo.instance.SetMapInfo(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.easy));
                            MapDisplay.instance.DisplayMap(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.easy));
                            break;
                        case "NormalModeBtn":
                            MapInfo.instance.SetMapInfo(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.normal));
                            MapDisplay.instance.DisplayMap(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.normal));
                            break;
                        case "DifficultModeBtn":
                            MapInfo.instance.SetMapInfo(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.difficult));
                            MapDisplay.instance.DisplayMap(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.difficult));
                            break;
                        case "HeroModeBtn":
                            MapInfo.instance.SetMapInfo(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.hero));
                            MapDisplay.instance.DisplayMap(mapData.map_[currentIndexMap].mapDifficulty_.Find(x => x.mapDifficulty_ == MapDifficulty.hero));
                            break;
                    }
                }
                else
                {
                    selectMapDifficultyBtn[i].GetComponent<MapDifficultyBtn>().Unselected();
                }
            }
        }
    }
}
