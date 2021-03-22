import React, { Component } from 'react';
import Layout from './../Layout/Layout';
import axios from 'axios';
import ProductItem from './ProductItem';
export default class Home extends Component {
    constructor(props) {
        super(props);

        this.state = {
            products: [],
            loading: false
        }
        
    }

    getProducts() {
        this.setState({ loading: true });
        var url = process.env.REACT_APP_API_HOST + '/api/product/products';
        console.log(url);
        axios({url: url, method: 'get' })
            .then(res => {
                console.log(res);
                this.setState({ products: res.data });
            })
            .catch(err => {
                console.log(err);
            })
            .then(() => {
                this.loading = false;
            });
    }

    componentDidMount() {
        this.getProducts();
    }

    render() {
        return (
            <Layout>
                <div className="section">
                    <div className="columns is-multiline is-mobile">
                    
                        {this.state.products.map((product, i) => {
                            return (
                                <div key={i} className="column is-3-desktop is-4-tablet is-6-mobile">
                                    <ProductItem product={product} />
                                </div>
                            );
                        })}
                    </div>
                    </div>
            </Layout>
        );
    }
}
