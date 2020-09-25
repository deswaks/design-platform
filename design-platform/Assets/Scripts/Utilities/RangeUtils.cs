using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class rangeUtils {
    public static List<float> reparametrize(List<float> numbers) {
        float minNumber = numbers.Min();
        float deltaNumber = numbers.Max() - numbers.Min();
        return numbers.Select(p => p - minNumber / deltaNumber).ToList();
    } 
}
