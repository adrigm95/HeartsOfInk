using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Rawgen.Models
{
    public class CachedCollider2D : IComparable
    {
        public Vector2[] edges;
        public Vector2 offset;
        public string spriteName;

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(CachedCollider2D))
            {
                return this.spriteName.CompareTo(((CachedCollider2D)obj).spriteName);
            }
            else if (obj.GetType() == typeof(string))
            {
                return this.spriteName.CompareTo(obj);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static bool operator <(CachedCollider2D a, string b)
        {
            return a.spriteName.CompareTo(b) < 0;
        }

        public static bool operator >(CachedCollider2D a, string b)
        {
            return a.spriteName.CompareTo(b) > 0;
        }
    }
}
