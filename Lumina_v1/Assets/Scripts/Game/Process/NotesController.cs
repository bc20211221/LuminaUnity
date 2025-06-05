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
    public class NotesController : SingletonMonoBehaviour<NotesController>
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

        [SerializeField] private float[] laneAngles = new float[4]; // 每个轨道的旋转角度
        [SerializeField] private Transform[] laneCenters = new Transform[4];
        [SerializeField] private float[] laneXOffsets = new float[4];

        [SerializeField] private List<Transform> lane1s = new List<Transform>();
        [SerializeField] private List<Transform> lane2s = new List<Transform>();
        [SerializeField] private List<Transform> lane3s = new List<Transform>();
        [SerializeField] private List<Transform> lane4s = new List<Transform>();

        [SerializeField] private List<float> Time1s = new List<float>();
        [SerializeField] private List<float> Time2s = new List<float>();
        [SerializeField] private List<float> Time3s = new List<float>();
        [SerializeField] private List<float> Time4s = new List<float>();

        private float noteMoveToEndTime;
        private float noteMoveY;

        private Transform lastNote;

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
            //LaneController.Instance.CreateLanes(laneCount);

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

                n.gameObject.AddComponent<UIVelocityTracker>();

                notesWaiting.Add(n);

                // 将音符设置到对应的lane Transform下
                n.transform.SetParent(noteLanes[block]);
                n.transform.localScale = Vector3.one;

                //音符对象的时间
                n.time = ConvertUtils.NoteToSamples(n, frequency, BPM) + playerOffset;

                //音符对象的位置          
                float x = LaneController.Instance.GetLaneX(n.Block());
                float y = targetY + noteMoveY * MusicController.Instance.SampleToTime(n.time + offset);

                if (block == 0)
                {
                    lane1s.Add(n.transform);
                    Time1s.Add(MusicController.Instance.SampleToTime(n.time + offset));
                }
                else if (block == 1)
                {
                    lane2s.Add(n.transform);
                    Time2s.Add(MusicController.Instance.SampleToTime(n.time + offset));
                }
                else if (block == 2)
                {
                    lane3s.Add(n.transform);
                    Time3s.Add(MusicController.Instance.SampleToTime(n.time + offset));
                }
                else if (block == 3)
                {
                    lane4s.Add(n.transform);
                    Time4s.Add(MusicController.Instance.SampleToTime(n.time + offset));
                }

                Vector2 pos = new Vector2(x, y);
                Vector2 rotatedPos = RotateAround(pos, laneCenters[block].position, laneAngles[block]);
                rotatedPos.x -= laneXOffsets[block];
                n.SetPosition(rotatedPos);

                lastNote = n.transform;

                n.transform.Rotate(0, 0, laneAngles[block]);

                //对于长按音符，生成结尾音符和hold条
                if (type == 2)
                {
                    if (dto.notes.Count == 0)
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

                        /*  if (block == 0)
                          {
                              lane1s.Add(nn.transform);
                              Time1s.Add(MusicController.Instance.SampleToTime(nn.time + offset));
                          }
                          else if (block == 1)
                          {
                              lane2s.Add(nn.transform);
                              Time2s.Add(MusicController.Instance.SampleToTime(nn.time + offset));
                          }
                          else if (block == 2)
                          {
                              lane3s.Add(nn.transform);
                              Time3s.Add(MusicController.Instance.SampleToTime(nn.time + offset));
                          }
                          else if (block == 3)
                          {
                              lane4s.Add(nn.transform);
                              Time4s.Add(MusicController.Instance.SampleToTime(n.time + offset));
                          }*/

                        Vector2 pos2 = new Vector2(xx, yy);
                        Vector2 rotatedPos2 = RotateAround(pos2, laneCenters[endBlock].position, laneAngles[endBlock]);
                        rotatedPos2.x -= laneXOffsets[endBlock];
                        nn.SetPosition(rotatedPos2);

                        //生成条带 - 这里需要决定放在哪个parent下，我放在第一个音符的parent下
                        var bar = Instantiate(holdBarPrefab, noteLanes[block]).GetComponent<HoldingBar>();
                        bar.transform.SetAsFirstSibling();
                        bar.SetPosition(rotatedPos.x, rotatedPos.y);
                        bar.SetHeight(Vector2.Distance(rotatedPos, rotatedPos2));

                        bar.transform.Rotate(0, 0, laneAngles[endBlock]);
                        nn.transform.Rotate(0, 0, laneAngles[endBlock]);
                        bar.transform.SetParent(lastNote);
                        nn.transform.SetParent(lastNote);

                        //将尾判音符和长条与头部音符绑定
                        n.AddChainedNote(nn);
                        n.AddHoldingBar(bar);
                        nn.AddHoldingBar(bar);
                    }
                }
            }

            yield return new WaitForSeconds(startTime);

            ready = true;
        }

        void Update()
        {
            #region 加载完成后，音符开始下落
            if (loading && ready)
            {
                playing = true;
                loading = false;

                MusicController.Instance.PlayMusic();

                for (int i = 0; i < lane1s.Count; i++)
                {
                    var go = lane1s[i].gameObject;
                    lane1s[i].DOMove(laneCenters[0].position, Time1s[i]).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        go.GetComponent<UIVelocityTracker>().StartMaintainingMovement();
                    });
                }

                for (int i = 0; i < lane2s.Count; i++)
                {
                    var go = lane2s[i].gameObject;
                    lane2s[i].DOMove(laneCenters[1].position, Time2s[i]).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        go.GetComponent<UIVelocityTracker>().StartMaintainingMovement();
                    });
                }

                for (int i = 0; i < lane3s.Count; i++)
                {
                    var go = lane3s[i].gameObject;
                    lane3s[i].DOMove(laneCenters[2].position, Time3s[i]).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        go.GetComponent<UIVelocityTracker>().StartMaintainingMovement();
                    });
                }

                for (int i = 0; i < lane4s.Count; i++)
                {
                    var go = lane4s[i].gameObject;
                    lane4s[i].DOMove(laneCenters[3].position, Time4s[i]).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        go.GetComponent<UIVelocityTracker>().StartMaintainingMovement();
                    });
                }
            }
            #endregion

            if (!playing) return;

            #region 将所有音符加入判定队列中
            while (notesWaiting.Count > 0)
            {
                var gn = notesWaiting[0];
                notesWaiting.Remove(gn);
                PlayController.Instance.NoteEnqueue(gn);

                if (gn.Type() == 2)
                {
                    var cn = gn.GetChainedNote();
                    PlayController.Instance.NoteEnqueue(cn);
                }
            }
            #endregion

            #region 音乐播放完成后，结束游戏
            if (!MusicController.Instance.IsPlaying())
            {
                playing = false;
                ComboPresenter.Instance.ShowResult();
            }
            #endregion
        }

        private Vector2 RotateAround(Vector2 point, Vector2 center, float angleDeg)
        {
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector2 dir = point - center;
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);
            Vector2 rotated = new Vector2(
                dir.x * cos - dir.y * sin,
                dir.x * sin + dir.y * cos
            );
            return center + rotated;
        }
    }
}