#region Using Directives

// .NET
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Katana
using HeavensSword.Katana.Utilities.Extensions;

#endregion

namespace HeavensSword.Katana.Utilities.ObjectPool
{
	public sealed class GameObjectPool : ObjectPool<GameObject>
    {
        #region Private Fields

        private GameObject pooledObjContainer = null;
        private GameObject objectPrefab = null;

        #endregion

        #region Constructor

        public GameObjectPool( GameObject _objectPrefab, GameObject parentContainer = null, Int32 initialSize = 0,
                               GrowthMode _growthMode = GrowthMode.MODE_Double )
        {
            if( _objectPrefab == null )
                throw new ArgumentNullException( "prefab object can not be null!" );

            objectPrefab = _objectPrefab;

            // Create the container to hold our pooled GameObjects
            pooledObjContainer = parentContainer;

            objectGenerator = ObjectGenerator;
            growthMode = _growthMode;

            baseSize = initialSize;

            if( initialSize > 0 )
                pool.Resize( initialSize, objectGenerator );
        }

        #endregion

        #region Public Methods

        override public GameObject GetObject()
        {
            ResizeEmptyPool();

            ++numObjsInUse;

            var lastObj = pool[pool.Count-1];
            pool.Remove( lastObj );

            return lastObj;
        }

        override public void FreeObject( GameObject objToFree )
        {
            if( objToFree != null )
            {
                ParentObjectToChild( pooledObjContainer, objToFree );

                objToFree.SetActive( false );

                pool.Add( objToFree );
                --numObjsInUse;
            }
        }

        public GameObject AddObjectToParent( GameObject parent )
        {
            if( parent == null )
                throw new ArgumentNullException( "parent object can not be null!" );

            var item = GetObject();

            ParentObjectToChild( parent, item );

            return item;
        }

        #endregion

        #region Private Methods

        private static void ParentObjectToChild( GameObject parent, GameObject child )
        {
            if( parent == null )
                throw new ArgumentNullException( "parent object can not be null!" );
            if( child == null )
                throw new ArgumentNullException( "child object can not be null!" );

            Transform t = child.transform;

            t.transform.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            child.layer = parent.layer;
        }

        private GameObject ObjectGenerator()
        {
            var newItem = GameObject.Instantiate( objectPrefab ) as GameObject;

            ParentObjectToChild( pooledObjContainer, newItem );
            
            newItem.SetActive( false );

            return newItem;
        }

        #endregion
    }
}
