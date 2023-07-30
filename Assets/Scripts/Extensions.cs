using UnityEngine;

namespace Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the children of a game object in order by number (<c>#</c>). 
        /// The format is <c>{name}.{#}</c>
        /// </summary>
        /// <returns>An array containing the children in order.</returns>
        public static T[] GetChildrenByOrder<T>(this Transform transform)
        {
            int childCount = transform.childCount;

            T[] children = new T[childCount];

            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                string childName = child.name;

                // The index position of '.' in the game object's name
                int dotIndex = childName.IndexOf('.');
                // Throw an error if '.' could not be found or if there is nothing after it
                if (dotIndex < 0 || dotIndex == childName.Length - 1)
                {
                    Debug.LogError("The child's name does not follow the specified format.", child);
                    return null;
                }

                // The substring after '.'
                string substr = childName.Substring(dotIndex + 1);
                int number = int.Parse(substr);

                children[number - 1] = child.GetComponent<T>();
                // Throw an error if the component could not be found
                if (children[number - 1] == null)
                {
                    Debug.LogError($"Failed to get the component `{nameof(T)}` from the child.", child);
                    return null;
                }
            }

            return children;
        }

        /// <summary>
        /// Replaces a character at index <c>at</c> in a string by <c>character</c>.
        /// </summary>
        /// <param name="character">The character to replace with.</param>
        /// <param name="at">The index of the character to replace.</param>
        /// <returns>The result of the replacement.</returns>
        public static string Replace(this string str, char character, int at)
        {
            char[] chars = str.ToCharArray();
            chars[at] = character;

            return new string(chars);
        }

        /// <summary>
        /// Searches for a string (array of <c>char</c>) in an array.
        /// </summary>
        /// <param name="value">The string to search for.</param>
        /// <returns>The index first occurence of the string <c>value</c> relative to the array. 
        /// If the string couldn't be found, it returns <c>-1</c>.</returns>
        public static int Find(this string[] array, char[] value)
        {
            int length = value.Length;

            int i = 0;
            foreach (string item in array)
            {
                int j = 0;
                if (item.Length - 1 > length) // -1 to exclude '\n' or '\r'
                    continue;
                for (; j < length; j++)
                {
                    if (item[j] != value[j])
                        break;
                }
                if (j == length)
                    return i;
                i++;
            }

            return -1;
        }
    }
}
