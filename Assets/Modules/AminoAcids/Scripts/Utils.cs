using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids
{
    public static class Utils
    {
        public static float EaseOut(this float inT)
        {
            float t = Mathf.Clamp(inT, 0, 1) + 1;
            t = 2 * (1 / t) - 1;
            return 1 - t;
        }

        public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
        {
            result = null;

            if (component == null) return false;

            result = component.GetComponentInParent<T>();
            return true;
        }

        public static bool TryGetComponentInParent(this Component inComponent, Type type, out Component outComponent)
        {
            outComponent = null;
            if (inComponent == null) return false;

            Transform currentTransform = inComponent.transform;
            while (currentTransform != null)
            {
                Component possibleComponent = currentTransform.GetComponent(type);
                currentTransform = currentTransform.parent;

                if (possibleComponent == null) continue;

                outComponent = possibleComponent;
                return true;
            }

            return false;
        }

        public static Vector3 Midpoint(this Vector3 v)
        {
            return new Vector3(v.x / 2, v.y / 2, v.z / 2);
        }

        public static float AtomSize(this Element element)
        {
            return element switch
            {
                Element.H => Atom.SizeH,
                Element.C => Atom.SizeC,
                Element.N => Atom.SizeN,
                Element.O => Atom.SizeO,
                Element.S => Atom.SizeS,
                _ => 1f
            };
        }

        public static string GetTextForCurrentLanguage(this List<LanguageText> languageTexts)
        {
            return languageTexts.Find(text => text.language == Language.language).text ?? string.Empty;
        }

        public static List<T> SelectRandom<T>(this List<T> list, int count)
        {
            if (count <= 0 || count >= list.Count) return list;

            List<T> copy = new List<T>(list);
            List<T> result = new List<T>();

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, copy.Count);
                result.Add(copy[index]);
                copy.RemoveAt(index);
            }

            return result;
        }

        public static T WhereMin<T, C>(this IEnumerable<T> list, Func<T, C> predicate) where C : IComparable<C>, IEquatable<C>
        {
            if (list.Count() == 0) return default;

            // Alternative implementation (less lines but less efficient and I don't like the Equals())
            // C minValue = list.Min(predicate);
            // return list.FirstOrDefault(item => predicate(item).Equals(minValue));

            T result = list.First();
            C minValue = predicate(list.First());

            foreach (T item in list)
            {
                if (predicate(item).CompareTo(minValue) >= 0) continue;

                minValue = predicate(item);
                result = item;
            }

            return result;
        }

        public static T WhereMax<T, C>(this IEnumerable<T> list, Func<T, C> predicate) where C : IComparable<C>, IEquatable<C>
        {
            if (list.Count() == 0) return default;

            // Alternative implementation (less lines but less efficient and I don't like the Equals())
            // C maxValue = list.Max(predicate);
            // return list.FirstOrDefault(item => predicate(item).Equals(maxValue));

            T result = list.First();
            C maxValue = predicate(list.First());

            foreach (T item in list)
            {
                if (predicate(item).CompareTo(maxValue) <= 0) continue;

                maxValue = predicate(item);
                result = item;
            }

            return result;
        }

        public static IEnumerator AnimatePositionRotationScale(
            this Transform transform, Vector3? translation = null,
            Quaternion? rotation = null,
            Vector3? scale = null,
            float duration = 1f,
            EasingType easingType = EasingType.Quadratic)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + (translation ?? Vector3.zero);
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = (rotation ?? Quaternion.identity) * transform.rotation;
            Vector3 startScale = transform.lossyScale;
            Vector3 endScale = Vector3.Scale(transform.lossyScale, scale ?? Vector3.one);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Easing.EaseFull(elapsedTime / duration, easingType);

                transform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, t), Quaternion.Lerp(startRotation, endRotation, t));
                transform.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }
}
