using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class rangeUtils {

    /// <summary>
    /// Reparametrizes a list of numbers on the interval between startValue and endValue.
    /// Values above the endValue will be above 1.0
    /// Values below the startValue will be below 0.0
    /// </summary>
    /// <param name="numbers">List of numbers to reparametrize</param>
    /// <returns></returns>
    public static List<float> reparametrize(List<float> numbers) {
        float startValue = numbers.Min();
        float endValue = numbers.Min();
        List<float> reparametrized = numbers.Select(p => (p - startValue) / (endValue - startValue)).ToList();
        return reparametrized;
    }
    /// <summary>
    /// Reparametrizes a list of numbers on the interval between startValue and endValue.
    /// Values above the endValue will be above 1.0
    /// Values below the startValue will be below 0.0
    /// </summary>
    /// <param name="numbers">List of numbers to reparametrize</param>
    /// <param name="startValue">Start value (reparametrized to 0.0)</param>
    /// <param name="endValue">End value (reparametrized to 1.0)</param>
    /// <returns></returns>
    public static List<float> reparametrize(List<float> numbers, float startValue, float endValue) {
        List<float> reparametrized = numbers.Select(p => (p - startValue) / (endValue - startValue)).ToList();
        return reparametrized;
    }
}
