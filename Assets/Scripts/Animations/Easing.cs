using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Easing
    {
        
        // Easing types
        public enum EasingType {
            Exponential, 
            Elastic,
            Quadratic,
            Linear,
            Back,
            Bounce,
            Pointer
        };

        // Main function
        public static float EaseOut(float x, EasingType easingType){

            switch (easingType){
                case EasingType.Exponential:
                    return EaseOutExponential(x);
                case EasingType.Elastic:
                    return EaseOutElastic(x);
                case EasingType.Quadratic:
                    return EaseOutQuadratic(x);
                case EasingType.Linear:
                    return x;
                case EasingType.Back:
                    return EaseOutBack(x);
                case EasingType.Bounce:
                    return EaseOutBounce(x);
            }

            return x;
        }

        // Main function
        public static float EaseIn(float x, EasingType easingType){

            switch (easingType){
                case EasingType.Exponential:
                    return EaseInExponential(x);
                case EasingType.Elastic:
                    return EaseInElastic(x);
                case EasingType.Quadratic:
                    return EaseInQuadratic(x);
                case EasingType.Linear:
                    return x;
                case EasingType.Back:
                    return EaseInBack(x);
                case EasingType.Bounce:
                    return EaseInBounce(x);
            }

            return x;
        }

        public static float EaseFull(float x, EasingType easingType){

            switch (easingType){
                case EasingType.Pointer:
                    return EasePointer(x);
            }

            return (x);
        }

        // Exponential easing out
        private static float EaseOutExponential(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 1-2^(-10x)
            return 1f-Mathf.Pow(2,-10*x);

        }

        private static float EaseInExponential(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 2^(10x-10)
            return Mathf.Pow(2f, 10 * x - 10f);

        }

        // Elastic easing out
        private static float EaseOutElastic(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 2pi/3
            float c4 = (2 * Mathf.PI) / 3;

            // 2^(-10x) * sin(20pi*x/3 - pi/2) + 1
            return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1;

        }

        // Elastic easing in
        private static float EaseInElastic(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 2pi/3
            float c4 = (2 * Mathf.PI) / 3;

            return -1 * Mathf.Pow(2f, 10 * x - 10f) * Mathf.Sin((x*10-10.75f) * c4);

        }

        // Quadratic easing out
        private static float EaseOutQuadratic(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 1 - (x-1)^2
            return 1 - (x-1) * (x-1);

        }

        // Quadratic easing in
        private static float EaseInQuadratic(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            return x * x;
        }

        // Back easing out
        private static float EaseOutBack(float x){
            
            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            float c1 = 1.70158f;
            float c3 = 1 + c1;

            return 1 + c3 * (x-1)*(x-1)*(x-1) + c1 * (x-1)*(x-1);
        }

        // Back easing in
        private static float EaseInBack(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            float c1 = 1.70158f;
            float c3 = 1 + c1;

            return c3 * x*x*x - c1 * x*x;
        }

        // Bounce easing out
        private static float EaseOutBounce(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1f/d1){
                return n1 *x*x;
            } else if (x < 2f/d1){
                return n1 * (x -= 1.5f/d1) * x + 0.75f;
            } else if (x < 2.5f/d1){
                return n1 * (x -= 2.25f/d1) * x + 0.9375f;
            } else {
                return n1 * (x -= 2.625f/d1) * x + 0.984375f;
            }
        }

        // Bounce easing in
        private static float EaseInBounce(float x){
            return 1 - EaseOutBounce(1-x);
        }

        private static float EasePointer(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.995) return 1f;
            if (x < 0) return 0f;


            return 6.26f * (1f-Mathf.Cos(2*Mathf.PI*x)) * (x-1f)*(x-1f)*(x-1f)*(x-1f);
        }
    }
}
