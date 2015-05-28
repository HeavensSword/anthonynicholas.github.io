#region Using Directives

// .NET
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace HeavensSword.Katana.Utilities.Extensions
{
	public static class ListExtensions
	{
        public static void Resize<T>( this List<T> list, Int32 size, T c = default( T ) )
        {
            Int32 curSize = list.Count;

            if( size < curSize )
            {
                list.RemoveRange( size, curSize - size );
            }
            else if( size > curSize )
            {
                if( size > list.Capacity )
                    list.Capacity = size;

                list.AddRange( Enumerable.Repeat( c, size - curSize ) );
            }
        }

        public static void Resize<T>( this List<T> list, Int32 size, Func<T> objGenerator )
        {
            if( objGenerator == null )
                throw new ArgumentNullException( "List extension Resize<T> : objGenerator can not be null!" );

            Int32 curSize = list.Count;

            if( size < curSize )
            {
                list.RemoveRange( size, curSize - size );
            }
            else if( size > curSize )
            {
                if( size > list.Capacity )
                    list.Capacity = size;

                for( int i = 0; i < ( size - curSize ); ++i )
                {
                    list.Add( objGenerator() );
                }
            }
        }
	}
}
