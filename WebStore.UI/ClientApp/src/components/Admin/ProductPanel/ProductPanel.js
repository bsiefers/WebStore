import React from 'react';
import axios from 'axios';
import Cookies from '../../../lib/Cookies';
export default class ProductPanel extends React.Component {

    constructor(props) {
        super(props);
        
        this.state = { 
            editing: false,
            produceIndex: 0,
            productModel: {
                id: 0,
                name: "",
                description: "",
                productImage: null,
                productFile: null
            },
            loading: false,
            products: []
        };

        this.onChangeProduct = this.onChangeProduct.bind(this);

        this.getProducts = this.getProducts.bind(this);
        this.getProduct = this.getProduct.bind(this);

        this.editProduct = this.editProduct.bind(this);
        this.updateProduct = this.updateProduct.bind(this);
        
        this.createProduct = this.createProduct.bind(this);
        this.deleteProduct = this.deleteProduct.bind(this);
    }

    componentDidMount() {
        this.getProducts();
    }

    loadProductImage(event) {
        var product = this.state.productModel;        
        product.productImage = event.target.result;
        this.setState({productModel: product});
    }

    onChangeProduct(event) {
        var product = this.state.productModel;        
        var reader = new FileReader();
        reader.onload = this.loadProductImage.bind(this);
        if (event.target.name === "productImage") {
            product.productFile = event.target.files[0];
            reader.readAsDataURL(event.target.files[0]);            
        }else{
            product[event.target.name] = event.target.value;
        }
        this.setState({ productModel: product });
    }

    getProducts() {
        this.setState({ loading: true });
        
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/products',
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }})
            .then(res => {
                this.setState({ products: res.data });
            })
            .catch(err => {
                console.log(err);
            })
            .then(() => {
                this.loading = false;
            });
    }

    getProduct(id) {
        this.loading = true;
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/products/' + id,
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }})
            .then(res => {
                this.setState({ products: res.data });
            })
            .catch(err => {
                console.log(err);
            })
            .then(() => {
                this.loading = false;
            });
    }

    editProduct(event) {
        var id = event.target.name;
        this.setState({ editing: true });
        var product = this.state.products[id];

        this.setState({
            productIndex: id,
            productModel: {
                id: product.id,
                name: product.name,
                description: product.description,
                productImage: product.productImage
            }
        });
    }

    updateProduct(event) {

        var data = new FormData();
        data.append("id", this.state.productModel.id);
        data.append("name", this.state.productModel.name);
        data.append("description", this.state.productModel.description);
        data.append("productImage", this.state.productModel.productFile, this.state.productModel.productFile.fileName);
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/products',
            data: data,
            method: "put",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`,
                'content-type': 'multipart/form-data'
            }})
            .then(res => {
                var products = this.state.products;
                products.splice(this.state.productIndex, 1, res.data);
                this.setState({ products: products });
            }).catch(error => {
                console.log(error);
            }).then(() => {
                this.setState({ loading: false, editing: false});
            });
    }

    createProduct(event) {
        this.setState({ loading: true });
        var data = new FormData();
        data.append("id", this.state.productModel.id);
        data.append("name", this.state.productModel.name);
        data.append("description", this.state.productModel.description);
        data.append("productImage", this.state.productModel.productFile, this.state.productModel.productFile.fileName);
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/products',
            data: data,
            method: "post",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`,
                'content-type': 'multipart/form-data'
            }})
            .then(res => {
                var products = this.state.products;
                products.push(res.data);
                this.setState({ products: products });
            }).catch(error => {
                console.log(error);
            }).then(() => {
                this.setState({ loading: false, editing: false });
            });
    }

    deleteProduct(event) {
        this.setState({ loading: true });

        var id = event.target.name;
        var product = this.state.products[id];
        
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/products/' + product.id,
            method: "delete",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }})
            .then(res => {
                var products = this.state.products;
                products.splice(id, 1);
                this.setState({ products: products});
            })
            .catch(err => {
                console.log(err);
            })
            .then(() => {
                this.setState({ loading: false });
            });
    }

    render() {
        if (this.state.editing) {

            return (
                <div>
                    <div className="tabs">
                        <ul>
                            <li onClick={() => this.setState({ editing: false })} className={!this.state.editing ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>Edit Products</a></li>
                            <li onClick={() => this.setState({ editing: true })} className={this.state.editing ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>New Product</a></li>
                        </ul>
                    </div>
                    <div className="field">
                        <label className="label">Name</label>
                        <div className="control">
                            <input className="input" onChange={this.onChangeProduct} value={this.state.productModel.name} name="name" required/>
                        </div>
                    </div>
                    <div className="field">
                        <label className="label">Description</label>
                        <div className="control">
                            <textarea className="textarea" onChange={this.onChangeProduct} value={this.state.productModel.description} name="description" rows="5" required/>
                        </div>
                    </div>
                    <div className="field">
                        <label className="label">Default Image</label>
                        <img style={{ "maxHeight": "200px" }}
                            src={ this.state.productModel.productImage ?
                            this.state.productModel.productImage :
                            process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg"}
                            alt={this.state.productModel.name} />
                        <div className="control">
                            <div className="file">
                                <label className="file-label">
                                    <input
                                        className="file-input"
                                        type="file" name="productImage"
                                        onChange={this.onChangeProduct}
                                        />
                                    <span className="file-cta">
                                        <span className="file-icon">
                                            <i className="fas fa-upload"></i>
                                        </span>
                                        <span className="file-label">
                                            Choose a file…
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                    </div>
                    {!this.state.productModel.id ?
                        <button className="button is-success mr-3" onClick={this.createProduct}>Create</button> :
                        <button className="button is-warning mr-3" onClick={this.updateProduct}>Update</button>
                    }
                    <button className="button" onClick={() => this.setState({ editing: false })}>Cancel</button>
                </div>
            );

        } else {

            return (
                <div>
                    <div className="tabs">
                        <ul>
                            <li onClick={() => this.setState({ editing: false })} className={!this.state.editing ? "is-active" : ""}>
                                <a href="# " onClick={e => e.preventDefault()}>Edit Products</a>
                            </li>
                            <li onClick={() => this.setState({ editing: true })} className={this.state.editing ? "is-active" : ""}>
                                <a href="# " onClick={e => e.preventDefault()}>New Product</a>
                            </li>
                        </ul>
                    </div>
                    <table className="table" width="100%">
                        <thead className="has-text-center">
                            <tr>
                                <td>Id</td>
                                <td>Product</td>
                                <td>Description</td>                                
                                <td>Default Image</td>
                                <td></td>
                                <td></td>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.products.map((product, i) => {

                                return (
                                    <tr key={i}>
                                        <td>{product.id}</td>
                                        <td>{product.name}</td>
                                        <td>{product.description}</td>                                        
                                        <td><img style={{"height": "50px","width": "50px", "display": "block", "objectFit": "cover"}} src={product.productImage ?
                                            product.productImage :
                                            process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg" }
                                            alt={product.name} /></td>                                        
                                        <td><button className="button is-warning" name={i} onClick={this.editProduct}>Edit </button></td>
                                        <td><button className="button is-danger" name={i} onClick={this.deleteProduct}> Remove</button></td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>                    
                </div>    
            );
        }

    }
}

