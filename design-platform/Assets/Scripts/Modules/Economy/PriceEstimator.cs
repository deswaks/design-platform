using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPlatform.Core;

namespace MyModule {
    public static class PriceEstimator {

        public static float DkkPrM2 = 17000.0f;
        public static float VariationOfPrice = 0.2f;

        public static string GetPriceString() {
            float area = GetBuildingArea();


            float minPrice = area * (1.0f - VariationOfPrice) * DkkPrM2;
            float maxPrice = area * (1.0f + VariationOfPrice) * DkkPrM2;

            // Format with thousands separator
            string minPriceString = string.Format("{0:0,0}", minPrice/1000) + ",000";
            string maxPriceString = string.Format("{0:0,0}", maxPrice/1000) + ",000";

            string priceString = minPriceString + "-" + maxPriceString + " DKK";
            return priceString;
        }

        private static float GetBuildingArea() {
            float area = 0.0f;
            foreach (Room room in Building.Instance.Rooms) {
                area += room.GetFloorArea();
            }
            return area;
        }


    }
}
