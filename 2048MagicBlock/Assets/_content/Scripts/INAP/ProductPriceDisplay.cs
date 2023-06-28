using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ProductPriceDisplay : MonoBehaviour, IStoreListener
{
    public Text specialOfferPriceTextPopUp;
    public Text specialOfferPriceText;
    public Text masterBundlePriceText;
    public Text superBundlePriceText;

    public Text coins1500PriceText;
    public Text coins700PriceText;
    public Text coins300PriceText;
    public Text coins100PriceText;

    public Text weeklySubscriptionPriceText;
    public Text monthlySubscriptionPriceText;
    public Text yearlySubscriptionPriceText;

    public Text removeAdsPriceText;

    private IStoreController controller;
    private IExtensionProvider extensions;

    public INAPcontroller inapController;

    private int timeSec = 0;


    private void Start()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct("com.oriplay.project2048.specialoffer", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.masterbundle", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.superbundle", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.1500coins", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.700coins", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.300coins", ProductType.Consumable);
        builder.AddProduct("com.oriplay.project2048.100coins", ProductType.Consumable);

        builder.AddProduct("vip_weekly", ProductType.Subscription);
        builder.AddProduct("vip_monthly", ProductType.Subscription);
        builder.AddProduct("vip_yearly", ProductType.Subscription);

        builder.AddProduct("com.oriplay.project2048.removeads", ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);

        StartCoroutine(TimerForReplacePrice());
    }

    public virtual void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    public virtual void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("Initialization Failed: " + error);
    }

    public virtual void OnPurchaseFailed(Product product, PurchaseFailureReason error)
    {
        Debug.LogError("Purchase Failed: " + error);
    }

    public virtual PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //removeAdsPriceText.text = args.purchasedProduct.metadata.localizedPrice.ToString();
        inapController.OnPurchaseCompleted1(args.purchasedProduct);
        return PurchaseProcessingResult.Complete;
    }

    private void Update()
    {
        if (controller != null && controller.products != null && timeSec < 10)
        {
            var product = controller.products.WithID("com.oriplay.project2048.specialoffer");
            if (product != null)
                specialOfferPriceText.text = product.metadata.localizedPrice.ToString() + "$";
            //
            product = controller.products.WithID("com.oriplay.project2048.specialoffer");
            if (product != null)
                specialOfferPriceTextPopUp.text = product.metadata.localizedPrice.ToString()+"$";
            //
            product = controller.products.WithID("com.oriplay.project2048.masterbundle");
            if (product != null)
                masterBundlePriceText.text = product.metadata.localizedPrice.ToString() + "$";
            //
            product = controller.products.WithID("com.oriplay.project2048.superbundle");
            if (product != null)
                superBundlePriceText.text = product.metadata.localizedPrice.ToString() + "$";
            /////////////////////////
            product = controller.products.WithID("com.oriplay.project2048.1500coins");
            if (product != null)
                coins1500PriceText.text = product.metadata.localizedPrice.ToString() + "$";

            product = controller.products.WithID("com.oriplay.project2048.700coins");
            if (product != null)
                coins700PriceText.text = product.metadata.localizedPrice.ToString() + "$";

            product = controller.products.WithID("com.oriplay.project2048.300coins");
            if (product != null)
                coins300PriceText.text = product.metadata.localizedPrice.ToString() + "$";

            product = controller.products.WithID("com.oriplay.project2048.100coins");
            if (product != null)
                coins100PriceText.text = product.metadata.localizedPrice.ToString() + "$";
            //////////////////////////
            product = controller.products.WithID("com.oriplay.project2048.removeads");
            if (product != null)
                removeAdsPriceText.text = product.metadata.localizedPrice.ToString() + "$";
            ////////////////////////////
            product = controller.products.WithID("vip_weekly");
            if (product != null)
                weeklySubscriptionPriceText.text = product.metadata.localizedPrice.ToString() + "$";

            product = controller.products.WithID("vip_monthly");
            if (product != null)
                monthlySubscriptionPriceText.text = product.metadata.localizedPrice.ToString() + "$";

            product = controller.products.WithID("vip_yearly");
            if (product != null)
                yearlySubscriptionPriceText.text = product.metadata.localizedPrice.ToString() + "$";
            ////////////////////////////////
        }
        else if(timeSec == 11)
        {
            timeSec++;
            StopCoroutine(TimerForReplacePrice());
        }
    }

    private System.Collections.IEnumerator TimerForReplacePrice()
    {
        timeSec++;
        yield return new WaitForSeconds(1);
    }
}
