import React, { Component } from 'react';
import Cookies from './../../lib/Cookies';
import jwt_decode from 'jwt-decode';
import Cart from './../Checkout/Cart';
export default class HomeLayout extends Component {
    constructor(props) {
        super(props);
        var token = Cookies.getCookie("JWT_Token");
        var decodedToken;
        if (token)
            decodedToken = jwt_decode(token);
        this.state = {
            loggedIn: token != null,
            userType: decodedToken ? decodedToken.role : null,
            cartShown: false
        }

    }

    componentDidMount() {
        const $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);

        // Check if there are any navbar burgers
        if ($navbarBurgers.length > 0) {

            // Add a click event on each of them
            $navbarBurgers.forEach(el => {
                el.addEventListener('click', () => {

                    // Get the target from the "data-target" attribute
                    const target = el.dataset.target;
                    const $target = document.getElementById(target);

                    // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
                    el.classList.toggle('is-active');
                    $target.classList.toggle('is-active');

                });
            });
        }
    }
    toggleCart(event) {
        this.setState({ cartShown: !this.state.cartShown });
    }

    render() {
        return (       
            <div>
                <div className={this.state.cartShown ? "modal is-active" : "modal"}>
                    <div className="modal-background"></div>
                    <div className="modal-card">
                        <header className="modal-card-head">
                            <p className="modal-card-title">Cart</p>
                            <button className="delete" onClick={this.toggleCart.bind(this)} aria-label="close"></button>
                        </header>
                        <section className="modal-card-body">
                            <Cart />
                        </section>    
                        <footer className="modal-card-foot">
                            <div className="has-text-right"><a className="button is-success" href="customer">Checkout</a></div>                          
                        </footer>
                    </div>
                </div>
                <nav className="navbar" role="navigation" aria-label="main navigation">
                    <div className="navbar-brand">
                        <a className="navbar-item" href="https://bulma.io">
                            <img src="https://bulma.io/images/bulma-logo.png" alt="logo" width="112" height="28" />
                        </a>
                    </div>
                    <div id="navbarBasicExample" className="navbar-menu">
                        <div className="navbar-start">
                            <a className="navbar-item" href="/">
                                Home
                            </a>
                            { this.state.userType === "Admin" &&
                                <a className="navbar-item" href="admin">
                                    Admin
                                </a>
                            }
                        </div>
                        <div className="navbar-end">
                            <div className="navbar-item is-clickable" onClick={this.toggleCart.bind(this)}>
                                Cart
                            </div>
                            <div className="navbar-item">
                                { !this.state.loggedIn ?
                                    (<div className="buttons">
                                        <a className="button is-primary" href="signup">
                                            <strong>Sign up</strong>
                                        </a>
                                        <a className="button is-light" href="login">
                                            Log in
                                        </a>
                                    </div>) :
                                    (<div className="buttons">
                                        <button className="button is-primary" onClick={() => { Cookies.eraseCookie("JWT_Token"); window.location.href = ""; }}>
                                            <strong>Log Out</strong>
                                        </button>
                                    </div>)
                                }
                            </div>
                        </div>
                    </div>
                </nav>
                <div className="">
                    {this.props.children}
                </div>
            </div>
        );
    }
}
