import React, { Component } from 'react';
import Layout from './../Layout/Layout';

import ProductPanel from './ProductPanel/ProductPanel';
import InventoryPanel from './InventoryPanel/InventoryPanel';
import OrderPanel from './OrderPanel/OrderPanel';
export default class AdminPanel extends Component {
    constructor(props) {
        super(props);
        this.state = {
            panelPage: props.panelPage ? props.panelPage : 0
        }
    }

    getPanelPage(panelPage) {
        if (panelPage === 0) {
            return <ProductPanel />
        } else if (panelPage === 1) {
            return <OrderPanel />
        } else {
            return <InventoryPanel />         
        }
    }

    render() {
        return (
            <Layout>
                <div className="columns">
                    <div className="column is-2 p-6">
                        <div className="menu ">                            
                            <ul className="menu-list">
                                <li><a className={this.state.panelPage === 0 ? "is-active" : ""} href="admin/product">Product</a></li>
                                <li><a className={this.state.panelPage === 1 ? "is-active" : ""} href="admin/order">Orders</a></li>
                                <li><a className={this.state.panelPage === 2 ? "is-active" : ""} href="admin/stock">Inventory</a></li>
                            </ul>
                        </div>
                    </div>
                    <div className="column is-10">
                        {this.getPanelPage(this.state.panelPage)}
                    </div>                    
                </div>
            </Layout>
        );
    }
}
