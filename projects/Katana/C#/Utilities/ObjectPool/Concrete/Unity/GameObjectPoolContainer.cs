#region Using Directives

// .NET
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

#endregion

namespace HeavensSword.Katana.Utilities.ObjectPool
{
	public sealed class GameObjectPoolContainer : MonoBehaviour
    {
        #region Private Fields

        private GameObjectPool gameObjectPool = null;

        [SerializeField]
        private GameObject gameObjectPrefab = null;

        [SerializeField]
        private Int32 basePoolSize = 10;

        [SerializeField]
        private GrowthMode poolGrowthMode = GrowthMode.MODE_Double;

        #endregion

        #region MonoBehaviour Methods

        void Awake()
        {
            if( gameObjectPool == null )
            {
                gameObjectPool = new GameObjectPool( gameObjectPrefab, gameObject, basePoolSize, poolGrowthMode );
            }
        }

        #endregion

        #region Public Properties

        public GameObjectPool GameObjectPool
        {
            get { return gameObjectPool; }
        }

        #endregion
    }
}
