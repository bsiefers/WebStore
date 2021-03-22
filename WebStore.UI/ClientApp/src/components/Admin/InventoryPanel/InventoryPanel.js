import React from 'react';
import axios from 'axios';
import Cookies from '../../../lib/Cookies';
export default class InventoryPanel extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            products: [],
            loading: false,
            inventoryModel: {
                id: null,
                description: "",
                quantity: 0,
                price: 0,
                inventoryImage: null,
                inventoryFile: null
            },
            selectedProduct: null,
            creating: false,
            editing: true
        }
        

    }

    componentDidMount() {
        this.getInventory();
    }

    onChangeSelectedProduct(event) {
        var product = this.state.selectedProduct;
        var index = event.target.name.split('_')[0];
        var field = event.target.name.split('_')[1];
        product.inventory[index][field] = event.target.value;                      
        this.setState({ selectedProduct: product});
    }

    loadInventoryImage(event) {
        var inventoryModel = this.state.inventoryModel;
        inventoryModel.inventoryImage = event.target.result;
        
        this.setState({inventoryModel: inventoryModel});
    }

    onChangeInventoryModel(event) {
        var inventoryModel = this.state.inventoryModel
        var reader = new FileReader();
        reader.onload = this.loadInventoryImage.bind(this);
        if (event.target.name === "inventoryImage") {
            inventoryModel.inventoryFile = event.target.files[0];
            reader.readAsDataURL(event.target.files[0]);  
        } else {
            inventoryModel[event.target.name] = event.target.value
        }
        
        this.setState({inventoryModel: inventoryModel});
    }

    getInventory() {
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/inventory',
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
        .then(res => {
            this.setState({ products: res.data, selectedProduct: res.data ? res.data[0] : null });
            
        })
        .catch(err => {
            console.log(err);
        })
        .then(res => {
            this.setState({ loading: false });
        });
    }

    updateInventory(event) {
        this.setState({ loading: true });
        var data = new FormData();
        data.append("id", this.state.inventoryModel.id);
        data.append("productId", this.state.selectedProduct.id);
        data.append("description", this.state.inventoryModel.description);
        data.append("quantity", this.state.inventoryModel.quantity);
        data.append("price", this.state.inventoryModel.price);
        if(this.state.inventoryModel.inventoryFile)
            data.append("inventoryImage", this.state.inventoryModel.inventoryFile, this.state.inventoryModel.inventoryFile.fileName);
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/inventory',
            data: data,
            method: "put",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
        .then(res => {                
            var selectedProduct = this.state.selectedProduct;
            selectedProduct.inventory = res.data.inventory;
            this.setState({selectedProduct: selectedProduct});
        })
        .catch(err => {
            console.log(err);
        })
        .then(res => {
            this.setState({ loading: false });
        });
    }

    deleteInventory(event) {
        var index = event.target.name;
        var id = this.state.selectedProduct.inventory[event.target.name].id;
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/inventory/' + id,
            method: "delete",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
        .then(res => {                
            var product = this.state.selectedProduct;
            product.inventory.splice(index, 1);
            this.setState({ selectedProduct: product });
        })
        .catch(err => {
            console.log(err);
        })
        .then(res => {
            this.setState({ loading: false });
        });
    }

    addInventory(event) {
        this.setState({ loading: true });
        var data = new FormData();
        data.append("productId", this.state.selectedProduct.id);
        data.append("description", this.state.inventoryModel.description);
        data.append("price", this.state.inventoryModel.price);
        data.append("quantity", this.state.inventoryModel.quantity);
        if (this.state.inventoryModel.inventoryFile)
            data.append("inventoryImage", this.state.inventoryModel.inventoryFile, this.state.inventoryModel.inventoryFile.fileName);
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/inventory',
            data: data,
            method: "post",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
        .then(res => {                
            var product = this.state.selectedProduct;
            product.inventory.push(res.data);
            this.setState({ selectedProduct:  product});
        })
        .catch(err => {
            console.log(err);
        })
        .then(res => {
            this.setState({ loading: false });
        });
    }

    render() {
        
        return (
            <div className="columns">
                <div className="column is-9">
                    <div className="tabs">
                        <ul>
                            <li onClick={() => this.setState({ editing: true, creating: false })} className={this.state.editing ? "is-active" : ""}>
                                <a href="# " onClick={e => e.preventDefault()}>Edit Inventories</a>
                            </li>
                            <li onClick={() => this.setState({ editing: false, creating: true })} className={!this.state.editing ? "is-active" : ""}>
                                <a href="# " onClick={e => e.preventDefault()}>New Inventory</a>
                            </li>
                        </ul>
                    </div>
                    {this.state.selectedProduct &&
                        <div>
                            {this.state.editing ?
                                <table className="table" width="100%">
                                    <thead>
                                        <tr>
                                            <td>Id</td>
                                            <td>Description</td>
                                            <td>Quantity</td>
                                            <td>Price</td>
                                            <td>Image</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {this.state.selectedProduct.inventory.map((inventory, i) => {
                                            return (
                                                <tr key={i}>
                                                    <td>{inventory.id}</td>
                                                    <td>{inventory.description}</td>
                                                    <td>{inventory.quantity}</td>
                                                    <td>{inventory.price}</td>
                                                    <td>
                                                        <img 
                                                            src={inventory.inventoryImage ?
                                                                inventory.inventoryImage :
                                                                process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg"}
                                                            style={{"height": "50px", "width": "50px", "display": "block", "objectFit": "cover" }}
                                                            alt="inventory item" />
                                                    </td>
                                                        <td><button onClick={()=>this.setState({creating: false, editing: false, inventoryModel: inventory})} className="button is-warning">Edit</button></td>
                                                    <td><button name={i} onClick={this.deleteInventory.bind(this)} className="button is-danger">Remove</button></td>
                                                </tr>
                                            );
                                        })}
                                    </tbody>
                                </table> :
                                <form>
                                    <label className="label">Description</label>
                                    <input
                                        className="input"
                                        name="description"
                                        value={this.state.inventoryModel.description}
                                        onChange={this.onChangeInventoryModel.bind(this)}
                                        required
                                    />
                                    <label className="label" >Quantity</label>
                                    <input
                                        className="input"
                                        name="quantity"
                                        value={this.state.inventoryModel.quantity}
                                        onChange={this.onChangeInventoryModel.bind(this)}
                                        required
                                    />
                                    <label className="label" >Price</label>
                                    <input
                                        className="input" 
                                        name="price"
                                        value={this.state.inventoryModel.price}
                                        onChange={this.onChangeInventoryModel.bind(this)}
                                        required
                                    />
                                        <label className="label">Inventory Image</label>
                                    <img style={{ "maxHeight": "200px" }}
                                        src={this.state.inventoryModel.inventoryImage ?
                                             this.state.inventoryModel.inventoryImage :
                                             process.env.REACT_APP_API_HOST + "/images/product-image-placeholder.jpg"}
                                        alt="inventory" />
                                    <div className="file">
                                        <label className="file-label">
                                            <input
                                                className="file-input"
                                                type="file" name="inventoryImage"
                                                onChange={this.onChangeInventoryModel.bind(this)}
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
                                    <button
                                        type="button"
                                        onClick={this.state.creating ? this.addInventory.bind(this) : this.updateInventory.bind(this)}
                                        className="button is-success my-4">
                                        {this.state.creating ? "Create New Inventory" : "Edit Inventory"}
                                    </button>
                                </form>
                        }
                    </div>
                    }
                </div>

                <aside className="menu column is-3">
                    <ul className="menu-list">
                        <li>
                            <h3 className="menu-label">Inventory To Edit</h3>
                            <ul>
                                {this.state.products.map((product, i) => {                                    
                                    return (
                                        <li key={i}>
                                            <a
                                                href="# "
                                                onClick={(e) => {
                                                    e.preventDefault();
                                                    this.setState({
                                                        selectedProduct: product,
                                                        inventoryModel: {
                                                            id: null,
                                                            description: "",
                                                            quantity: 0,
                                                            inventoryImage: null,
                                                            inventoryFile: null
                                                        },
                                                        creating: false,
                                                        editing: true
                                                    });
                                                }}
                                                className={this.state.selectedProduct &&
                                                    this.state.selectedProduct.id === product.id ? "is-active" : ""
                                            }>
                                                {product.productName}
                                            </a>
                                        </li>
                                    );
                                })}
                            </ul>
                        </li>
                    </ul>
                </aside>
            </div>
        );
    }
}