

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Menu;
using NoteEditor.Utility;
using NoteEditor.Notes;
using NoteEditor.DTO;
using Game.MusicSelect;
using DG.Tweening;

namespace Game.Process
{
    public class NotesController1 : SingletonMonoBehaviour<NotesController1>
    {
        [SerializeField]
        private float maxY = 1120, minY = -40; //音符显示的最高和最低位置，低于最低位置的音符会被回收

        [SerializeField] private GameObject notePrefab, holdNotePrefab, holdBarPrefab;
        
        // 添加这4个Transform用于放置音符
        [SerializeField] 
        private Transform[] noteLanes = new Transform[4];
        
        [SerializeField]
        Image targetLine, clickLine;

        [HideInInspector] 
        public float targetY, clickFieldY;

        private List<MusicDTO.Note> notesInfo;//音符信息
        private List<NoteObject> notesWaiting = new List<NoteObject>(); //等待激活的音符对象

        [SerializeField]
        public int notesPerFrame = 50; //每一帧最多生成的音符数量

        [HideInInspector] public int laneCount; //轨道数量
        [HideInInspector] public int BPM; //速度
        [HideInInspector] public int offset; //开始的偏移时间
        [HideInInspector] public float startTime = 2f; //开始游戏前等待时间
        [HideInInspector] public int playerOffset; //玩家调整的延迟

        [HideInInspector] public float frequency; //音乐频率
        [HideInInspector] public float length; //音乐时长

        private bool playing = false; //播放中
        private bool loading = false; //加载中
        private bool ready = false; //加载完毕

        [HideInInspector] public float noteMoveTime = 1.5f; //从出现到落到位置需要多久
        private float noteMoveToEndTime;
        private float noteMoveY;

        void Awake()
        {
            #region 获取判定点、判定生效区的位置
            if (targetLine == null)
            {
                Debug.Log("Target lost!");
                return;
            }

            targetY = targetLine.rectTransform.anchoredPosition.y;
            clickFieldY = clickLine.rectTransform.anchoredPosition.y;
            #endregion

            #region  获取音符下落速度信息
            int speed = PlayerSettings.Instance.speed;

            noteMoveTime = 2.5f - 0.021f * speed;
            noteMoveToEndTime = noteMoveTime * (maxY - minY) / (maxY - targetY);
            noteMoveY = (maxY - targetY) / noteMoveTime;
            #endregion

            #region  判断是否加载乐谱
            if (NotesContainer.Instance == null || NotesContainer.Instance.json == null)
            {
                Debug.LogError("Data not loaded!");
                return;
            }
            #endregion

            #region 读取乐谱信息
            var editData = JsonUtility.FromJson<MusicDTO.EditData>(NotesContainer.Instance.json);

            notesInfo = editData.notes;
            laneCount = editData.maxBlock;
            BPM = editData.BPM;
            offset = editData.offset;
            #endregion

            #region 判断是否加载音乐
            if (NotesContainer.Instance.music == null)
            {
                Debug.LogError("Music not loaded!");
                return;
            }
            #endregion

            #region  加载音乐信息
            frequency = NotesContainer.Instance.music.frequency;
            length = NotesContainer.Instance.music.length;
            #endregion

            #region 加载玩家设定
            playerOffset = PlayerSettings.Instance.playerOffset;
            #endregion
        }

        void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            playing = false;
            loading = true;
            ready = false;

            ResetMusic();

            PlayController.Instance.Init(laneCount);
            LaneController.Instance.CreateLanes(laneCount);

            StartCoroutine(GenerateNotes());            
        }

        private void ResetMusic()
        {
            MusicController.Instance.SetMusic(NotesContainer.Instance.music);
        }

        private void ClearNotes()
        {
            if (notesWaiting.Count > 0)
            {
                foreach (NoteObject gn in notesWaiting)
                {
                    Destroy(gn.gameObject);
                }
            }
            notesWaiting.Clear();
        }

