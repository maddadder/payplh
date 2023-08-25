async function initializeStripe(publishableKey) {
    var stripe = Stripe(publishableKey); // Replace with your actual publishable key
    var elements = stripe.elements();

    // Create and mount the card element
    var card = elements.create('card');
    card.mount('#cardElement'); // Make sure you have an element with the ID "cardElement"

    // Wait for the card to be mounted before creating the token
    await new Promise((resolve, reject) => {
        card.on('ready', () => {
            resolve();
        });
    });

    // Create a token using the mounted card element
    async function createStripeToken() {
        var result = await stripe.createToken(card);

        if (result.error) {
            // Handle error
            return null;
        } else {
            return result.token.id;
        }
    }

    // Expose the createStripeToken function to Blazor
    window.createStripeToken = createStripeToken;
}