import React from 'react';
import axios from 'axios';
import Cookies from '../../../lib/Cookies';
export default class OrderDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedOrder: props.selectedOrder,
            orders: props.orders

        }
        this.toggleForm = props.toggleForm;
        this.removeSelectedOrder = props.removeSelectedOrder;
        this.setOrders = props.setOrders;
    }


    deleteOrder() {
        var id = this.state.selectedOrder.id;
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST +
                `/api/admin/orders/${id}`,
            method: "delete",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                var orders = this.state.orders;
                var orderIndex;
                for (var i = 0; i < orders.length; i++){
                    if (orders[i].id === this.state.selectedOrder.id) {
                        orderIndex = i;
                        break;
                    }                        
                }                   
                orders.splice(orderIndex, 1);           
                this.setState({ orders: orders });                
                
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
                this.setOrders({orders: this.state.orders});
            });
    }

    render() {

        return (
            <div className="modal is-active">
                <div className="modal-background"></div>
                <div className="modal-content has-background-white" style={{"borderRadius": "10px", "overflowX": "hidden"}}>                    
                    <header className="modal-card-head">
                        <p className="modal-card-title">Order Details</p>
                        <button className="delete" aria-label="close" onClick={this.removeSelectedOrder}></button>
                    </header>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">First Name</label>
                                <p>{this.state.selectedOrder.firstName}</p>
                            </div>
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Last Name</label>
                                <p>{this.state.selectedOrder.lastName}</p>                                
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Phone Number</label>
                                <p>{this.state.selectedOrder.phoneNumber}</p>
                            </div>  
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Email</label>
                                <p>{this.state.selectedOrder.email}</p>
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Address 1</label>
                                <p>{this.state.selectedOrder.address1}</p>
                            </div>
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Address 2</label>
                                <p>{this.state.selectedOrder.address2}</p>
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">City</label>
                                <p>{this.state.selectedOrder.city}</p>
                            </div>
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">State/Province</label>
                                <p>{this.state.selectedOrder.state}</p>
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Post Code</label>
                                <p>{this.state.selectedOrder.postCode}</p>
                            </div>
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Country</label>
                                <p>{this.state.selectedOrder.country}</p>
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Status</label>
                                <p>{this.state.selectedOrder.status}</p>
                            </div>
                        </div>
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Date</label>
                                <p>{this.state.selectedOrder.orderDate}</p>
                            </div>
                        </div>
                    </div>
                    <div className="columns">
                        <div className="column">
                            <div style={{ "margin": "auto", "width": "150px" }}>
                                <label className="has-text-weight-semibold">Total</label>
                                <p>${this.state.selectedOrder.total}</p>
                            </div>
                        </div>
                        <div className="column">
                        </div>
                    </div>
                    <div className="px-6 " >
                        <label className="has-text-weight-semibold">Note</label>
                        <p  style={{ "minHeight": "150px"}}>{this.state.selectedOrder.note}</p>
                    </div>
                    <div className="pl-6">
                        <label className="has-text-weight-semibold">Ordered Items</label>
                        {this.state.selectedOrder.orderInventory.map((x, i) => {
                            return (
                                <div className="ml-6 mb-4" key={i}>
                                    <p className="has-text-weight-medium">{x.productName} - {x.description}</p>
                                    <p className="has-text-weight-light">quantity: {x.quantity}</p>                                             
                                </div>
                            );
                        })}
                    </div>
                    <div className="has-text-right">
                        <button className="button is-warning m-4" onClick={this.toggleForm}>Edit</button>
                        <button className="button is-danger m-4" onClick={() => { this.deleteOrder(); }}>Delete</button>
                    </div>
                    <button className="modal-close is-large" aria-label="close" onClick={this.removeSelectedOrder}></button>
                </div>
            </div>    
            
        );
    }
}