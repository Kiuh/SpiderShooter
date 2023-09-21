using System;
using System.Collections.Generic;
using UnityEngine;

namespace AINamesGenerator
{
    public static class NickGenerator
    {
        [Serializable]
        private class NamesList
        {
            public List<string> Names;
        }

        private static NamesList namesList;

        private static NamesList CurrentNamesList
        {
            get
            {
                if (namesList is null)
                {
                    TextAsset textAsset = Resources.Load("Texts/NamesList") as TextAsset;
                    namesList = JsonUtility.FromJson<NamesList>(textAsset.text);
                }
                return namesList;
            }
        }

        public static string GetRandomName()
        {
            return CurrentNamesList.Names[
                UnityEngine.Random.Range(0, CurrentNamesList.Names.Count)
            ];
        }

        public static List<string> GetRandomNames(int nbNames)
        {
            if (nbNames > CurrentNamesList.Names.Count)
            {
                throw new Exception("Asking for more random names than there actually are!");
            }

            NamesList copy = new() { Names = new List<string>(CurrentNamesList.Names) };

            List<string> result = new();

            for (int i = 0; i < nbNames; i++)
            {
                int rnd = UnityEngine.Random.Range(0, copy.Names.Count);
                result.Add(copy.Names[rnd]);
                copy.Names.RemoveAt(rnd);
            }

            return result;
        }
    }
}
