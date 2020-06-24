using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_hurtownia.Models {
    public class PriceCalculatorReturn {
        public decimal NewBasePrice { get; set; }
        public decimal DiscountValue { get; set; }
    }

    public class PriceCalculator {
        private static EhurtowniaContext databaseContext = new EhurtowniaContext();

        private static Dictionary<int, decimal> discountValues = new Dictionary<int, decimal> {
            [100] = 5M,
            [1000] = 10M,
            [5000] = 20M
        }; // Left side in [] braces is minimal amount for discount, right side is discount value in percents [%]

        public static PriceCalculatorReturn CalculatePrice(int productID, int amount) {
            Products selectedProduct = databaseContext.Products.Where(product => product.IdProduct == productID).SingleOrDefault();

            if (selectedProduct == default(Products)) {
                return null;
            } else {
                decimal basePrice = selectedProduct.BasePricePerUnit;
                decimal newBasePrice = basePrice;
                decimal discount = 0;

                foreach (KeyValuePair<int, decimal> discountValue in discountValues.OrderBy(value => value.Key)) {
                    if (amount > discountValue.Key) {
                        discount = discountValue.Value;
                        newBasePrice = basePrice - (basePrice * (discount / 100M));
                    }
                }

                return new PriceCalculatorReturn() {
                    NewBasePrice = newBasePrice,
                    DiscountValue = discount
                };
            }
        }
    }
}
