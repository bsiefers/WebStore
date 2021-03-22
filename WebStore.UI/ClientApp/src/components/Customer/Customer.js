import React, { Component } from 'react';
export default class Customer extends Component {
    constructor(props) {
        super(props);
        var customer = props.customer ? props.customer : JSON.parse(window.localStorage.getItem("customer_info"));
        console.log(customer);
        this.state = {
            customer: customer
        };

    }



    render() {
        return (
            <div style={{"maxWidth": "400px"}}>                
                <h4 className="is-size-4 has-text-weight-semibold">Customer Infomation</h4>
                <div className="pl-5">
                    <div >
                        <h4 className="is-size-6 has-text-weight-bold">Contact Info: </h4>
                        <div className="columns">
                            <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Name:</h4>
                                <p>{this.state.customer.firstName} {this.state.customer.lastName}</p>
                            </div>
                            <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Email: </h4>
                                <p>{this.state.customer.email}</p>
                            </div>
                            { this.state.customer.phone &&
                                <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Phone: </h4>
                                    {this.state.customer.phone && <p>{this.state.customer.phone}</p>}
                                </div>
                            }
                        </div>
                    </div>
                    <div>
                        <h4 className="is-size-6 has-text-weight-bold">Delivery Address: </h4>
                        <div className="columns">
                            <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">City:</h4>
                                <p>{this.state.customer.city}</p>
                            </div>
                            <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Zip Code:</h4>
                                <p>{this.state.customer.postCode}</p>
                            </div>
                        </div>
                        <div className="columns">
                            <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Address 1:</h4>
                                <p>{this.state.customer.address1}</p>
                            </div>
                            { this.state.customer.address2 &&
                                <div className="column">
                                <h4 className="is-size-6 has-text-weight-semibold">Address 2:</h4>
                                    <p>{this.state.customer.address2}</p>
                                </div>
                            }
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
