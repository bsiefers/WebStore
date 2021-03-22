import React, { Component } from 'react';
import { Elements } from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import Layout from './../Layout/Layout';
import CheckoutForm from './CheckoutForm';
import Cart from './Cart';
import Customer from './../Customer/Customer';

const stripePromise = loadStripe("pk_test_51I6WqQLjUeLWWr4aH83Xz2yaWilQ6Yko5Wh2vCbtUidr6ygwnWr61qJLjD1cKt1IDVium6ofQYywsDXCVEW2gKNI00Bj97IAno");

export default class Checkout extends Component {

    constructor(props) {
        super(props);
        var customer = JSON.parse(window.localStorage.getItem('customer_info'));
        var cart = JSON.parse(window.localStorage.getItem('cart'));
        if (!customer) window.location.href = 'customer';
        if (!cart) window.location.href = '';
        this.state = {
            customer: customer,
            cart: cart
        }
    }

    render() {
        return (
            <Layout>
                <div className="container">
                    <Elements stripe={stripePromise}>
                        <div className="columns">
                            <div className="column">
                                <Customer customer={this.state.customer} />
                            </div>
                            <div className="column">
                                <Cart cart={this.state.cart} size="text" /><br />
                                <CheckoutForm cart={this.state.cart} customer={this.state.customer} />
                            </div>
                        </div>
                        
                    </Elements>
                </div>
            </Layout>
        );
    }   
}