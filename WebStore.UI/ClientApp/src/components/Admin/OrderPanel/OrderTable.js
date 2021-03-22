import React from 'react';
import axios from 'axios';
import Cookies from './../../../lib/Cookies';
export default class OrderTable extends React.Component {
    constructor(props) {
        super(props);
        this.state = {            
            orders: props.orders ? props.orders : [],
            
        }
        this.getOrder = props.getOrder;
        this.toggleForm = props.toggleForm;
        this.viewOrder = props.viewOrder;
        this.setOrders = props.setOrders;
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (nextProps.orders.length !== this.state.orders.length) {
            nextState.orders = nextProps.orders;
            return true;
        }
            
        for (var order of this.state.orders) {
            if (!this.state.orders.includes(order)) {
                nextState.orders = nextProps.orders;
                return true;
            }
        }
        
        return false;
    }

    deleteOrder(id) {
        var orderId = this.state.orders[id].id;
        this.setState({ loading: true });
        axios({
            url: process.env.REACT_APP_API_HOST +
                `/api/admin/orders/${orderId}`,
            method: "delete",
            headers: {
                Authorization: `Bearer ${Cookies.getCookie("JWT_Token")}`
            }
        })
            .then(res => {
                var orders = this.state.orders;
                orders.splice(id, 1);
                this.setState({ orders: orders });
                this.setOrders(orders);
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
            <table className="table" width="100%">
                <thead>
                    <tr>
                        <td>Id</td>
                        <td>Status</td>
                        <td>Name</td>
                        <td>Email</td>
                        <td>Phone</td>
                        <td>Total</td>
                        <td>Date</td>                        
                        <td />
                        <td />
                    </tr>
                </thead>
                <tbody>
                    {this.state.orders.map((order, i) => {
                        return (
                            <tr key={i} >
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.id}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.status}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.name}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.email}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.phone}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    ${order.total}
                                </td>
                                <td className="is-clickable" onClick={() => this.viewOrder(i)}>
                                    {order.orderDate}
                                </td>
                                <td >
                                    <button className="button is-warning" onClick={() => this.getOrder(i).then(()=>this.toggleForm())}>Edit</button>
                                </td>
                                <td>
                                    <button className="button is-danger" onClick={() => { this.deleteOrder(i) }}>Delete</button>
                                </td>
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        );
    }

}