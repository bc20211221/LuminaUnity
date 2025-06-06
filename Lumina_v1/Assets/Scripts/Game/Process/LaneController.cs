﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Menu;
using NoteEditor.Utility;

namespace Game.Process
{
    public class LaneController : SingletonMonoBehaviour<LaneController>
    {
        public GameObject lanePrefab;

        public Transform laneParent;

        private List<GameObject> laneList = new List<GameObject>();

        private List<Image> imgList = new List<Image>();

        private float width;

        private bool playing = false;

        void Awake()
        {
            if(laneParent == null || laneParent == null)
            {
                Debug.LogError("Lane component lost!");
            }
            width = laneParent.GetComponent<RectTransform>().sizeDelta.x;

        }

        public void CreateLanes(int laneCount)
        {

            if (laneCount<1 || laneCount > 5)
            {
                Debug.LogError("Incorrect lane number!");
                return;
            }

            if(laneList.Count == laneCount)
            {

            }
            else
            {
                for (int i = 0; i < laneCount; i++)
                {
                    GameObject obj = Instantiate(lanePrefab, laneParent);

                    obj.name = "lane " + i;

                    obj.GetComponentInChildren<Text>().text = PlayerSettings.Instance.GetKeyCode(i).ToString();

                    laneList.Add(obj);


                    Image img = obj.transform.Find("ShowClick").GetComponent<Image>();
                    imgList.Add(img);
                }
            }

            

            playing = true;
        }

        void Update()
        {
            if (playing)
            {
                for(int i = 0; i < laneList.Count; i++)
                {
                    
                    if (Input.GetKey(PlayerSettings.Instance.GetKeyCode(i)))
                    {
                        imgList[i].gameObject.SetActive(true);

                    }
                    else
                    {
                        imgList[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public float GetLaneX(int index)
        {
            if (index < 0 || index >= laneList.Count) return -1;
            else
            {

                float delta = width / laneList.Count;

                return delta * (0.5f + index) - width/2;

            }
        }



    }
}


