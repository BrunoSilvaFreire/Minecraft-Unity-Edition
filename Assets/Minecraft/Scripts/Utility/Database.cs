using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.Singletons;

namespace Minecraft.Scripts.Utility {
    public class Database<T> : ScriptableSingleton<Database<T>>, IEnumerable<T> {
        [SerializeField]
        private List<T> objects;

        public static Database<T> operator +(Database<T> db, T obj) {
            db.objects.Add(obj);
            return db;
        }

        public IEnumerator<T> GetEnumerator() {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}