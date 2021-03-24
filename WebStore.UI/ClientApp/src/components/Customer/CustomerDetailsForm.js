import React, { Component } from 'react';
import Layout from './../Layout/Layout';
var inlineBlock = { "display": "inline-block" };

export default class CustomerDetailsForm extends Component {
    constructor(props) {
        super(props);
        var customerInfo = JSON.parse(window.localStorage.getItem("customer_info"));

        this.state = {
            customerInfo: customerInfo ? customerInfo : {
                firstName: "",
                lastName: "",                
                email: "",
                phone: "",
                city: "",
                country: "",
                state: "",
                postCode: "",
                address1: "",
                address2: ""
            },
            isValidData: {valid: true, invalidInputName: ""}
        };
        
        this.onSubmit = this.onSubmit.bind(this);
        this.onChangeCustomerInfo = this.onChangeCustomerInfo.bind(this);
    }

    onChangeCustomerInfo(event) {
        var customerInfo = this.state.customerInfo;
        customerInfo[event.target.name] = event.target.value;
        this.setState({customerInfo: customerInfo});
    }

    onSubmit(event) {
        event.preventDefault();

        window.localStorage.setItem("customer_info", JSON.stringify(this.state.customerInfo));
        window.location.href = "payment";
    }

    render() {
        return (
            <Layout>
                <div className="container has-text-centered">
                    <form onSubmit={this.onSubmit}>
                        <div style={inlineBlock}>
                        
                            <div className="has-text-left has-text-weight-bold is-size-3"><h2>Customer Info:</h2></div><br />
                            <div className="has-text-left has-text-weight-semibold is-size-5"><h3>Contact Info:</h3></div>
                            <div className="has-text-weight-medium">
                                <div style={inlineBlock}>
                                    <div  className="has-text-left"><label >First Name*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="firstName" value={this.state.customerInfo.firstName} required></input>                    
                                </div>
                                {"  "}
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Last Name*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="lastName" value={this.state.customerInfo.lastName} required></input>
                                </div>
                            </div>
                            <div className="has-text-weight-medium">
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Email*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="email" type="email" value={this.state.customerInfo.email} required></input>
                                </div>
                                {"  "}
                                <div style={inlineBlock}>
                                    <div className="has-text-left "><label>Phone</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="phone" value={this.state.customerInfo.phone}></input>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div style={inlineBlock}>
                            <div className="has-text-left has-text-weight-semibold is-size-5"><h3>Shipping Details:</h3></div>
                            <div className="has-text-weight-medium">
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>City*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="city" value={this.state.customerInfo.city} required></input>
                                </div>
                                {"  "}
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Postcode*</label> </div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="postCode" value={this.state.customerInfo.postCode} required></input>
                                </div>
                            </div>
                            <div className="has-text-weight-medium">
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>State/Province*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="state" value={this.state.customerInfo.state} required></input>
                                </div>
                                {"  "}
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Country*</label> </div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="country" value={this.state.customerInfo.country} required></input>
                                </div>
                            </div>
                            <div className="has-text-weight-medium">
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Address 1*</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="address1" value={this.state.customerInfo.address1} required></input>
                                </div>
                                {"  "}
                                <div style={inlineBlock}>
                                    <div className="has-text-left"><label>Address 2</label></div>
                                    <input className="input" onChange={this.onChangeCustomerInfo} name="address2" value={this.state.customerInfo.address2}></input>
                                </div>
                            </div>
                            <br />
                            <div className="has-text-right"><button className="button is-success" type="submit">Submit</button></div>
                        </div>                                        
                    </form>
                </div>
            </Layout>
        );
    }
}
