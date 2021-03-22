import React from 'react';
import axios from 'axios';
import Cookies from '../../../lib/Cookies';
import OrderForm from './OrderForm';
import OrderTable from './OrderTable';
import OrderDetails from './OrderDetails';
export default class OrderPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            orders: [],
            loading: false,
            selectedOrder: null,
            creating: false
        }
        this.toggleForm = this.toggleForm.bind(this);
        this.removeSelectedOrder = this.removeSelectedOrder.bind(this);
        this.setOrders = this.setOrders.bind(this);
    }

    componentDidMount() {                
        this.getOrders();
    }

    getOrders() {
        this.setState({ loading: true });
        return axios({
            url: process.env.REACT_APP_API_HOST + '/api/admin/orders',
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {                
                this.setState({ orders: res.data.orders });
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
            });
    }
    setOrders(orders) {        
        this.setState({ orders: orders, selectedOrder: null });
    }


    getOrder(id) {
        var orderId = this.state.orders[id].id;
        this.setState({ loading: true });
        return axios({
            url: process.env.REACT_APP_API_HOST + `/api/admin/orders/id/${orderId}`,
            method: "get",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                console.log(res);
                this.setState({ selectedOrder: res.data });
            })
            .catch(err => {
                console.log(err);
            })
            .then(res => {
                this.setState({ loading: false });
            });
    }

    toggleForm() {
        this.setState({ creating: !this.state.creating });
    }

    removeSelectedOrder(){
        this.setState({ selectedOrder: null });
    }   

    updateOrders(newOrder) {
        var orders = this.state.orders;
        for (var i = 0; i < orders.length; i++) {
            if (orders[i].id === newOrder.id) {
                orders[i] = newOrder;
                this.setState({orders: orders, selectedOrder: null});
                return;
            }
        }
        this.state.orders.push(newOrder);
        this.setState({selectedOrder: null});
    }

    viewOrder(id) {
        this.getOrder(id);
        this.setState({creating: true});
    }

    render() {        
        if (this.state.selectedOrder && this.state.creating) {
            return (
                <div>
                    <div className="tabs">
                        <ul>
                            <li onClick={() => { this.toggleForm(); this.setState({ selectedOrder: null }); }} className={!this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>View Orders</a></li>
                            <li onClick={this.toggleForm} className={this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>Edit Order</a></li>
                        </ul>
                    </div>
                    <OrderForm toggleForm={this.toggleForm} isUpdating={true} order={this.state.selectedOrder} updateOrders={this.updateOrders.bind(this)} />
                </div>
            );
        } else if (this.state.creating) {
            return (
                <div>
                    <div className="tabs">
                        <ul>
                            <li onClick={this.toggleForm} className={!this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>View Orders</a></li>
                            <li onClick={this.toggleForm} className={this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>New Order</a></li>
                        </ul>
                    </div>
                    <OrderForm toggleForm={this.toggleForm} isUpdating={false} updateOrders={this.updateOrders.bind(this)} />;
                </div>
            );
        } else if (this.state.selectedOrder) {

            return (
                <div>
                    <OrderDetails setOrders={this.setOrders} removeSelectedOrder={this.removeSelectedOrder} selectedOrder={this.state.selectedOrder} orders={this.state.orders} toggleForm={this.toggleForm} />
                    <div className="tabs">
                        <ul>
                            <li onClick={this.toggleForm} className={!this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>View Orders</a></li>
                            <li onClick={this.toggleForm} className={this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>New Order</a></li>
                        </ul>
                    </div>
                    <OrderTable setOrders={this.setOrders} viewOrder={this.viewOrder} orders={this.state.orders} getOrder={this.getOrder.bind(this)} toggleForm={this.toggleForm} />
                </div>
            );
        } else {
            return (
                <div>                    
                    <div className="tabs">
                        <ul>
                            <li onClick={this.toggleForm} className={!this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>View Orders</a></li>
                            <li onClick={this.toggleForm} className={this.state.creating ? "is-active" : ""}><a href="# " onClick={e => e.preventDefault()}>New Order</a></li>
                        </ul>
                    </div>
                    <OrderTable setOrders={this.setOrders} viewOrder={this.viewOrder} orders={this.state.orders} getOrder={this.getOrder.bind(this)} toggleForm={this.toggleForm} />
                </div>
            );
        }
    }
}