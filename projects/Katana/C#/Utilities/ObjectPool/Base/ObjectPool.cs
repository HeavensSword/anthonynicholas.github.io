#region Using Directives

// .NET
using System;
using System.Collections.Generic;

// Katana
using HeavensSword.Katana.Utilities.Extensions;

#endregion

namespace HeavensSword.Katana.Utilities.ObjectPool
{
    /// <summary>
    /// Flag specifying how the ObjectPool will increase its capacity should it not have enough objects available at the time of a call to GetObject().
    /// 
    /// MODE_Lean  : Only increase the size of the pool by 1. Keeps the overall memory footprint lower, but will have greater performance cost if the pool has to resize many times.
    /// MODE_Double: Doubles the size of the pool whenever GetObject() is called and there are no available objects. Larger memory footprint, but has better performance overall because of fewer resizes.
    /// </summary>
    public enum GrowthMode { MODE_Lean = 0, MODE_Double };

    /// <summary>
    /// Maintains a pool of T objects. Allows for pre-allocating memory for objects that are normally created and destoryed often during run-time.
    /// Will provide major performance increases since it provides object re-use without the need to destroy and create objects each time.
    /// </summary>
    /// <typeparam name="T">The type of object allocated and used by the pool.</typeparam>
	public class ObjectPool<T>
    {
        #region Protected Fields

        /// <summary>
        /// The pool of T Objects.
        /// </summary>
        protected List<T> pool = new List<T>();
        /// <summary>
        /// Delegate function that constucts an object of type T.
        /// </summary>
        protected Func<T> objectGenerator;
        /// <summary>
        /// Flag specifying how the ObjectPool will increase its capacity should it not have enough objects available at the time of a call to GetObject().
        /// 
        /// MODE_Lean  : Only increase the size of the pool by 1. Keeps the overall memory footprint lower, but will have greater performance cost if the pool has to resize many times.
        /// MODE_Double: Doubles the size of the pool whenever GetObject() is called and there are no available objects. Larger memory footprint, but has better performance overall because of fewer resizes.
        /// </summary>
        protected GrowthMode growthMode;

        /// <summary>
        /// The current number of objects in use.
        /// </summary>
        protected Int32 numObjsInUse = 0;
        /// <summary>
        /// The initial size of the pool.
        /// </summary>
        protected Int32 baseSize = 0;

        #endregion

        #region Constructors

        protected ObjectPool() {}

        /// <summary>
        /// Constructs an ObjectPool.
        /// </summary>
        /// <param name="_objectGenerator">Delegate function that constucts an object of type T.</param>
        /// <param name="initialSize">The initial size of the pool.</param>
        /// <param name="_growthMode">Flag specifying how the ObjectPool will increase its capacity should it not have enough objects available at the time of a call to GetObject().</param>
        public ObjectPool( Func<T> _objectGenerator, Int32 initialSize = 0, GrowthMode _growthMode = GrowthMode.MODE_Double )
        {
            if( _objectGenerator == null )
                throw new ArgumentNullException( "Object Pool objectGenerator can not be null!" );

            objectGenerator = _objectGenerator;
            growthMode = _growthMode;

            baseSize = initialSize;

            if( initialSize > 0 )
                pool.Resize( initialSize, objectGenerator );
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an available object of type T from the pool.
        /// If no object is available, more will be created based on the GrowthMode set for the pool.
        /// </summary>
        /// <returns>An available object of type T.</returns>
        virtual public T GetObject()
        {
            //ResizeEmptyPool();
            {// Unity is failing to compile the above function call....
                if( pool.Count <= 0 )
                {
                    switch( growthMode )
                    {
                        case GrowthMode.MODE_Double:
                            pool.Resize( baseSize, objectGenerator );
                            break;

                        case GrowthMode.MODE_Lean:
                            pool.Add( objectGenerator() );
                            break;
                    }
                }
            }

            ++numObjsInUse;

            var lastObj = pool[pool.Count-1];
            pool.Remove( lastObj );

            return lastObj;
        }

        /// <summary>
        /// Return an object of type T to the pool and makes it available for re-use.
        /// </summary>
        /// <param name="objToFree">The object of type T to free.</param>
        virtual public void FreeObject( T objToFree )
        {
            pool.Add( objToFree );
            --numObjsInUse;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Helper method that will resize the pool using the specified GrowthMode.
        /// Mode_Lean  : Will increase the size of the pool by 1.
        /// Mode_Double: Will double the current size of the pool.
        /// </summary>
        protected void ResizeEmptyPool()
        {
            if( pool.Count <= 0 )
            {
                switch( growthMode )
                {
                    case GrowthMode.MODE_Double:
                        pool.Resize( baseSize, objectGenerator );
                        break;

                    case GrowthMode.MODE_Lean:
                        pool.Add( objectGenerator() );
                        break;
                }
            }
        }

        #endregion
    }
}
