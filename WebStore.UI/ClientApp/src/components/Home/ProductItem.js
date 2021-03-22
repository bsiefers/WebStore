import React, { Component } from 'react';
export default class Home extends Component {
    constructor(props) {
        super(props);
        this.state = {
            product: props.product,
            loading: true
        }
    }

    componentDidMount() {

    }

    render() {
        return (
            <div className="card" >
                <a href={"product?name=" + this.state.product.name}>
                    <div className="card-image">
                        <figure className="image" >
                            <img style={{ "objectFit": "cover", "height": "250px" }} src={
                                this.state.product.productImage ? 
                                this.state.product.productImage :
                                process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg"
                            } alt={this.state.product.name} />
                        </figure>
                    </div>
                    <div className="card-content">
                        <p className="title is-size-5">
                            {this.state.product.name}  {this.state.product.price}
                        </p>
                        <div className="content">

                        </div>
                    </div>
                </a>
            </div>
        );
    }
}
