import React from 'react';
import { useStripe, useElements, CardElement } from '@stripe/react-stripe-js';
import axios from 'axios';

const CARD_ELEMENT_OPTIONS = {
    style: {
        base: {
            color: "#32325d",
            fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
                color: "#aab7c4",
            },
        },
        invalid: {
            color: "#fa755a",
            iconColor: "#fa755a",
        },
    },
};

export default function CheckoutForm(props) {
    const stripe = useStripe();
    const elements = useElements();
    console.log(props.customer)
    const stripeTokenHandler = (token) => {       
        var cart = [];
        Object.keys(props.cart).forEach(cartItem => {
            cart.push({ id: cartItem, quantity: props.cart[cartItem].quantity });
        });
        axios.post(process.env.REACT_APP_API_HOST + '/api/order/orders/payment', { cart, token: token.id, customerinformation: props.customer })
            .then(res => {
                console.log(res);
            }).catch(err => {
                console.log(err);
            });
    }
    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!stripe || !elements) {
            return;
        }

        const card = elements.getElement(CardElement);
        stripe.createToken(card)
            .then(res => {
                if (res.error) {
                    console.log(res.error.message);
                } else {
                    stripeTokenHandler(res.token);
                }
            });
    };

    return (
        <form onSubmit={handleSubmit} style={{ "maxWidth": "500px" }}>
            <CardElement options={CARD_ELEMENT_OPTIONS} /><br />
            <div className="has-text-right"><button className="button is-success" disabled={!stripe}>Confirm order</button></div>
        </form>
    );
}