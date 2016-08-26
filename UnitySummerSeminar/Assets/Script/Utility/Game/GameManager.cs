using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームで扱うスクリプトの名前空間
/// </summary>
namespace TUMU.Utility.Game
{
    /// <summary>
    /// ゲームシステムを管理するマネージャークラス．
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region プロパティ/フィールド

        /// <summary>
        /// ツムの存在数
        /// </summary>
        [SerializeField]
        private int tumuNum = 45;

        /// <summary>
        /// 存在するツムリスト
        /// </summary>
        private List<GameObject> tumuList = new List<GameObject>();

        /// <summary>
        /// 削除予定のツムリスト
        /// </summary>
        private List<GameObject> removableTumuList = new List<GameObject>();

        /// <summary>
        /// 最初に選択されたツム
        /// </summary>
        private GameObject selectFirstTumu;

        /// <summary>
        /// 最後に選択されたツム
        /// </summary>
        private GameObject selectLastTumu;

        /// <summary>
        /// 選択したオブジェクトの名前
        /// </summary>
        private string currentName;

        /// <summary>
        /// ドラッグ中かどうかを判別するフラグ
        /// </summary>
        private bool nowDraggingFlag;

        /// <summary>
        /// ゲーム中かどうかを判別するフラグ
        /// </summary>
        private bool isPlaying;

        /// <summary>
        /// ドラッグ中かどうかのアクセサ
        /// </summary>
        /// <value>ドラッグ中かどうか</value>
        public bool NowDraggingFlag
        {
            set
            {
                nowDraggingFlag = value;
            }
            get
            {
                return nowDraggingFlag;
            }
        }

        /// <summary>
        /// ゲーム中かどうかのアクセサ
        /// </summary>
        /// <value>ドラッグ中かどうか</value>
        public bool IsPlaying
        {
            set
            {
                isPlaying = value;
            }
            get
            {
                return isPlaying;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 一番最初に呼ばれる．
        /// </summary>
        void Awake()
        {
            // 生成前のセットアップを行う
            TumuGenerator.Instance.SetupResource();

            // ゲームマネージャーのセットアップを行う
            Setup();
        }

        /// <summary>
        /// 最初のフレームで呼ばれる
        /// </summary>
        void Start()
        {
            // ツムの生成を呼び出す
            SendGenerateAllTumu(tumuNum);
        }

        /// <summary>
        /// フレーム毎に呼ばれる
        /// </summary>
        void Update()
        {
            // 生成準備完了かどうか
            if (!IsSetupAccept())
            {
                return;
            }
            else if (!IsPlaying)
            {
                IsPlaying = true;
            }
            
            if (Input.GetMouseButton(0) && selectFirstTumu == null)
            {
                OnDragStart();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnDragEnd();
            }
            else if (selectFirstTumu != null)
            {
                OnDragging();
            }
        }

        #endregion

        #region アクセス不可能なメソッド

        private void Setup()
        {
            IsPlaying = false;
            NowDraggingFlag = false;
            currentName = "";
        }

        /// <summary>
        /// 生成準備の完了か否かを返す
        /// </summary>
        /// <returns>生成準備の出来具合い</returns>
        private bool IsSetupAccept()
        {
            return tumuList.Count != tumuNum ? false : true;
        }

        /// <summary>
        /// 指定数分のツムを生成するように通知する
        /// </summary>
        private void SendGenerateAllTumu(int numValue)
        {
            for (int roopValue = 0; roopValue < numValue; roopValue++)
            {
                StartCoroutine(SendGenerateTumu(roopValue * 0.2f));
            }
        }

        /// <summary>
        /// 一定時間待機した後，ツムの生成を呼び出す
        /// </summary>
        /// <param name="waitSec">待機時間</param>
        IEnumerator SendGenerateTumu(float waitSec, GameObject obj = null)
        {
            yield return new WaitForSeconds(waitSec);
            if (!isPlaying)
            {
                tumuList.Add(TumuGenerator.Instance.GenerateTumu());
            }
            else
            {
                TumuGenerator.Instance.ChangeTumu(obj);
            }

        }

        /// <summary>
        /// ドラッグ開始時に処理を実行する
        /// </summary>
        private void OnDragStart()
        {
            Collider2D col = GetCurrentHitCollider();
            if (col != null)
            {
                GameObject colObj = col.gameObject;
                removableTumuList = new List<GameObject>();
                NowDraggingFlag = true;
                selectFirstTumu = colObj;
                currentName = colObj.name;
                PushToList(colObj);
            }
        }

        /// <summary>
        /// ドラッグ中に処理を実行する
        /// </summary>
        private void OnDragging()
        {
            Collider2D col = GetCurrentHitCollider();
            if (col != null)
            {
                GameObject colObj = col.gameObject;
                if (colObj.name == currentName)
                {
                    if (selectLastTumu != colObj)
                    {
                        float dist = Vector2.Distance(selectLastTumu.transform.position, colObj.transform.position);
                        if (dist <= 1.5)
                        {
                            PushToList(colObj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ドラッグ解除時に処理を実行する
        /// </summary>
        private void OnDragEnd()
        {
            if (selectFirstTumu != null)
            {
                int length = removableTumuList.Count;
                if (length >= 3)
                {
                    ClearRemovables();
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        GameObject ListedObj = removableTumuList[i];
                        ChangeColor(ListedObj, 1.0f);
                        ListedObj.name = ListedObj.name.Substring(1, ListedObj.name.Length - 1);
                    }
                }
                selectFirstTumu = null;
            }
        }

        /// <summary>
        /// マウスオーバーしているオブジェクトを取得する
        /// </summary>
        /// <returns>マウスの位置からマウスオーバーしているオブジェクト</returns>
        private Collider2D GetCurrentHitCollider()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
            return hit.collider;
        }

        /// <summary>
        /// 色の変更を行う
        /// </summary>
        /// <param name="obj">色の変更したいオブジェクト</param>
        /// <param name="transparency">透明度</param>
        private void ChangeColor(GameObject obj, float transparency)
        {
            SpriteRenderer ObjTexture = obj.GetComponent<SpriteRenderer>();
            ObjTexture.color = new Color(ObjTexture.color.r, ObjTexture.color.g, ObjTexture.color.b, transparency);
        }

        /// <summary>
        /// 削除Listにゲームオブジェクトを追加する
        /// </summary>
        /// <param name="obj">削除予定リストに追加するオブジェクト</param></param>
        private void PushToList(GameObject obj)
        {
            selectLastTumu = obj;
            ChangeColor(selectLastTumu, 0.5f);
            removableTumuList.Add(obj);
            obj.name = "_" + obj.name;
        }

        private void ClearRemovables()
        {
            if (removableTumuList != null)
            {
                DestroyTumu(removableTumuList);
                NowDraggingFlag = false;
            }
        }

        /// <summary>
        /// ツムの削除
        /// </summary>
        private void DestroyTumu(List<GameObject> removeObjList)
        {
            foreach (GameObject removeObj in removeObjList)
            {
                ChangeColor(removeObj, 1.0f);
                removeObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                removeObj.transform.position = new Vector3(0, 30, 0);
                removeObj.SetActive(false);
            }

            for (int roopValue = 0; roopValue < removeObjList.Count; roopValue++)
            {
                StartCoroutine(SendGenerateTumu(roopValue * 0.2f, removeObjList[roopValue]));
            }
        }

        #endregion
    }
}