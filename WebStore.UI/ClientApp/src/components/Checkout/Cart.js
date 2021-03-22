import React, { Component } from 'react';
export default class Cart extends Component {
    constructor(props) {
        super(props);
        const sizes = { "text": "text", "small": "small", "medium": "medium", "large": "large" };
        this.sizes = Object.freeze(sizes);
        var cart = props.cart ? props.cart : JSON.parse(window.localStorage.getItem("cart"));
        if (cart == null)
            cart = {};
        this.state = {
            cart: cart,
            size: props.size ? props.size : this.sizes.text
        }
    }


    getTotal() {
        var total = 0;
        Object.keys(this.state.cart).forEach((cartItem) => {
            total += this.state.cart[cartItem].quantity * this.state.cart[cartItem].price;
        });
        return total;
    }

    getTextCart() {
        
            return (
                <div>
                    <h4 className="is-size-4 has-text-weight-semibold">Cart</h4>
                    <div className="pl-5">
                        {Object.keys(this.state.cart).map((cartItem, x) => {
                            return (
                                <div className="" key={x} style={{"maxWidth": "300px"}}>
                                    <p className="is-size-5 has-text-weight-medium">{this.state.cart[cartItem].name }</p>
                                    <div className="columns pl-5">
                                        <div className="column">
                                            <p className="is-size-6 has-text-weight-medium">Description</p>
                                            <p className="has-text-weight-light">{this.state.cart[cartItem].description}</p>
                                        </div>
                                        <div className="column">
                                            <p className="is-size-6 has-text-weight-medium">Quantity</p>
                                            <p className="has-text-weight-light">{this.state.cart[cartItem].quantity}</p>
                                        </div>
                                        <div className="column">
                                            <p className="is-size-6 has-text-weight-medium">Price</p>
                                            <p className="has-text-weight-light">${this.state.cart[cartItem].price}</p>
                                        </div>
                                    </div>
                                </div>
                                );
                        })}
                        <h5 className="is-size-5 has-text-weight-semibold">Total: {this.getTotal()}</h5>
                    </div>
                </div>
            );
    }

    getSmallmCart() {

    }

    getMediumCart() {

    }

    getLargeCart() {

    }

    render() {
        if (this.state.size === this.sizes.text) {
            return this.getTextCart();
        } else if (this.state.size === this.sizes.small) {
            return this.getSmallCart();
        } else if (this.state.size === this.sizes.medium) {
            return this.getMediumCart();
        } else {
            return this.getLargeCart();
        }
        
    }
}