        private NoteObject CreateNote(int type)
        {
            var prefab = (type == 1) ? notePrefab : holdNotePrefab;
            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);
            return obj.GetComponent<NoteObject>();
        }

        private IEnumerator GenerateNotes()
        {
            ClearNotes();
                        
            for (int i = 0; i < notesInfo.Count; i++)
            {
                var dto = notesInfo[i];
                var type = dto.type;
                var block = dto.block % 4; // 确保block在0-3范围内

                var n = CreateNote(type);
                n.Init(dto);

                notesWaiting.Add(n);

                // 将音符设置到对应的lane Transform下
                n.transform.SetParent(noteLanes[block]);
                n.transform.localScale = Vector3.one;

                //音符对象的时间
                n.time = ConvertUtils.NoteToSamples(n, frequency, BPM) + playerOffset;

                //音符对象的位置          
                float x = LaneController.Instance.GetLaneX(n.Block());
                float y = targetY + noteMoveY * MusicController.Instance.SampleToTime(n.time + offset);
                n.SetPosition(new Vector2(x, y));

                //对于长按音符，生成结尾音符和hold条
                if(type == 2)
                {
                    if(dto.notes.Count == 0)
                    {
                        Debug.Log("Hold key has no ending!");
                    }
                    else
                    {
                        var endDto = dto.notes[0];
                        var endBlock = endDto.block % 4; // 确保block在0-3范围内

                        var nn = CreateNote(2);
                        nn.Init(endDto);

                        // 将音符设置到对应的lane Transform下
                        nn.transform.SetParent(noteLanes[endBlock]);
                        nn.transform.localScale = Vector3.one;

                        //音符对象的时间
                        nn.time = ConvertUtils.NoteToSamples(nn, frequency, BPM) + playerOffset;

                        //音符对象的位置          
                        float xx = LaneController.Instance.GetLaneX(nn.Block());
                        float yy = targetY + noteMoveY * MusicController.Instance.SampleToTime(nn.time + offset);
                        nn.SetPosition(new Vector2(xx, yy));

                        //生成条带 - 这里需要决定放在哪个parent下，我放在第一个音符的parent下
                        var bar = Instantiate(holdBarPrefab, noteLanes[block]).GetComponent<HoldingBar>();
                        bar.transform.SetAsFirstSibling();
                        bar.SetPosition(x, y);
                        bar.SetHeight(yy - y);

                        //将尾判音符和长条与头部音符绑定
                        n.AddChainedNote(nn);
                        n.AddHoldingBar(bar);
                        nn.AddHoldingBar(bar);
                    }
                }
            }

            // 让所有lane一起下落
            foreach (var lane in noteLanes)
            {
                RectTransform rect = lane.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, noteMoveY * startTime);
                
                Tweener tweener = rect.DOAnchorPosY(0, startTime);
                tweener.SetEase(Ease.Linear);
                tweener.OnComplete(()=>
                {
                    ready = true;
                });
            }

            yield return null;
        }

        void Update()
        {
            #region 加载完成后，音符开始下落
            if(loading && ready)
            {
                playing = true;
                loading = false;

                MusicController.Instance.PlayMusic();
                
                // 让所有lane一起下落
                foreach (var lane in noteLanes)
                {
                    Tweener tweener = lane.GetComponent<RectTransform>().DOAnchorPosY(-noteMoveY * length, length);
                    tweener.SetEase(Ease.Linear);
                }
            }
            #endregion

            if (!playing) return;

            #region 将所有音符加入判定队列中
            while(notesWaiting.Count > 0){
                var gn = notesWaiting[0];
                notesWaiting.Remove(gn);
                PlayController.Instance.NoteEnqueue(gn);

                if(gn.Type() == 2){
                    var cn = gn.GetChainedNote();
                    PlayController.Instance.NoteEnqueue(cn);
                }
            }
            #endregion
            
            #region 音乐播放完成后，结束游戏
            if(!MusicController.Instance.IsPlaying())
            {
                playing = false;
                ComboPresenter.Instance.ShowResult();
            }
            #endregion
        }
    }
}
