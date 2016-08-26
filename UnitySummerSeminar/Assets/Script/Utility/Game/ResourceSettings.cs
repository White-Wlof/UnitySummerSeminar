using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ゲームで扱うスクリプトの名前空間
/// </summary>
namespace TUMU.Utility.Game
{
    /// <summary>
    /// リソース関連の設定を管理するクラス
    /// </summary>
    public class ResourceSettings : SingletonMonoBehaviour<ResourceSettings>
    {
        #region プロパティ/フィールド

        /// <summary>
        /// ツムの画像素材リスト
        /// </summary>
        [SerializeField]
        private List<Sprite> tumuResourceList = new List<Sprite>();

        /// <summary>
        /// ツムのオブジェクト
        /// </summary>
        [SerializeField]
        private GameObject tumuObject;

        #endregion

        #region アクセス可能なメソッド

        /// <summary>
        /// ツムオブジェクトの取得，
        /// </summary>
        /// <returns>ツムオブジェクト.</returns>
        public GameObject GetTumuObject()
        {
            if (tumuObject == null)
            {
                Debug.LogError("取得可能なツムオブジェクトがありません．");
                return null;
            }
            return tumuObject;
        }

        /// <summary>
        /// ツムの画像リストの取得．
        /// </summary>
        /// <returns>ツムの画像リスト.</returns>
        public List<Sprite> GetTumuResourceList()
        {
            if (tumuResourceList == null)
            {
                Debug.LogError("取得可能なツムの画像素材リストがありません．");
            }
            return tumuResourceList;
        }

        #endregion
       
    }
}