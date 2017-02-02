using System;
using System.Collections.Generic;

namespace Mehspot.Core.DTO
{
    public class CollectionDto<T>
    {
        public CollectionDto ()
        {
        }

        public int Count { get; set; }

        public T[] Data { get; set; }
    }
}
