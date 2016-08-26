using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// ゲームで扱うスクリプトの名前空間
/// </summary>
namespace TUMU.Utility.Game
{
    /// <summary>
    /// ツム情報の構造体
    /// </summary>
    [Serializable]
    public struct TumuObujectInfo
    {
        public int tumuId;
        public GameObject tumuObject;
        public Sprite tumuSprite;
    }

    /// <summary>
    /// ツム生成を行うクラス
    /// </summary>
    public class TumuGenerator : SingletonMonoBehaviour<TumuGenerator>
    {
        #region プロパティ/フィールド

        /// <summary>
        /// ツム情報のリスト
        /// </summary>
        [SerializeField]
        private List<TumuObujectInfo> tumuInfoList = new List<TumuObujectInfo>();

        #endregion

        #region アクセス可能なメソッド

        /// <summary>
        /// 生成で使うリソース関連のセットアップを行う
        /// </summary>
        public void SetupResource()
        {
            // ツム情報を全て設定
            SetupAllTumuObujectInfo();
        }

        /// <summary>
        /// ツムの生成を行う(初回のみ)
        /// </summary>
        public GameObject GenerateTumu()
        {
            int uniqueId = UnityEngine.Random.Range(0, tumuInfoList.Count);
            GameObject obj = (GameObject)Instantiate(tumuInfoList[uniqueId].tumuObject);
            obj.name = tumuInfoList[uniqueId].tumuObject.name + "_" + tumuInfoList[uniqueId].tumuId.ToString();
            obj.GetComponent<SpriteRenderer>().sprite = tumuInfoList[uniqueId].tumuSprite;
            obj.transform.position = SetPosition();

            return obj;
        }

        /// <summary>
        /// ツムの譲歩を変更
        /// </summary>
        /// <returns>変更後のツム</returns>
        /// <param name="obj">変更をかけるツム</param>
        public void ChangeTumu(GameObject obj)
        {
            if (obj == null)
                return;

            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
            int uniqueId = UnityEngine.Random.Range(0, tumuInfoList.Count);
            obj.name = tumuInfoList[uniqueId].tumuObject.name + "_" + tumuInfoList[uniqueId].tumuId.ToString();
            obj.GetComponent<SpriteRenderer>().sprite = tumuInfoList[uniqueId].tumuSprite;
            obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            obj.transform.position = SetPosition();
        }

        #endregion

        #region アクセス不可能なメソッド

        /// <summary>
        /// ツム情報を全て設定する．
        /// </summary>
        private void SetupAllTumuObujectInfo()
        {
            // ツム情報のリストを空に
            tumuInfoList.Clear();

            // 設定されたツム画像リストを取得
            List<Sprite> spriteList = ResourceSettings.Instance.GetTumuResourceList();

            //全ての情報をセットアップ
            for (int roopValue = 0; roopValue < spriteList.Count; roopValue++)
            {
                // ツム情報リストに追加
                tumuInfoList.Add(SetupTumuObujectInfo(roopValue, spriteList[roopValue]));
            }
        }

        /// <summary>
        /// IDからツム情報を設定する．
        /// </summary>
        /// <returns>ツム情報</returns>
        /// <param name="uniqueId">ユニークID</param>
        /// <param name="sprite">スプライト画像</param>
        private TumuObujectInfo SetupTumuObujectInfo(int uniqueId, Sprite sprite)
        {
            TumuObujectInfo tumuInfo = new TumuObujectInfo();
            tumuInfo.tumuId = uniqueId;
            tumuInfo.tumuObject = ResourceSettings.Instance.GetTumuObject();
            tumuInfo.tumuSprite = sprite;

            return tumuInfo;
        }

        /// <summary>
        /// 生成位置の設定を行う
        /// </summary>
        /// <returns>生成位置</returns>
        private Vector3 SetPosition()
        {
            Vector3 Position;
            Position.x = UnityEngine.Random.Range(-1.5f, 1.5f);
            Position.y = 13;
            Position.z = 0;
            return Position;
        }

        #endregion
    }
}