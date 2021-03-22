import React, { Component } from 'react';
import Cart from './../../lib/Cart';
import axios from 'axios';
import Layout from './../Layout/Layout';

export default class Product extends Component {
    constructor(props) {
        super(props)
        this.state = {
            loading: false,
            product: props.product,
            selectedItem: 0,
            amount: 1,
            modal: {
                success: false,
                shown: false
            }
        }
        this.getProduct = this.getProduct.bind(this);
        this.toggleModal = this.toggleModal.bind(this).bind(this);
        this.cart = Cart;
    }

    getProduct(name) {
        this.setState({loading: true});
        axios.get(process.env.REACT_APP_API_HOST + '/api/product/products/' + name)
            .then(res => {
                console.log(res);
                this.setState({
                    product: res.data,
                    selectedItem: 0
                });
            }).catch(err => {
                console.log(err)
            }).then(res => {
                this.setState({ loading: false });
            });
    }

    componentDidMount() {
        
        if (!this.state.product) {
            var productName = new URLSearchParams(this.props.location.search).get("name")
            console.log(productName);
            this.getProduct(productName);
        }
        
    }

    onChange(event) {
        console.log(this.state.amount);
        this.setState({[event.target.name]: event.target.value});
    }

    onSubmit(event) {
        event.preventDefault();
        console.log(this.state.amount);
        if (this.state.amount > 0) {
            this.cart.adjustItem(
                this.state.product.inventory[this.state.selectedItem].id,
                this.state.amount,
                this.state.product.name,
                this.state.product.price.substring(1),
                this.state.product.inventory[this.state.selectedItem].description
            );
            this.toggleModal(true);
        } else {
            this.toggleModal(false);
        }        
    }

    toggleModal(success) {        
        this.setState({ modal: { success, shown: !this.state.modal.shown } });
    }

    render() {
        
        return (
            <div>
                <div className={this.state.modal.shown ? "modal is-active" : "modal" }>
                    <div className="modal-background"></div>
                    <div className="modal-content">
                        <article className={this.state.modal.success ? "message is-success" : "message is-danger" }>
                            <div className="message-header">
                                <p>{this.state.modal.success ? "Success" : "No Items added!"}</p>
                                <button onClick={this.toggleModal} className="delete" aria-label="delete"></button>
                            </div>
                            <div className="message-body">
                                {this.state.modal.success ? "Items added to your cart!" : "No Items were added to your cart!"}
                               
                            </div>
                        </article>
                    </div>
                    
                </div>
                { this.state.product &&
                <Layout>
                    <div className="hero">
                        <div className="hero-body">
                            <div className="">
                                <div className="columns">
                                    <div className="column is-6">
                                        <figure className="is-pulled-right pr-6">
                                            <img style={{ "maxHeight": "80vh", "maxWidth": "30vw", "margin": "auto", "objectFit": "cover" }}
                                                src={this.state.product.productImage ?
                                                this.state.product.productImage :
                                                process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg"} alt="Product" />
                                        </figure>
                                    </div>
                                    <div className="column is-6">
                                        <section>
                                            <header>
                                                <p className="title">{this.state.product.name}</p>
                                                {(this.state.product.inventory[this.state.selectedItem].stock <= 10 &&
                                                  this.state.product.inventory[this.state.selectedItem].stock !== 0 ) &&
                                                    <p className="has-text-weight-semibold has-text-warning">Only {this.state.product.inventory[this.state.selectedItem].stock} remain!</p>}
                                                {this.state.product.inventory[this.state.selectedItem].stock === 0 &&
                                                    <p className="has-text-weight-semibold has-text-danger">Out Of Stock!</p>}
                                                <p>{this.state.product.description}</p>
                                            </header>
                                            <main>
                                            </main>
                                            <footer>
                                                <form onSubmit={this.onSubmit.bind(this)}>
                                                    <div className="field is-horizontal">
                                                        <div className="field-label">
                                                            <label className="label">Inventory</label>
                                                        </div>
                                                        <div className="field-body">
                                                            <div className="field is-narrow">
                                                                <div className="control">
                                                                    <div className="select">
                                                                        <select name="selectedItem" onChange={this.onChange.bind(this)}>
                                                                            {this.state.product.inventory.map((inventory, x) => {
                                                                                return (
                                                                                    <option key={x} value={x}>{inventory.description}</option>
                                                                                );
                                                                            })}
                                                                        </select>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>                                                    
                                                    <div className="field is-horizontal">
                                                        <div className="field-label is-normal">
                                                            <label className="label">Quantity</label>
                                                        </div>
                                                        
                                                        <div className="field-body">
                                                            <div className="field is-narrow">
                                                                <div className="control">
                                                                    <input
                                                                        className="input"
                                                                        type="number"
                                                                        name="amount"
                                                                        onChange={this.onChange.bind(this)}
                                                                        value={this.state.amount}
                                                                        disabled={this.state.product.inventory[this.state.selectedItem].stock === 0}
                                                                    ></input>                                                        
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div className="field is-horizontal">
                                                        <div className="field-label is-normal">
                                                            
                                                        </div>
                                                        <div className="field-body">
                                                            <div className="field is-narrow">
                                                                <div className="control">
                                                                    <button
                                                                        className="button is-success"
                                                                        type="submit"
                                                                        disabled={this.state.product.inventory[this.state.selectedItem].stock === 0}
                                                                    >Add To Cart</button>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </form>
                                            </footer>
                                        </section>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </Layout>
                }
            </div>
        );
    }
}
