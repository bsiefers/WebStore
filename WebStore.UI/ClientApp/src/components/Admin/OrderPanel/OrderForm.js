import React from 'react';
import axios from 'axios';
import Cookies from './../../../lib/Cookies';


export default class OrderForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isUpdating: props.isUpdating,
            orderModel: props.order || {
                orderId: "",
                status: "ordered",
                firstName: "",
                lastName: "",
                email: "",
                phoneNumber: "",
                address1: "",
                address2: "",
                city: "",
                state: "",
                country: "",
                postCode: "",
                orderInventory: [],
                note: "",
                total: 0
            },
            products: [],            
        }
        this.updateOrders = props.updateOrders;
        this.toggleForm = props.toggleForm;


        this.onOrderChange = this.onOrderChange.bind(this);
    }

    componentDidMount() {
        this.getAvailableProducts();
    }

    onOrderChange(event) {
        var order = this.state.orderModel;
        order[event.target.name] = event.target.value;
        this.setState({orderModel: order});
    }

    onOrderInventoryChange(event) {
        var index = event.target.name.split("-")[0];
        var attrib = event.target.name.split("-")[1];
        var orderModel = this.state.orderModel;
        orderModel.orderInventory[index][attrib] = event.target.value;
        this.setState({ orderModel: orderModel });
    }

    updateOrder(event) {
        event.preventDefault();
        var updateModel = {
            orderId: this.state.orderModel.id,
            status: this.state.orderModel.status,
            customerInformation: {
                firstName: this.state.orderModel.firstName,
                lastName: this.state.orderModel.lastName,
                email: this.state.orderModel.email,
                phone: this.state.orderModel.phoneNumber,
                address1: this.state.orderModel.address1,
                address2: this.state.orderModel.address2,
                city: this.state.orderModel.city,
                state: this.state.orderModel.state,
                country: this.state.orderModel.country,
                postCode: this.state.orderModel.postCode
            },
            cart: this.state.orderModel.orderInventory.map(x => {
                return {
                    inventoryId: x.inventoryId,
                    quantity: x.quantity
                }

            }),
            total: this.state.orderModel.total,
            note: this.state.orderModel.note
        };
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/orders/',
            method: "put",
            data: updateModel,
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                this.updateOrders(res.data);              
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
            });
    }

    getAvailableProducts() {
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/inventory',
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                this.setState({ products: res.data });
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
            });
    }

    createOrder(event) {
        event.preventDefault();        
        var creationModel = {            
            status: this.state.orderModel.status,
            customerInformation: {
                firstName: this.state.orderModel.firstName,
                lastName: this.state.orderModel.lastName,
                email: this.state.orderModel.email,
                phone: this.state.orderModel.phoneNumber,
                address1: this.state.orderModel.address1,
                address2: this.state.orderModel.address2,
                city: this.state.orderModel.city,
                state: this.state.orderModel.state,
                country: this.state.orderModel.country,
                postCode: this.state.orderModel.postCode
            },
            cart: this.state.orderModel.orderInventory.map(x => {
                return {
                    id: x.inventoryId,
                    quantity: x.quantity
                }

            }),
            total: this.state.orderModel.total,
            note: this.state.orderModel.note
        };
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/orders/',
            method: "post",
            data: creationModel,
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                this.updateOrders(res.data);                
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
                this.toggleForm();
            });
    }

    render() {
        return (
            <form className="px-4" onSubmit={this.state.isUpdating ? this.updateOrder.bind(this) : this.createOrder.bind(this)}>
                <div><p className="is-size-3 label">Order {this.state.isUpdating ? "Update": "Creation"} Form</p></div>
                <br />
                { this.state.isUpdating &&
                    <div className="columns">
                        <div className="column">
                            <label className="has-text-weight-semibold">Order Id</label>
                            <p>{this.state.orderModel.id}</p>
                        </div>
                        <div className="column">
                            <label className="has-text-weight-semibold">Stripe Reference</label>
                            <p>{this.state.orderModel.stripeRef}</p>
                        </div>
                    </div>
                }
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">First Name*</label>
                        <input
                            className="input"
                            name="firstName"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.firstName}
                            required
                        />
                    </div>
                    <div className="column">
                        <label className="has-text-weight-semibold">Last Name*</label>
                        <input
                            className="input"
                            name="lastName"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.lastName}
                            required
                        />
                    </div>
                </div>
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">Phone Number</label>
                        <input
                            type="tel"
                            className="input"
                            name="phoneNumber"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.phoneNumber}

                        />
                    </div>
                    <div className="column">
                        <label className="has-text-weight-semibold">Email*</label>
                        <input
                            className="input"
                            type="email"
                            name="email"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.email}
                            required
                        />
                    </div>
                </div>
                <br /><br />
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">Address 1*</label>
                        <input
                            className="input"
                            name="address1"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.address1}
                            required
                        />
                    </div>
                    <div className="column">
                        <label className="has-text-weight-semibold">Address 2</label>
                        <input
                            className="input"
                            name="address2"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.address2}

                        />
                    </div>
                </div>
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">City*</label>
                        <input
                            className="input"
                            name="city"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.city}
                            required
                        />
                    </div>
                    <div className="column">
                        <label className="has-text-weight-semibold">State/Province*</label>
                        <input
                            className="input"
                            name="state"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.state}
                            required
                        />
                    </div>
                </div>
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">Post Code*</label>
                        <input
                            className="input"
                            name="postCode"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.postCode}
                            required
                        />
                    </div>
                    <div className="column">
                        <label className="has-text-weight-semibold">Country*</label>
                        <input
                            className="input"
                            name="country"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.country}
                            required
                        />
                    </div>

                </div>
                <br /><br />
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">Status</label><br />
                        <div className="select">
                            <select
                                name="status"
                                onChange={this.onOrderChange}
                                value={this.state.orderModel.status}>
                                <option value="ordered">Ordered</option>
                                <option value="ordered">Shipped</option>
                                <option value="ordered">Filled</option>
                            </ select>
                        </div>
                    </div>
                    <div className="column">
                        {this.state.isUpdating && 
                            <div>
                                <label className="has-text-weight-semibold">Date</label>
                                <p>{this.state.orderModel.orderDate && this.state.orderModel.orderDate.split("T")[0]}</p>
                            </div>
                        }
                    </div>
                </div>
                <div className="columns">
                    <div className="column">
                        <label className="has-text-weight-semibold">Total</label>
                        <input
                            className="input"
                            name="total"
                            onChange={this.onOrderChange}
                            value={this.state.orderModel.total} />
                    </div>
                    <div className="column">
                    </div>
                </div>
                <div>
                    <label className="has-text-weight-semibold">Note: </label>
                    <textarea className="textarea" onChange={this.onOrderChange} value={this.state.orderModel.note} name="note" rows="5" />
                </div>
                <div>
                    <p className="is-size-4 has-text-weight-semibold">Order Items</p>
                </div>
                <div className="pl-6">
                    <div className="select mr-4">
                        <select id="orderPanelItemSelect" className="is-inline-block" >
                            {this.state.products.map((product, i) => {                                
                                return product.inventory.map((item, j) => {
                                    var inventory = this.state.orderModel.orderInventory;
                                    for (var x = 0; x < inventory.length; x++) {
                                        if (inventory[x].inventoryId === item.id)
                                            return "";
                                    }
                                    return (
                                        <option value={i + "-" + j} key={i + "-" + j} >
                                            {product.productName + " " + item.description}
                                        </option>

                                    );
                                })
                            })}
                        </select>
                    </div>
                    <button type="button"
                        className="button is-primary is-inline-block"
                        onClick={() => {
                            var orderModel = this.state.orderModel;
                            var itemId = document.getElementById("orderPanelItemSelect").value;
                            var i = itemId.split("-")[0];
                            var j = itemId.split("-")[1];
                            
                            orderModel.orderInventory.push({
                                description: this.state.products[i].inventory[j].description,
                                inventoryId: this.state.products[i].inventory[j].id,
                                productName: this.state.products[i].productName,
                                quantity: 1
                            });
                            this.setState({ orderModel: orderModel });
                        }}
                    >Add</button>
                </div>
                <br />
                <div className="pl-6">
                    {this.state.orderModel.orderInventory.map((x, i) => {
                        return (
                            <div className="columns" style={{ "maxWidth": "500px" }} key={i}>
                                <div className="column">{x.productName} - {x.description}</div>
                                <div className="column">
                                    <input
                                        className="input"
                                        name={i + "-quantity"}
                                        onChange={this.onOrderInventoryChange.bind(this)}
                                        value={x.quantity}>
                                    </input>
                                </div>
                                <div>
                                    <button type="button" className="button is-text has-text-danger" onClick={() => {
                                        var orderModel = this.state.orderModel;
                                        orderModel.orderInventory.splice(i, 1);
                                        this.setState({ orderModel: orderModel });
                                    }}>Remove</button>
                                </div>
                            </div>
                        );
                    })}
                </div>
                <br />
                <div className="has-text-right" type="button">
                    <button type="submit" className="button is-success">Submit</button>
                </div>
            </form>
        );
    }

}